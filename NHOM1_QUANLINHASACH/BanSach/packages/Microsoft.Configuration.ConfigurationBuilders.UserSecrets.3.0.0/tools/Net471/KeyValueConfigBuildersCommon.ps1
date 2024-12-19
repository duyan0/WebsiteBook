# Copyright (c) .NET Foundation. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.


##
## Assigning a "DefaultValue" to a ParameterDescription will result in emitting this parameter when
## writing out a default builder declaration.
##
## Setting IsRequired to $true will require the attribute to be set on all declarations in config.
##
## Parameters that are not in the allowed list will be dropped from config declarations.
##
Add-Type @"
	using System;

	public class ValueMap {
		public string OriginalValue;
		public string NewValue;
	}

	public class ParameterDescription {
		public string Name;
		public string DefaultValue;
		public bool IsRequired;
		public string MigrateTo;
		public ValueMap[] ValueMigration;
		public bool IsObsolete;
		public string[] ObsoleteValues;
	}

	public class BuilderDescription {
		public string TypeName;
		public string Assembly;
		public string Version;
		public string DefaultName;
		public ParameterDescription[] AllowedParameters;
	}
"@

$keyValueCommonParameters = @(
		[ParameterDescription]@{ Name="mode"; IsRequired=$false; MigrateTo="mode";
			ValueMigration=@(
				[ValueMap]@{ OriginalValue="Expand"; NewValue="Token"}
			) },
		[ParameterDescription]@{ Name="prefix"; IsRequired=$false },
		[ParameterDescription]@{ Name="stripPrefix"; IsRequired=$false },
		[ParameterDescription]@{ Name="tokenPattern"; IsRequired=$false },
		[ParameterDescription]@{ Name="escapeExpandedValues"; IsRequired=$false },
		[ParameterDescription]@{ Name="charMap"; IsRequired=$false },
		[ParameterDescription]@{ Name="enabled"; IsRequired=$false },
		[ParameterDescription]@{ Name="optional"; IsRequired=$false; MigrateTo="enabled";
			ValueMigration=@(
				[ValueMap]@{ OriginalValue="true"; NewValue="optional"},
				[ValueMap]@{ OriginalValue="false"; NewValue="enabled"}
			) }
		);

function CommonInstall($builderDescription) {
	##### Update/Rehydrate config declarations #####
	$config = ReadConfigFile
	$rehydratedCount = RehydrateOldDeclarations $config $builderDescription
	$updatedCount = UpdateDeclarations $config $builderDescription
	if ($rehydratedCount -le 0) { AddDefaultDeclaration $config $builderDescription }
	SaveConfigFile $config
}

function CommonUninstall($builderType) {
	##### Dehydrate config declarations #####
	$config = ReadConfigFile
	DehydrateDeclarations $config $builderType | Out-Null
	SaveConfigFile $config
}

function GetConfigFileName() {
	# Try web.config first. Then fall back to app.config.
	$configFile = $project.ProjectItems | where { $_.Name -ieq "web.config" }
	if ($configFile -eq $null) { $configFile = $project.ProjectItems | where { $_.Name -ieq "app.config" } }
	$configPath = $configFile.Properties | where { $_.Name -ieq "LocalPath" }
    if ($configPath -eq $null) { $configPath = $configFile.Properties | where { $_.Name -ieq "FullPath" } }
	return $configPath.Value
}

function GetTempFileName() {
	$uname = $project.UniqueName
	if ([io.path]::IsPathRooted($uname)) { $uname = $project.Name }
	return [io.path]::Combine($env:TEMP, "Microsoft.Configuration.ConfigurationBuilders.KeyValueConfigBuilders.Temp", $uname + ".xml")
}

function ReadConfigFile() {
	$configFile = GetConfigFileName
	$configObj = @{ fileName = $configFile; xml = (Select-Xml -Path "$configFile" -XPath /).Node }
	$configObj.xml.PreserveWhitespace = $true
	return $configObj
}

function DehydrateDeclarations($config, $typeName) {
	$tempFile = GetTempFileName
	$count = 0

	if ([io.file]::Exists($tempFile)) {
		$xml = (Select-Xml -Path "$tempFile" -XPath /).Node
	} else {
		$xml = New-Object System.Xml.XmlDocument
		$xml.PreserveWhitespace = $true
		$xml.AppendChild($xml.CreateElement("driedDeclarations")) | Out-Null
	}

	foreach ($rec in $config.xml.configuration.configBuilders.builders.add  | where { IsSameType $_.type $typeName }) {
		# Remove records from config.
		$config.xml.configuration.configBuilders.builders.RemoveChild($rec) | Out-Null

		# Add the record to the temp stash. Don't worry about duplicates.
		AppendBuilderNode $xml.ImportNode($rec, $true) $xml.DocumentElement | Out-Null
		$count++
	}

	# Save the dehydrated declarations
	$tmpFolder = Split-Path $tempFile
	md -Force $tmpFolder | Out-Null
	$xml.Save($tempFile) | Out-Null
	return $count
}

function RehydrateOldDeclarations($config, $builderDescription) {
	$tempFile = GetTempFileName
	if (![io.file]::Exists($tempFile)) { return 0 }

	$count = 0
	$xml = (Select-Xml -Path "$tempFile" -XPath /).Node
	$xml.PreserveWhitespace = $true

	foreach($rec in $xml.driedDeclarations.add | where { IsSameType $_.type ($builderDescription.TypeName + "," + $builderDescription.Assembly) }) {
		# Remove records that match type, even if we don't end up rehydrating them.
		$xml.driedDeclarations.RemoveChild($rec) | Out-Null

		# Skip if an existing record of the same name already exists.
		$existingRecord = $config.xml.configuration.configBuilders.builders.add | where { $_.name -eq $rec.name }
		if ($existingRecord -ne $null) { continue }

		# Bring the record back to life
		AppendBuilderNode $config.xml.ImportNode($rec, $true) $config.xml.configuration.configBuilders.builders | Out-Null
		$count++
		Write-Host "Restored configBuilder '$($rec.name)'."
	}

	# Make dried record removal permanent
	$xml.Save($tempFile) | Out-Null

	return $count
}

function UpdateDeclarations($config, $builderDescription) {
	$count = 0

	foreach ($builder in $config.xml.configuration.configBuilders.builders.add | where { IsSameType $_.type ($builderDescription.TypeName + "," + $builderDescription.Assembly) }) {

		$failed = $false

		# Migrate parameters that have changed
		foreach ($p in $builderDescription.AllowedParameters | where { $_.MigrateTo -ne $null }) {
			if ($builder.($p.Name) -ne $null) {
				$oldvalue = $builder.($p.Name)
				$newvalue = ($p.ValueMigration | where { $_.OriginalValue -eq $oldvalue } | select -First 1 ).NewValue
				if ($newvalue -eq $null) { $newvalue = $oldvalue }
				$builder.RemoveAttribute($p.Name) | Out-Null
				$builder.SetAttribute($p.MigrateTo, $newvalue) | Out-Null
				Write-Host "Migrated '$($p.Name):$($oldvalue)' to '$($p.MigrateTo):$($newvalue)' for configBuilder '$($builder.name)'"
			}
		}

		# Add default parameters if they are required and not already present
		foreach ($p in $builderDescription.AllowedParameters | where { $_.IsRequired -eq $true }) {
			if ($builder.($p.Name) -eq $null) {
				if ($p.DefaultValue -eq $null) {
					Write-Warning "Failed to add parameter to '$($builder.name)' configBuilder: '$($p.Name)' is required, but does not have a default value."
					$failed = $true
				}
				$builder.SetAttribute($p.Name, $p.DefaultValue) | Out-Null
				Write-Host "Added default value for parameter '$($p.Name)' to configBuilder '$($builder.name)'"
			}
		}

		# Finally, update type. And do so with remove/add so the 'type' parameter get put at the end
		$builder.RemoveAttribute("type") | Out-Null
		$builder.SetAttribute("type", "$($builderDescription.TypeName), $($builderDescription.Assembly), Version=$($builderDescription.Version), Culture=neutral, PublicKeyToken=31bf3856ad364e35") | Out-Null

		# Check for unknown parameters
		foreach ($attr in $builder.Attributes | where { ($_.Name -ne "name") -and ($_.Name -ne "type") }) {
			if (($builderDescription.AllowedParameters | where { $_.Name -ceq $attr.Name }) -eq $null) {
				# Leave it alone, but spit out a warning?
				Write-Warning "The parameter '$($attr.Name)' on configBuilder '$($builder.name)' is unknown and may cause errors at runtime."
			}
		}

		# Warn about any obsolete settings
		foreach ($p in $builderDescription.AllowedParameters | where { $_.IsObsolete -eq $true }) {
			if ($builder.($p.Name) -ne $null) {
				Write-Warning "The parameter '$($p.Name)' on '$($builder.name)' configBuilder is obsolete. Please consider alternative configurations."
			}
		}
		foreach ($p in $builderDescription.AllowedParameters | where { $_.ObsoleteValues -ne $null -and $_.ObsoleteValues.Count -gt 0 }) {
			if ($builder.($p.Name) -ne $null -and $p.ObsoleteValues.Contains($builder.($p.Name))) {
				Write-Warning "The value of parameter '$($p.Name)' on '$($builder.name)' configBuilder is obsolete. Please consider alternative configurations."
			}
		}

		# Count the existing declaration as updated
		if ($failed -ne $true) { $count++ }
	}

	return $count
}

function AddDefaultDeclaration($config, $builderDescription) {
	$dd = $config.xml.CreateElement("add")

	# name first
	$dd.SetAttribute("name", $builderDescription.DefaultName) | Out-Null

	# everything else in the middle
	foreach ($p in $builderDescription.AllowedParameters) {
		if ($p.IsRequired -and ($p.DefaultValue -eq $null)) {
			Write-Host "Failed to add default declaration for $($builderDescription.TypeName): '$($p.Name)' is required, but does not have a default value."
			return
		}

		if ($p.DefaultValue -ne $null) {
			$dd.SetAttribute($p.Name, $p.DefaultValue) | Out-Null
		}
	}

	# type last
	$dd.SetAttribute("type", "$($builderDescription.TypeName), $($builderDescription.Assembly), Version=$($builderDescription.Version), Culture=neutral, PublicKeyToken=31bf3856ad364e35") | Out-Null

	AppendBuilderNode $dd $config.xml.configuration.configBuilders.builders | Out-Null
	Write-Host "Added default configBuilder '$($dd.name)'."
}

function AppendBuilderNode($builder, $parent, $indentLevel = 3) {
	$lastSibling = $parent.ChildNodes | where { $_ -isnot [System.Xml.XmlWhitespace] } | select -Last 1
	if ($lastSibling -ne $null) {
		# If not the first child, then copy the whitespace convention of the existing child
		$ws = "";
		$prev = $lastSibling.PreviousSibling | where { $_ -is [System.Xml.XmlWhitespace] }
		while ($prev -ne $null) {
			$ws = $prev.data + $ws
			$prev = $prev.PreviousSibling | where { $_ -is [System.Xml.XmlWhitespace] }
		}
		$parent.InsertAfter($builder, $lastSibling) | Out-Null
		if ($ws.length -gt 0) { $parent.InsertAfter($parent.OwnerDocument.CreateWhitespace($ws), $lastSibling) | Out-Null }
		return
	}

	# Add on a new line with indents. Make sure there is no existing whitespace mucking this up.
	foreach ($exws in $parent.ChildNodes | where { $_ -is [System.Xml.XmlWhitespace] }) { $parent.RemoveChild($exws) | Out-Null }
	$parent.AppendChild($parent.OwnerDocument.CreateWhitespace("`r`n")) | Out-Null
	$parent.AppendChild($parent.OwnerDocument.CreateWhitespace("  " * $indentLevel)) | Out-Null
	$parent.AppendChild($builder) | Out-Null
	$parent.AppendChild($parent.OwnerDocument.CreateWhitespace("`r`n")) | Out-Null
	$parent.AppendChild($parent.OwnerDocument.CreateWhitespace("  " * ($indentLevel - 1))) | Out-Null
}

function SaveConfigFile($config) {
	$config.xml.Save($config.fileName) | Out-Null
}

function IsSameType($typeString1, $typeString2) {

	if (($typeString1 -eq $null) -or ($typeString2 -eq $null)) { return $false }

	# First check the type
	$t1 = $typeString1.Split(',')[0].Trim()
	$t2 = $typeString2.Split(',')[0].Trim()
	if ($t1 -cne $t2) { return $false }

	# Then check for assembly match if possible
	$a1 = $typeString1.Split(',')[1]
	$a2 = $typeString2.Split(',')[1]
	if (($a1 -ne $null) -and ($a2 -ne $null)) {
		return ($a1.Trim() -eq $a2.Trim())
	}

	# Don't care about assembly. Match is good.
	return $true
}

# SIG # Begin signature block
# MIInvwYJKoZIhvcNAQcCoIInsDCCJ6wCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCDKEEH4+pt4RRI+
# 2rCsAuPD1adca5ZHG1Kk4Yg7OczfCqCCDXYwggX0MIID3KADAgECAhMzAAACy7d1
# OfsCcUI2AAAAAALLMA0GCSqGSIb3DQEBCwUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTEwHhcNMjIwNTEyMjA0NTU5WhcNMjMwNTExMjA0NTU5WjB0MQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMR4wHAYDVQQDExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
# AQC3sN0WcdGpGXPZIb5iNfFB0xZ8rnJvYnxD6Uf2BHXglpbTEfoe+mO//oLWkRxA
# wppditsSVOD0oglKbtnh9Wp2DARLcxbGaW4YanOWSB1LyLRpHnnQ5POlh2U5trg4
# 3gQjvlNZlQB3lL+zrPtbNvMA7E0Wkmo+Z6YFnsf7aek+KGzaGboAeFO4uKZjQXY5
# RmMzE70Bwaz7hvA05jDURdRKH0i/1yK96TDuP7JyRFLOvA3UXNWz00R9w7ppMDcN
# lXtrmbPigv3xE9FfpfmJRtiOZQKd73K72Wujmj6/Su3+DBTpOq7NgdntW2lJfX3X
# a6oe4F9Pk9xRhkwHsk7Ju9E/AgMBAAGjggFzMIIBbzAfBgNVHSUEGDAWBgorBgEE
# AYI3TAgBBggrBgEFBQcDAzAdBgNVHQ4EFgQUrg/nt/gj+BBLd1jZWYhok7v5/w4w
# RQYDVR0RBD4wPKQ6MDgxHjAcBgNVBAsTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEW
# MBQGA1UEBRMNMjMwMDEyKzQ3MDUyODAfBgNVHSMEGDAWgBRIbmTlUAXTgqoXNzci
# tW2oynUClTBUBgNVHR8ETTBLMEmgR6BFhkNodHRwOi8vd3d3Lm1pY3Jvc29mdC5j
# b20vcGtpb3BzL2NybC9NaWNDb2RTaWdQQ0EyMDExXzIwMTEtMDctMDguY3JsMGEG
# CCsGAQUFBwEBBFUwUzBRBggrBgEFBQcwAoZFaHR0cDovL3d3dy5taWNyb3NvZnQu
# Y29tL3BraW9wcy9jZXJ0cy9NaWNDb2RTaWdQQ0EyMDExXzIwMTEtMDctMDguY3J0
# MAwGA1UdEwEB/wQCMAAwDQYJKoZIhvcNAQELBQADggIBAJL5t6pVjIRlQ8j4dAFJ
# ZnMke3rRHeQDOPFxswM47HRvgQa2E1jea2aYiMk1WmdqWnYw1bal4IzRlSVf4czf
# zx2vjOIOiaGllW2ByHkfKApngOzJmAQ8F15xSHPRvNMmvpC3PFLvKMf3y5SyPJxh
# 922TTq0q5epJv1SgZDWlUlHL/Ex1nX8kzBRhHvc6D6F5la+oAO4A3o/ZC05OOgm4
# EJxZP9MqUi5iid2dw4Jg/HvtDpCcLj1GLIhCDaebKegajCJlMhhxnDXrGFLJfX8j
# 7k7LUvrZDsQniJZ3D66K+3SZTLhvwK7dMGVFuUUJUfDifrlCTjKG9mxsPDllfyck
# 4zGnRZv8Jw9RgE1zAghnU14L0vVUNOzi/4bE7wIsiRyIcCcVoXRneBA3n/frLXvd
# jDsbb2lpGu78+s1zbO5N0bhHWq4j5WMutrspBxEhqG2PSBjC5Ypi+jhtfu3+x76N
# mBvsyKuxx9+Hm/ALnlzKxr4KyMR3/z4IRMzA1QyppNk65Ui+jB14g+w4vole33M1
# pVqVckrmSebUkmjnCshCiH12IFgHZF7gRwE4YZrJ7QjxZeoZqHaKsQLRMp653beB
# fHfeva9zJPhBSdVcCW7x9q0c2HVPLJHX9YCUU714I+qtLpDGrdbZxD9mikPqL/To
# /1lDZ0ch8FtePhME7houuoPcMIIHejCCBWKgAwIBAgIKYQ6Q0gAAAAAAAzANBgkq
# hkiG9w0BAQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24x
# EDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlv
# bjEyMDAGA1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5
# IDIwMTEwHhcNMTEwNzA4MjA1OTA5WhcNMjYwNzA4MjEwOTA5WjB+MQswCQYDVQQG
# EwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwG
# A1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSgwJgYDVQQDEx9NaWNyb3NvZnQg
# Q29kZSBTaWduaW5nIFBDQSAyMDExMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIIC
# CgKCAgEAq/D6chAcLq3YbqqCEE00uvK2WCGfQhsqa+laUKq4BjgaBEm6f8MMHt03
# a8YS2AvwOMKZBrDIOdUBFDFC04kNeWSHfpRgJGyvnkmc6Whe0t+bU7IKLMOv2akr
# rnoJr9eWWcpgGgXpZnboMlImEi/nqwhQz7NEt13YxC4Ddato88tt8zpcoRb0Rrrg
# OGSsbmQ1eKagYw8t00CT+OPeBw3VXHmlSSnnDb6gE3e+lD3v++MrWhAfTVYoonpy
# 4BI6t0le2O3tQ5GD2Xuye4Yb2T6xjF3oiU+EGvKhL1nkkDstrjNYxbc+/jLTswM9
# sbKvkjh+0p2ALPVOVpEhNSXDOW5kf1O6nA+tGSOEy/S6A4aN91/w0FK/jJSHvMAh
# dCVfGCi2zCcoOCWYOUo2z3yxkq4cI6epZuxhH2rhKEmdX4jiJV3TIUs+UsS1Vz8k
# A/DRelsv1SPjcF0PUUZ3s/gA4bysAoJf28AVs70b1FVL5zmhD+kjSbwYuER8ReTB
# w3J64HLnJN+/RpnF78IcV9uDjexNSTCnq47f7Fufr/zdsGbiwZeBe+3W7UvnSSmn
# Eyimp31ngOaKYnhfsi+E11ecXL93KCjx7W3DKI8sj0A3T8HhhUSJxAlMxdSlQy90
# lfdu+HggWCwTXWCVmj5PM4TasIgX3p5O9JawvEagbJjS4NaIjAsCAwEAAaOCAe0w
# ggHpMBAGCSsGAQQBgjcVAQQDAgEAMB0GA1UdDgQWBBRIbmTlUAXTgqoXNzcitW2o
# ynUClTAZBgkrBgEEAYI3FAIEDB4KAFMAdQBiAEMAQTALBgNVHQ8EBAMCAYYwDwYD
# VR0TAQH/BAUwAwEB/zAfBgNVHSMEGDAWgBRyLToCMZBDuRQFTuHqp8cx0SOJNDBa
# BgNVHR8EUzBRME+gTaBLhklodHRwOi8vY3JsLm1pY3Jvc29mdC5jb20vcGtpL2Ny
# bC9wcm9kdWN0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFfMDNfMjIuY3JsMF4GCCsG
# AQUFBwEBBFIwUDBOBggrBgEFBQcwAoZCaHR0cDovL3d3dy5taWNyb3NvZnQuY29t
# L3BraS9jZXJ0cy9NaWNSb29DZXJBdXQyMDExXzIwMTFfMDNfMjIuY3J0MIGfBgNV
# HSAEgZcwgZQwgZEGCSsGAQQBgjcuAzCBgzA/BggrBgEFBQcCARYzaHR0cDovL3d3
# dy5taWNyb3NvZnQuY29tL3BraW9wcy9kb2NzL3ByaW1hcnljcHMuaHRtMEAGCCsG
# AQUFBwICMDQeMiAdAEwAZQBnAGEAbABfAHAAbwBsAGkAYwB5AF8AcwB0AGEAdABl
# AG0AZQBuAHQALiAdMA0GCSqGSIb3DQEBCwUAA4ICAQBn8oalmOBUeRou09h0ZyKb
# C5YR4WOSmUKWfdJ5DJDBZV8uLD74w3LRbYP+vj/oCso7v0epo/Np22O/IjWll11l
# hJB9i0ZQVdgMknzSGksc8zxCi1LQsP1r4z4HLimb5j0bpdS1HXeUOeLpZMlEPXh6
# I/MTfaaQdION9MsmAkYqwooQu6SpBQyb7Wj6aC6VoCo/KmtYSWMfCWluWpiW5IP0
# wI/zRive/DvQvTXvbiWu5a8n7dDd8w6vmSiXmE0OPQvyCInWH8MyGOLwxS3OW560
# STkKxgrCxq2u5bLZ2xWIUUVYODJxJxp/sfQn+N4sOiBpmLJZiWhub6e3dMNABQam
# ASooPoI/E01mC8CzTfXhj38cbxV9Rad25UAqZaPDXVJihsMdYzaXht/a8/jyFqGa
# J+HNpZfQ7l1jQeNbB5yHPgZ3BtEGsXUfFL5hYbXw3MYbBL7fQccOKO7eZS/sl/ah
# XJbYANahRr1Z85elCUtIEJmAH9AAKcWxm6U/RXceNcbSoqKfenoi+kiVH6v7RyOA
# 9Z74v2u3S5fi63V4GuzqN5l5GEv/1rMjaHXmr/r8i+sLgOppO6/8MO0ETI7f33Vt
# Y5E90Z1WTk+/gFcioXgRMiF670EKsT/7qMykXcGhiJtXcVZOSEXAQsmbdlsKgEhr
# /Xmfwb1tbWrJUnMTDXpQzTGCGZ8wghmbAgEBMIGVMH4xCzAJBgNVBAYTAlVTMRMw
# EQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVN
# aWNyb3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNp
# Z25pbmcgUENBIDIwMTECEzMAAALLt3U5+wJxQjYAAAAAAsswDQYJYIZIAWUDBAIB
# BQCgga4wGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcCAQQwHAYKKwYBBAGCNwIBCzEO
# MAwGCisGAQQBgjcCARUwLwYJKoZIhvcNAQkEMSIEIPlyvnKdjMTf/qqiHe6oTEi8
# qd+pI0Q2drSmoA8uvONtMEIGCisGAQQBgjcCAQwxNDAyoBSAEgBNAGkAYwByAG8A
# cwBvAGYAdKEagBhodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20wDQYJKoZIhvcNAQEB
# BQAEggEACTj4b2/4E2EttWdIpzr68qVcfWJ1DN6FdfhlpqFO19E/WIaPQs+ED54v
# E+ykz6iRGVn5Nna+ayLp4H42J74epEU26kMU06HEvKYe93EEX9Vge686q4fTOv4K
# Qwb0GOqBmWPAIIZB7nuKVviY1U7jZFqexB7AG46FzCmWC2aVZhpY89f50gQJeCKa
# wOcZQSzcof65t6ksP9pLi2diGOVRwFyGeEGI79kQv/mXOrrwuIF/cgz1htE3+iwX
# WvNZdZT6q5aok5X6VLif3uGcOEChkEXnGhJT2jqUKGERct9ckO96a3DyTl9hXhhH
# kjfhG04GtKg+5IZLRDHMRtLzPzCrnaGCFykwghclBgorBgEEAYI3AwMBMYIXFTCC
# FxEGCSqGSIb3DQEHAqCCFwIwghb+AgEDMQ8wDQYJYIZIAWUDBAIBBQAwggFZBgsq
# hkiG9w0BCRABBKCCAUgEggFEMIIBQAIBAQYKKwYBBAGEWQoDATAxMA0GCWCGSAFl
# AwQCAQUABCA7iZPxF4kGcA31tfaA9iQdlwqGzvUp+H86GMp0Y6wE4QIGY/dactUD
# GBMyMDIzMDMyMTIxMjA1Ny41NjZaMASAAgH0oIHYpIHVMIHSMQswCQYDVQQGEwJV
# UzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UE
# ChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMS0wKwYDVQQLEyRNaWNyb3NvZnQgSXJl
# bGFuZCBPcGVyYXRpb25zIExpbWl0ZWQxJjAkBgNVBAsTHVRoYWxlcyBUU1MgRVNO
# OkQwODItNEJGRC1FRUJBMSUwIwYDVQQDExxNaWNyb3NvZnQgVGltZS1TdGFtcCBT
# ZXJ2aWNloIIReDCCBycwggUPoAMCAQICEzMAAAG6Hz8Z98F1vXwAAQAAAbowDQYJ
# KoZIhvcNAQELBQAwfDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24x
# EDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlv
# bjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUtU3RhbXAgUENBIDIwMTAwHhcNMjIw
# OTIwMjAyMjE5WhcNMjMxMjE0MjAyMjE5WjCB0jELMAkGA1UEBhMCVVMxEzARBgNV
# BAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jv
# c29mdCBDb3Jwb3JhdGlvbjEtMCsGA1UECxMkTWljcm9zb2Z0IElyZWxhbmQgT3Bl
# cmF0aW9ucyBMaW1pdGVkMSYwJAYDVQQLEx1UaGFsZXMgVFNTIEVTTjpEMDgyLTRC
# RkQtRUVCQTElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2VydmljZTCC
# AiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIBAIhOFYMzkjWAE9UVnXF9hRGv
# 0xBRxc+I5Hu3hxVFXyK3u38xusEb0pLkwjgGtDsaLLbrlMxqX3tFb/3BgEPEC3L0
# wX76gD8zHt+wiBV5mq5BWop29qRrgMJKKCPcpQnSjs9B/4XMFFvrpdPicZDv43FL
# gz9fHqMq0LJDw5JAHGDS30TCY9OF43P4d44Z9lE7CaVS2pJMF3L453MXB5yYK/KD
# bilhERP1jxn2yl+tGCRguIAsMG0oeOhXaw8uSGOhS6ACSHb+ebi0038MFHyoTNhK
# f+SYo4OpSY3xP4+swBBTKDoYP1wH+CfxG6h9fymBJQPQZaqfl0riiDLjmDunQtH1
# GD64Air5k9Jdwhq5wLmSWXjyFVL+IDfOpdixJ6f5o+MhE6H4t31w+prygHmd2UHQ
# 657UGx6FNuzwC+SpAHmV76MZYac4uAhTgaP47P2eeS1ockvyhl9ya+9JzPfMkug3
# xevzFADWiLRMr066EMV7q3JSRAsnCS9GQ08C4FKPbSh8OPM33Lng0ffxANnHAAX/
# DE7cHcx7l9jaV3Acmkj7oqir4Eh2u5YxwiaTE37XaMumX2ES3PJ5NBaXq7YdLJwy
# SD+U9pk/tl4dQ1t/Eeo7uDTliOyQkD8I74xpVB0T31/67KHfkBkFVvy6wye21V+9
# IC8uSD++RgD3RwtN2kE/AgMBAAGjggFJMIIBRTAdBgNVHQ4EFgQUimLm8QMeJa25
# j9MWeabI2HSvZOUwHwYDVR0jBBgwFoAUn6cVXQBeYl2D9OXSZacbUzUZ6XIwXwYD
# VR0fBFgwVjBUoFKgUIZOaHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9j
# cmwvTWljcm9zb2Z0JTIwVGltZS1TdGFtcCUyMFBDQSUyMDIwMTAoMSkuY3JsMGwG
# CCsGAQUFBwEBBGAwXjBcBggrBgEFBQcwAoZQaHR0cDovL3d3dy5taWNyb3NvZnQu
# Y29tL3BraW9wcy9jZXJ0cy9NaWNyb3NvZnQlMjBUaW1lLVN0YW1wJTIwUENBJTIw
# MjAxMCgxKS5jcnQwDAYDVR0TAQH/BAIwADAWBgNVHSUBAf8EDDAKBggrBgEFBQcD
# CDAOBgNVHQ8BAf8EBAMCB4AwDQYJKoZIhvcNAQELBQADggIBAF/I8U6hbZhvDcn9
# 6nZ6tkbSEjXPvKZ6wroaXcgstEhpgaeEwleLuPXHLzEWtuJuYz4eshmhXqFr49lb
# AcX5SN5/cEsP0xdFayb7U5P94JZd3HjFvpWRNoNBhF3SDM0A38sI2H+hjhB/VfX1
# XcZiei1ROPAyCHcBgHLyQrEu6mnb3HhbIdr8h0Ta7WFylGhLSFW6wmzKusP6aOlm
# nGSac5NMfla6lRvTYHd28rbbCgfSm1RhTgoZj+W8DTKtiEMwubHJ3mIPKmo8xtJI
# WXPnXq6XKgldrL5cynLMX/0WX65OuWbHV5GTELdfWvGV3DaZrHPUQ/UP31Keqb2x
# jVCb30LVwgbjIvYS77N1dARkN8F/9pJ1gO4IvZWMwyMlKKFGojO1f1wbjSWcA/57
# tsc+t2blrMWgSNHgzDr01jbPSupRjy3Ht9ZZs4xN02eiX3eG297NrtC6l4c/gzn2
# 0eqoqWx/uHWxmTgB0F5osBuTHOe77DyEA0uhArGlgKP91jghgt/OVHoH65g0QqCt
# gZ+36mnCEg6IOhFoFrCc0fJFGVmb1+17gEe+HRMM7jBk4O06J+IooFrI3e3PJjPr
# Qano/MyE3h+zAuBWGMDRcUlNKCDU7dGnWvH3XWwLrCCIcz+3GwRUMsLsDdPW2OVv
# 7v1eEJiMSIZ2P+M7L20Q8aznU4OAMIIHcTCCBVmgAwIBAgITMwAAABXF52ueAptJ
# mQAAAAAAFTANBgkqhkiG9w0BAQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNVBAgT
# Cldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29m
# dCBDb3Jwb3JhdGlvbjEyMDAGA1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNh
# dGUgQXV0aG9yaXR5IDIwMTAwHhcNMjEwOTMwMTgyMjI1WhcNMzAwOTMwMTgzMjI1
# WjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMH
# UmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQD
# Ex1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDCCAiIwDQYJKoZIhvcNAQEB
# BQADggIPADCCAgoCggIBAOThpkzntHIhC3miy9ckeb0O1YLT/e6cBwfSqWxOdcjK
# NVf2AX9sSuDivbk+F2Az/1xPx2b3lVNxWuJ+Slr+uDZnhUYjDLWNE893MsAQGOhg
# fWpSg0S3po5GawcU88V29YZQ3MFEyHFcUTE3oAo4bo3t1w/YJlN8OWECesSq/XJp
# rx2rrPY2vjUmZNqYO7oaezOtgFt+jBAcnVL+tuhiJdxqD89d9P6OU8/W7IVWTe/d
# vI2k45GPsjksUZzpcGkNyjYtcI4xyDUoveO0hyTD4MmPfrVUj9z6BVWYbWg7mka9
# 7aSueik3rMvrg0XnRm7KMtXAhjBcTyziYrLNueKNiOSWrAFKu75xqRdbZ2De+JKR
# Hh09/SDPc31BmkZ1zcRfNN0Sidb9pSB9fvzZnkXftnIv231fgLrbqn427DZM9itu
# qBJR6L8FA6PRc6ZNN3SUHDSCD/AQ8rdHGO2n6Jl8P0zbr17C89XYcz1DTsEzOUyO
# ArxCaC4Q6oRRRuLRvWoYWmEBc8pnol7XKHYC4jMYctenIPDC+hIK12NvDMk2ZItb
# oKaDIV1fMHSRlJTYuVD5C4lh8zYGNRiER9vcG9H9stQcxWv2XFJRXRLbJbqvUAV6
# bMURHXLvjflSxIUXk8A8FdsaN8cIFRg/eKtFtvUeh17aj54WcmnGrnu3tz5q4i6t
# AgMBAAGjggHdMIIB2TASBgkrBgEEAYI3FQEEBQIDAQABMCMGCSsGAQQBgjcVAgQW
# BBQqp1L+ZMSavoKRPEY1Kc8Q/y8E7jAdBgNVHQ4EFgQUn6cVXQBeYl2D9OXSZacb
# UzUZ6XIwXAYDVR0gBFUwUzBRBgwrBgEEAYI3TIN9AQEwQTA/BggrBgEFBQcCARYz
# aHR0cDovL3d3dy5taWNyb3NvZnQuY29tL3BraW9wcy9Eb2NzL1JlcG9zaXRvcnku
# aHRtMBMGA1UdJQQMMAoGCCsGAQUFBwMIMBkGCSsGAQQBgjcUAgQMHgoAUwB1AGIA
# QwBBMAsGA1UdDwQEAwIBhjAPBgNVHRMBAf8EBTADAQH/MB8GA1UdIwQYMBaAFNX2
# VsuP6KJcYmjRPZSQW9fOmhjEMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6Ly9jcmwu
# bWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY1Jvb0NlckF1dF8yMDEw
# LTA2LTIzLmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0dHA6Ly93
# d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljUm9vQ2VyQXV0XzIwMTAtMDYt
# MjMuY3J0MA0GCSqGSIb3DQEBCwUAA4ICAQCdVX38Kq3hLB9nATEkW+Geckv8qW/q
# XBS2Pk5HZHixBpOXPTEztTnXwnE2P9pkbHzQdTltuw8x5MKP+2zRoZQYIu7pZmc6
# U03dmLq2HnjYNi6cqYJWAAOwBb6J6Gngugnue99qb74py27YP0h1AdkY3m2CDPVt
# I1TkeFN1JFe53Z/zjj3G82jfZfakVqr3lbYoVSfQJL1AoL8ZthISEV09J+BAljis
# 9/kpicO8F7BUhUKz/AyeixmJ5/ALaoHCgRlCGVJ1ijbCHcNhcy4sa3tuPywJeBTp
# kbKpW99Jo3QMvOyRgNI95ko+ZjtPu4b6MhrZlvSP9pEB9s7GdP32THJvEKt1MMU0
# sHrYUP4KWN1APMdUbZ1jdEgssU5HLcEUBHG/ZPkkvnNtyo4JvbMBV0lUZNlz138e
# W0QBjloZkWsNn6Qo3GcZKCS6OEuabvshVGtqRRFHqfG3rsjoiV5PndLQTHa1V1QJ
# sWkBRH58oWFsc/4Ku+xBZj1p/cvBQUl+fpO+y/g75LcVv7TOPqUxUYS8vwLBgqJ7
# Fx0ViY1w/ue10CgaiQuPNtq6TPmb/wrpNPgkNWcr4A245oyZ1uEi6vAnQj0llOZ0
# dFtq0Z4+7X6gMTN9vMvpe784cETRkPHIqzqKOghif9lwY1NNje6CbaUFEMFxBmoQ
# tB1VM1izoXBm8qGCAtQwggI9AgEBMIIBAKGB2KSB1TCB0jELMAkGA1UEBhMCVVMx
# EzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoT
# FU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEtMCsGA1UECxMkTWljcm9zb2Z0IElyZWxh
# bmQgT3BlcmF0aW9ucyBMaW1pdGVkMSYwJAYDVQQLEx1UaGFsZXMgVFNTIEVTTjpE
# MDgyLTRCRkQtRUVCQTElMCMGA1UEAxMcTWljcm9zb2Z0IFRpbWUtU3RhbXAgU2Vy
# dmljZaIjCgEBMAcGBSsOAwIaAxUAdqNHe113gCJ87aZIGa5QBUqIwvKggYMwgYCk
# fjB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMH
# UmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQD
# Ex1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAxMDANBgkqhkiG9w0BAQUFAAIF
# AOfDdSIwIhgPMjAyMzAzMjEwODEzMjJaGA8yMDIzMDMyMjA4MTMyMlowdDA6Bgor
# BgEEAYRZCgQBMSwwKjAKAgUA58N1IgIBADAHAgEAAgIc0jAHAgEAAgIR1DAKAgUA
# 58TGogIBADA2BgorBgEEAYRZCgQCMSgwJjAMBgorBgEEAYRZCgMCoAowCAIBAAID
# B6EgoQowCAIBAAIDAYagMA0GCSqGSIb3DQEBBQUAA4GBAFprVvSmnQB5xr+4Z1lo
# 9uEJowrQMbp8q5wDlN/59o9igIFSx9WOkQu1YDcsKB1M+El+OGLXFqnFbfNsovta
# 6VslMX68W+Ya0OvhwZKGEp1JkZE21BncqjDua3g1jWtBny+sJEAlK0k1q4UqYmtD
# iUNgtvE6ShyIeo9VzA0qBK4TMYIEDTCCBAkCAQEwgZMwfDELMAkGA1UEBhMCVVMx
# EzARBgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoT
# FU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUt
# U3RhbXAgUENBIDIwMTACEzMAAAG6Hz8Z98F1vXwAAQAAAbowDQYJYIZIAWUDBAIB
# BQCgggFKMBoGCSqGSIb3DQEJAzENBgsqhkiG9w0BCRABBDAvBgkqhkiG9w0BCQQx
# IgQgTuKU2ImWKux3ljpWqkzkaJK1xHa3idh6li9qw13GErYwgfoGCyqGSIb3DQEJ
# EAIvMYHqMIHnMIHkMIG9BCApVb08M25w+tYGWsmlGtp1gy1nPcqWfqgMF3nlWYVz
# BTCBmDCBgKR+MHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAw
# DgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24x
# JjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwAhMzAAABuh8/
# GffBdb18AAEAAAG6MCIEINcvz5sF7o3aicr36DbprjWwwvFtEk41w1E4u+taGSY0
# MA0GCSqGSIb3DQEBCwUABIICAF1QQ6KZ0pxVmntA5DGnyEbcf0KiExZAMbK4tSAZ
# dUvQptKP0Q3YPIQuGlqdp8mCSlWrYUvqr+kFSJzDElbT2cukke15S7CQjVvuMqoA
# eFKBObrKaJmGaX2NXd2EPGnU8YLz8kComzB1auQAjvppUvGT0UsZWOmljBGcTH1n
# kRKgTqT8p02uREIGntOk0bIAYss2l3d8L/4ex5AbzUe8r1nQil5JV1E8vfaVUjH1
# CZG9s2BtKuYZYdQX2pMIK0/Enq7Q1uLHXi+ON3JBAWghO4jAGm1RF5NCrYvD4Kqq
# q0OVjBmbaGFpqDKLXYtZxg3kezje4EfozLb5QDI5ep3NV+q+xqE0L+WqXXVmh3xX
# wEGmQIlD34G76wRPWqt34rrncI5GRxwYniAFXPrh51Ux1VmytQ8j/I33S4KGUxib
# fUE8ddQabBPS8a4X0LpWuQEY7wlDtoqJm9j7g4BLXnqKYMY8yKugkgO4UT04b2Ln
# WsOCz2bd7b53IHX9BzivhIWV6ZNqDUzlBJWHvU1uUIifaB/1/u+oAqd4eDUj8OO0
# OIsCuD3NuA1LWzzdig7MA5VRlb1K9XcTw7PWGzCEpnzLGuPCtea1Qh8cCFVkq7ol
# KQsANZWyhRe+lAERp9UO+c5x52kw2LA0L0deQDYYaHyF0sLsDmrucnIAwREBqq2s
# XZ/t
# SIG # End signature block
