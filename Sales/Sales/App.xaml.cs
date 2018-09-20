using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Sales
{
    using Views;
    using ViewModels;
    using Sales.Helpers;

    public partial class App : Application
    {
        //internal set se usa para propiedad interna
        public static NavigationPage Navigator { get; internal set; }

        public App()
        {
            InitializeComponent();

            //preguntar si esta logueado 
            //si el usuario es recordado y el access token no esta vacio
            if (Settings.IsRemembered && !string.IsNullOrEmpty(Settings.AccessToken))
            {
                MainViewModel.GetInstance().Products = new ProductsViewModel();
                MainPage = new  MasterPage();
            }
            else
            {
              //antes de mostrar el apgo ahcemos una instancia de la mainviewmodel.
                MainViewModel.GetInstance().Login = new LoginViewModel();

                MainPage = new NavigationPage(new LoginPage()); //NavigationPage(new ProductsPage());
            }
          
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
