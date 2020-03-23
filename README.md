# Xamarin-Azure-Active-Directory-Auth
Authenticating Users by Azure Active Directory
# Code Explanation
## Generic Project
1. **App.cs** 
  - Variables
```csharp
        public static IPublicClientApplication PCA = null;

        public static string ClientID = "CLIENT-ID-GOES-HERE"; //msidentity-samples-testing tenant

        public static string[] Scopes = { "User.Read" };
        public static string Username = string.Empty;

        public static object ParentWindow { get; set; }
```
   - Constructor
```csharp
      public App()
        {
            PCA = PublicClientApplicationBuilder.Create(ClientID)
                .WithRedirectUri($"msal{ClientID}://auth")
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                .Build();

            MainPage = new NavigationPage(new MainPage());
        }
```
2. **MainPage.xaml** 
  - Page content
```xml
        <StackLayout>
        <Label Text="MSAL Xamarin Forms Sample" VerticalOptions="Start" HorizontalTextAlignment="Center" HorizontalOptions="FillAndExpand" />
        <BoxView Color="Transparent" VerticalOptions="FillAndExpand" HorizontalOptions="FillAndExpand" />
        <StackLayout x:Name="slUser" IsVisible="False" Padding="5,10">
            <StackLayout Orientation="Horizontal">
                <Label Text="DisplayName " FontAttributes="Bold" />
                <Label x:Name="lblDisplayName" />
            </StackLayout>
            <StackLayout Orientation="Horizontal">
                <Label Text="GivenName " FontAttributes="Bold" />
                <Label x:Name="lblGivenName" />
            </StackLayout>
            <StackLayout Orientation="Horizontal">
                <Label Text="Surname " FontAttributes="Bold" />
                <Label x:Name="lblSurname" />
            </StackLayout>
            <StackLayout Orientation="Horizontal">
                <Label Text="Id " FontAttributes="Bold" />
                <Label x:Name="lblId" />
            </StackLayout>
            <StackLayout Orientation="Horizontal">
                <Label Text="UserPrincipalName " FontAttributes="Bold" />
                <Label x:Name="lblUserPrincipalName" />
            </StackLayout>
        </StackLayout>
        <BoxView Color="Transparent" VerticalOptions="FillAndExpand" HorizontalOptions="FillAndExpand" />
        <Button x:Name="btnSignInSignOut" Text="Sign in" Clicked="OnSignInSignOut" VerticalOptions="End" HorizontalOptions="FillAndExpand"/>
    </StackLayout>
```
3. **MainPage.xaml.cs** 
  - Constructor
```csharp
        [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
```
- OnSignInSignOut Method
```csharp
        async void OnSignInSignOut(object sender, EventArgs e)
        {
            AuthenticationResult authResult = null;
            IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync();
            try
            {
                if (btnSignInSignOut.Text == "Sign in")
                {
                    try
                    {
                        IAccount firstAccount = accounts.FirstOrDefault();
                        authResult = await App.PCA.AcquireTokenSilent(App.Scopes, firstAccount)
                                              .ExecuteAsync();
                    }
                    catch (MsalUiRequiredException ex)
                    {
                        try
                        {
                            authResult = await App.PCA.AcquireTokenInteractive(App.Scopes)
                                                      .WithParentActivityOrWindow(App.ParentWindow)
                                                      .ExecuteAsync();
                        }
                        catch (Exception ex2)
                        {
                            await DisplayAlert("Acquire token interactive failed. See exception message for details: ", ex2.Message, "Dismiss");
                        }
                    }

                    if (authResult != null)
                    {
                        var content = await GetHttpContentWithTokenAsync(authResult.AccessToken);
                        UpdateUserContent(content);
                        Device.BeginInvokeOnMainThread(() => { btnSignInSignOut.Text = "Sign out"; });
                    }
                }
                else
                {
                    while (accounts.Any())
                    {
                        await App.PCA.RemoveAsync(accounts.FirstOrDefault());
                        accounts = await App.PCA.GetAccountsAsync();
                    }

                    slUser.IsVisible = false;
                    Device.BeginInvokeOnMainThread(() => { btnSignInSignOut.Text = "Sign in"; });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Authentication failed. See exception message for details: ", ex.Message, "Dismiss");
            }
        }
```
- UpdateUserContent Method
```csharp
        private void UpdateUserContent(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                JObject user = JObject.Parse(content);

                slUser.IsVisible = true;

                Device.BeginInvokeOnMainThread(() =>
                {
                    lblDisplayName.Text = user["displayName"].ToString();
                    lblGivenName.Text = user["givenName"].ToString();
                    lblId.Text = user["id"].ToString();
                    lblSurname.Text = user["surname"].ToString();
                    lblUserPrincipalName.Text = user["userPrincipalName"].ToString();

                    btnSignInSignOut.Text = "Sign out";
                });
            }
        }
```
- GetHttpContentWithTokenAsync
```csharp
        public async Task<string> GetHttpContentWithTokenAsync(string token)
        {
            try
            {
                //get data from API
                HttpClient client = new HttpClient();
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
            catch (Exception ex)
            {
                await DisplayAlert("API call to graph failed: ", ex.Message, "Dismiss");
                return ex.ToString();
            }
        }
```
## Android Project
1. **AndroidManifest.xml**
```xml
        <application android:label="UserDetailsClient.Droid">
    <activity android:name="microsoft.identity.client.BrowserTabActivity"
             android:configChanges="orientation|screenSize">
      <intent-filter>
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data android:scheme="msalCLIENT-ID-GOES-HERE" android:host="auth" />
      </intent-filter>
    </activity>
  </application>
```
2. **MainActivity**
   - at the end of the OnCreate method
```csharp
        App.ParentWindow = this;
```
   - OnActivityResult method
```csharp
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
```
> The following classes should be created to overrite default behaviour
3. **MainPageRenderer**
```
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MainPage), typeof(MainPageRenderer))]
namespace TestAAD.Droid
{
    class MainPageRenderer : PageRenderer
    {
        public MainPageRenderer(Context context) : base(context)
        {

        }
        MainPage page;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as MainPage;
            var activity = this.Context as Activity;
        }

    }
}
```
4. **MsalActivity**
```
using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace TestAAD.Droid
{
    [Activity]
    [IntentFilter(new[] { Intent.ActionView },
           Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
           DataHost = "auth",
           DataScheme = "msalCLIENT-ID-GOES-HERE")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}
```
