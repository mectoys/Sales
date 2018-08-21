


namespace Sales.ViewModels
{
    using Sales.Common.Models;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Xamarin.Forms;

    public class ProductsViewModel:BaseViewModel
    {
        private ApiService apiService;
        //crear elatributo
        private ObservableCollection<Product> products;

        public ObservableCollection<Product> Products
        {
            get { return this.products; }
            set { this.SetValue(ref this.products, value); }
        }
        public ProductsViewModel()
        {
            this.apiService = new ApiService();
            this.LoadProducts();
        }

        private async void LoadProducts()
        {
            var response = await this.apiService.GetList<Product>("https://salesapimectoys.azurewebsites.net", "/api", "/Products");
            //aca devolvio una lista de obj. response
          if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error",response.Message,"Acept");
                return;
            }
            var list = (List<Product>)response.Result;
            //armamos la obserbable collections
            this.Products = new ObservableCollection<Product>(list);
        }
        //Cargar la lista de productos
    }
}
