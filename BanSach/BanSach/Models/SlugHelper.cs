using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
namespace BanSach.Models
{


    public static class SlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
                return string.Empty;

            // Chuyển chuỗi về dạng không dấu
            string str = RemoveDiacritics(phrase).ToLowerInvariant();
            // Thay khoảng trắng bằng dấu gạch nối
            str = Regex.Replace(str, @"\s+", "-");
            // Xóa các ký tự không phải chữ cái, số hoặc dấu gạch nối
            str = Regex.Replace(str, @"[^a-z0-9-]", "");
            // Xóa nhiều dấu gạch nối liên tiếp
            str = Regex.Replace(str, @"-+", "-");
            // Xóa dấu gạch nối ở đầu và cuối
            return str.Trim('-');
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}