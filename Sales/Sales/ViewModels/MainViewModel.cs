

namespace Sales.ViewModels
{


    using GalaSoft.MvvmLight.Command;
    using Sales.Views;
    using System;
    using Xamarin.Forms;

    public  class MainViewModel
    {

        public ProductsViewModel Products { get; set; }

        public MainViewModel()
        {
            this.Products = new ProductsViewModel();
        }
        public System.Windows.Input.ICommand AddProductCommand

        { get
            {
                return new RelayCommand(GoToAddProduct);

            }


        }

        private async void GoToAddProduct()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new AddProductPage());
        }
    }
}
