using Microsoft.Identity.Client;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestAAD
{
    public partial class App : Application
    {
        public static IPublicClientApplication PCA = null;

        public static string ClientID = "client-id-goes-here"; //msidentity-samples-testing tenant

        public static string[] Scopes = { "User.Read" };
        public static string Username = string.Empty;

        public static object ParentWindow { get; set; }

        public App()
        {
            PCA = PublicClientApplicationBuilder.Create(ClientID)
                .WithRedirectUri($"msal{ClientID}://auth")
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                .Build();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
