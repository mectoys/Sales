

namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Sales.Common.Models;
    using Sales.Helpers;
    using System.Collections.ObjectModel;
    using Views;

    public  class MainViewModel
    {
        #region Properties
        public LoginViewModel Login { get; set; }

        public EditProductViewModel EditProduct { get; set; }

        public ProductsViewModel Products { get; set; }

        public AddProductViewModel AddProduct { get; set; }

        public RegisterViewModel Register { get; set; }

        public MyUserASP  UserASP { get; set; }

        public ObservableCollection<MenuItemViewModel> Menu { get; set; }

        public string UserFullName
        {
            get
            {
                if (this.UserASP != null && this.UserASP.Claims != null && this.UserASP.Claims.Count > 1)
                {
                    return $"{this.UserASP.Claims[0].ClaimValue} {this.UserASP.Claims[1].ClaimValue}";
                }

                return null;
            }
        }

        //public string UserImageFullPath
        //{
        //    get
        //    {
        //        if (this.UserASP != null && this.UserASP.Claims != null && this.UserASP.Claims.Count > 3)
        //        {
        //            return $"https://salesapimectoys.azurewebsites.net{this.UserASP.Claims[3].ClaimValue.Substring(1)}";
        //        }

        //        return null;
        //    }
        //}

        public string UserImageFullPath
        {
            get
            {
                foreach (var claim in this.UserASP.Claims)
                {
                    if (claim.ClaimType == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/uri")
                    {
                        if (claim.ClaimValue.StartsWith("~"))
                        {
                            return $"https://salesapiservices.azurewebsites.net{claim.ClaimValue.Substring(1)}";
                        }

                        return claim.ClaimValue;
                    }
                }

                return null;
            }
        }

        #endregion

        #region Sigleton
        private static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }
            return instance;
        }

        #endregion

        #region Constructor
        public MainViewModel()
        {
            instance = this;
            // this.Products = new ProductsViewModel();
            this.LoadMenu();
        }

        #region Methods
        private void LoadMenu()
        {
            this.Menu = new ObservableCollection<MenuItemViewModel>();

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_info",
                PageName = "AboutPage",
                Title = Languages.About,
            });

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_phonelink_setup",
                PageName = "SetupPage",
                Title = Languages.Setup,
            });

            this.Menu.Add(new MenuItemViewModel
            {
                Icon = "ic_exit_to_app",
                PageName = "LoginPage",
                Title = Languages.Exit,
            });
        }

        #endregion
        #endregion

        #region Commands

        public System.Windows.Input.ICommand AddProductCommand

        {
            get
            {
                return new RelayCommand(GoToAddProduct);

            }
        }
        private async void GoToAddProduct()
        {
            this.AddProduct = new AddProductViewModel();
            //  await Application.Current.MainPage.Navigation.PushAsync(new AddProductPage());
            await App.Navigator.PushAsync(new AddProductPage());
        } 
        #endregion
    }
}
