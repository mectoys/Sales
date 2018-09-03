
namespace Sales.ViewModels
{
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Sales.Common.Models;
    using Services;
    using Xamarin.Forms;

    public class AddProductViewModel : BaseViewModel
    {
        //todas las viewmodel heredan de BaseViewModel
        #region Attributes,
        private ApiService apiService;

        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Properties
        public string Description { get; set; }
        public string Price { get; set; }
        public string Remarks { get; set; }
      

        public bool IsRunning
        {
            get { return this.isRunning; }
            set { this.SetValue(ref this.isRunning, value); }
        }

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set { this.SetValue(ref this.isEnabled, value); }
        }
        #endregion

        #region Constructors
        public AddProductViewModel()
        {
            this.apiService = new ApiService();
            this.IsEnabled = true;
        }
        #endregion

        #region Commands
        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Description))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.DescriptionError,
                    Languages.Accept);
                return;
            }
            if (string.IsNullOrEmpty(this.Price))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PriceError,
                    Languages.Accept);
                return;
            }

            var price = decimal.Parse(this.Price);

            if (price<0)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PriceError,
                    Languages.Accept);
                return;
            }
            //activar el activity indicator y apagar el boton por los chequeos  conexion
            this.isRunning = true;
            this.isEnabled = false;
            //chequea conexion
            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.isRunning = false;
                this.isEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);
                return;
            }
            //ARMAMOS EL OBJETOS con los valores que ingreso el usuario en el formulario
            var product = new Product
            {
                Description=this.Description,
                Price=price,
                Remarks=this.Remarks,
            };

            //consumir el API
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            //NOTA el VS simplifica el nombre Post y no se coloca en la notacion diamante Product
            //pues ya esta dentro de los parametros.
            var response = await this.apiService.Post(url, prefix, controller,product);
            //preguntar si lo grabo
            if (!response.IsSuccess)
            {

                this.isRunning = false;
                this.isEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }
            //casteamos de la clase products
            var newProduct = (Product)response.Result;
            //adicionarle a ese producto a la obserbable collections
            //llamado el patron SIGLETON
            var viewModel = ProductsViewModel.GetInstance();
            //VIEWMODEL tiene la coleccion de productos y se debe adicionar
            viewModel.Products.Add(newProduct);
            
            //devolvernos al listado  por codigo un back 
            this.isRunning = false;
            this.isEnabled = true;
            //hacer un desapilamiento entre las PAGE (para regresar a listado de productos PAGE)
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        #endregion
    }

   
    }
