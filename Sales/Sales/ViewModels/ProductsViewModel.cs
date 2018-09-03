


namespace Sales.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Sales.Common.Models;
    using Sales.Helpers;
    using Services;
  
    using Xamarin.Forms;

    public class ProductsViewModel:BaseViewModel
    {
        #region Attributes
        private ApiService apiService;
        private bool isRefreshing;

        #endregion

        #region Properties
        //crear elatributo
        private ObservableCollection<Product> products;
        //aca tiene la lsita de los productos
        public ObservableCollection<Product> Products
        {
            get { return this.products; }
            set { this.SetValue(ref this.products, value); }
        } 
     

        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { this.SetValue(ref this.isRefreshing, value); }
        }
        #endregion

        #region Construct
        public ProductsViewModel()
        {
            //PASO 1 SIGLETON
            //la primera vez que lo llamamos debemos instanciar para que quede en memoria por ser STATIC
            instance = this;
            this.apiService = new ApiService();
            this.LoadProducts();
        }
        #endregion

        #region Sigleton
        private static ProductsViewModel instance;

        public static ProductsViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ProductsViewModel();
            }
            return instance;
        }


        #endregion

        #region Methods

        private async void LoadProducts()
        {
            this.IsRefreshing = true;
            //revisa si hay o no conexion a internet
            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }
            //agregar al diccionarion de recursos ap.xaml la direccion URL por seguridad
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.GetList<Product>(url, prefix, controller);
            //aca devolvio una lista de obj. response
            if (!response.IsSuccess)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }
            var list = (List<Product>)response.Result;
            //armamos la obserbable collections
            this.Products = new ObservableCollection<Product>(list);
            this.IsRefreshing = false;
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadProducts);
            }

        } 
        #endregion

        //Cargar la lista de productos


    }
}
