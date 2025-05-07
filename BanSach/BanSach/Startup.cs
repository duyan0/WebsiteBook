using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;

[assembly: OwinStartup(typeof(BanSach.Startup))]
namespace BanSach
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            System.Diagnostics.Debug.WriteLine("OWIN Startup Configuration called");

            // Cấu hình xác thực cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/dang-nhap")
            });

            // Cấu hình xác thực Google
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "", // Thay bằng Client ID từ Google Console
                ClientSecret = "", // Thay bằng Client Secret
                CallbackPath = new PathString("/signin-google")
            });
        }
    }
}