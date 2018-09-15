

namespace Sales.ViewModels
{

    using System;
    using GalaSoft.MvvmLight.Command;
    using Sales.Views;   
    using Xamarin.Forms;

    public  class MainViewModel
    {
        #region Properties
        public LoginViewModel Login { get; set; }

        public EditProductViewModel EditProduct { get; set; }

        public ProductsViewModel Products { get; set; }

        public AddProductViewModel AddProduct { get; set; }
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
        }
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
            await Application.Current.MainPage.Navigation.PushAsync(new AddProductPage());
        } 
        #endregion
    }
}
