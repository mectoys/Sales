


namespace Sales.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Common.Models;
    using Helpers;
    using Services;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Xamarin.Forms;
    using System;
    using System.Threading.Tasks;

    public class ProductsViewModel:BaseViewModel
    {
        #region Attributes

        private string filter;

        private ApiService apiService;

        private DataService dataService;

        private bool isRefreshing;

        //crear elatributo
        private ObservableCollection<ProductItemViewModel> products;

        #endregion

        #region Properties

        public string Filter
        {
            get { return this.filter; }
            set
            {
                this.filter = value;
                this.RefreshList();
            }
        }

        //propiedad puvblica para uso en todo el proyecto los valores
        public List<Product>MyProducts { get; set; }

        //aca tiene la lsita de los productos
        public ObservableCollection<ProductItemViewModel> Products
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
            //Iniciar los servicios para SQLITE
            this.dataService = new DataService();
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
            if (connection.IsSuccess)
            {
                var answer = await this.LoadProductsFromAPI();
                if (answer)
                {
                    //guardar los productos en la sqlite
                    this.SaveProductsToDB();
                }
             }
            else
            {
                //Cargar productos del sqlite
                await this.LoadProductsFromDB();
            }
            //si el objeto myproduct es null o no tiene registros
            if (this.MyProducts==null || this.MyProducts.Count==0)
            {
                this.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.NoProductsMessage, Languages.Accept);
                return;
            }
            //2 refrescame la lista 
            this.RefreshList();
            this.IsRefreshing = false;
        }
           
        private async Task LoadProductsFromDB()
        {
            //trae los prod. de la BD
            this.MyProducts = await this.dataService.GetAllProducts();
        }

        private async Task  SaveProductsToDB()
        {
            //elimina
            await this.dataService.DeleteAllProducts();
            //inserta el modelo del atributo 
            this.dataService.Insert(this.MyProducts);

        }

        private async Task<bool> LoadProductsFromAPI()
        {
            //agregar al diccionarion de recursos ap.xaml la direccion URL por seguridad
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.GetList<Product>(url, prefix, controller, Settings.TokkenType, Settings.AccessToken);
            //aca devolvio una lista de obj. response
            if (!response.IsSuccess)
            {
               // this.IsRefreshing = false;
                //await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return false;
            }
            //creamos un objeto lambda
            //1 despues que te traiga la lista de productos
            this.MyProducts = (List<Product>)response.Result;
            return true;
        }

        public void RefreshList()
        {
              if (string.IsNullOrEmpty(this.Filter))
              {             
              
                var myListProductItemViewModel = this.MyProducts.Select(p => new ProductItemViewModel
                {
                    Description = p.Description,
                    ImageArray = p.ImageArray,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remarks = p.Remarks,

                });
                //armamos la obserbable collections y la ordenamos con LINQ
                this.Products = new ObservableCollection<ProductItemViewModel>(
                    myListProductItemViewModel.OrderBy(p => p.Description));

                }
            else
            {
                //expresion lambda video 34 para busqueda en el serach bar
                //el THIS te dice si es un atributo de clase o no VIDEO 34
                var myListProductItemViewModel = this.MyProducts.Select(p => new ProductItemViewModel
                {
                    Description = p.Description,
                    ImageArray = p.ImageArray,
                    ImagePath = p.ImagePath,
                    IsAvailable = p.IsAvailable,
                    Price = p.Price,
                    ProductId = p.ProductId,
                    PublishOn = p.PublishOn,
                    Remarks = p.Remarks,
                }).Where(p => p.Description.ToLower().Contains(this.Filter.ToLower())).ToList();

                //armamos la obserbable collections y la ordenamos con LINQ
                this.Products = new ObservableCollection<ProductItemViewModel>(
                    myListProductItemViewModel.OrderBy(p => p.Description));
            }
         
            

            /*
            var list = (List<Product>)response.Result;
            var myList = new List<ProductItemViewModel>();
            foreach (var item in list)
            {
                myList.Add(new ProductItemViewModel
                    {

                });
            }
            */

        }
        #endregion

        #region Commands
        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(RefreshList);
            }

        }

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
