using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CustomerCheckIn.Services;
using CustomerCheckIn.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CustomerCheckIn
{
    public partial class App : Application
    {
        //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
        public static string AzureBackendUrl = "http://10.10.210.9:45455";
        public static bool UseMockDataStore = false;
        public App()        {
            InitializeComponent();

            if (UseMockDataStore)
                DependencyService.Register<MockDataStore>();
            else
                DependencyService.Register<AzureDataStore>();

            MainPage = new LookUp();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
