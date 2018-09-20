



namespace Sales.ViewModels
{

    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Sales.Helpers;
    using Sales.Services;
    using System;
    using System.Linq;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class EditProductViewModel:BaseViewModel
    {
        #region Attributes
        private Product product;

        private MediaFile file;

        private ImageSource imageSource;

        private ApiService apiService;

        private bool isRunning;

        private bool isEnabled;

        #endregion

        #region Properties

        public Product Product
        {
            get { return this.product; }
            set { this.SetValue(ref this.product, value); }

        }

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

        public ImageSource ImageSource
        {
            get { return this.imageSource; }
            set { this.SetValue(ref this.imageSource, value); }
        }

        #endregion


        #region Constructors
        public EditProductViewModel(Product product)
        {
            this.product = product;
           
            this.apiService = new ApiService();
            this.IsEnabled = true;
           
            this.ImageSource = product.ImageFullPath;
        }
        #endregion

        #region Commands

        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand(Delete);
            }
        }

        private async void Delete()
        {

            var answer = await Application.Current.MainPage.DisplayAlert(
             Languages.Confirm,
             Languages.DeleteConfirmation,
             Languages.Yes, Languages.No);
            if (!answer)
            {
                return;

            }
            this.IsRunning = true;
            this.IsEnabled = false;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }
            //agregar al diccionarion de recursos ap.xaml la direccion URL por seguridad
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.Delete(url, prefix, controller, this.Product.ProductId, Settings.TokkenType, Settings.AccessToken);
            //aca devolvio una lista de obj. response
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }
            //refrescar la lista
            //llamar el singleton
            var productsViewModel = ProductsViewModel.GetInstance();
            var deletedProduct = productsViewModel.MyProducts.Where(p => p.ProductId == this.Product.ProductId).FirstOrDefault();
            if (deletedProduct != null)
            {
                productsViewModel.MyProducts.Remove(deletedProduct);
            }
            productsViewModel.RefreshList();
            this.IsRunning = false;
            this.IsEnabled = true;
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public ICommand ChangeImageCommand
        {
            get
            {
                return new RelayCommand(ChangeImage);
            }
        }
        //metodo
        private async void ChangeImage()
        {
            //inicializar
            await CrossMedia.Current.Initialize();
            //mensaje con opciones
            var source = await Application.Current.MainPage.DisplayActionSheet(
                Languages.ImageSource,
                Languages.Cancel,
                null,
                //lista de opciones
                Languages.FromGallery,
                Languages.NewPicture);

            if (source == Languages.Cancel)
            {
                this.file = null;
                return;
            }

            if (source == Languages.NewPicture)
            {
                this.file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                //si no desea tomar una foto y seleccionar de la galeria
                this.file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (this.file != null)
            {
                //cargar el stream 
                this.ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = this.file.GetStream();
                    return stream;
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Product.Description))
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.DescriptionError,
                    Languages.Accept);
                return;
            }            

            if (this.product.Price < 0)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PriceError,
                    Languages.Accept);
                return;
            }
            //activar el activity indicator y apagar el boton por los chequeos  conexion
            this.IsRunning = true;
            this.IsEnabled = false;
            //chequea conexion
            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);
                return;
            }
            //SI EL USUARIO SELECCIONO FOTO O LA TOMO 
            byte[] imageArray = null;
            if (this.file != null)
            {
                imageArray = FilesHelper.ReadFully(this.file.GetStream());
                //enviar la iamgen cambiada al campo producto imagen
                this.Product.ImageArray = imageArray;
            }

            //Ya no creamos el objeto products por ya viene y esta bindado.
 
            //consumir el API
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            //NOTA el VS simplifica el nombre Post y no se coloca en la notacion diamante Product
            //pues ya esta dentro de los parametros.
            var response = await this.apiService.Put(url, prefix, controller,this.Product,this.product.ProductId, Settings.TokkenType, Settings.AccessToken);
            //preguntar si lo grabo
            if (!response.IsSuccess)
            {
                //la propiedad
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }
            //casteamos de la clase products y nos devuelve el servicio
            var newProduct = (Product)response.Result;
            //adicionarle a ese producto a la obserbable collections
            //llamado el patron SIGLETON
            var productsviewModel = ProductsViewModel.GetInstance();
            //buscar el producto elimina el viejo y agrega el nuevo con linq
            var oldProduct = productsviewModel.MyProducts.Where(p => p.ProductId == this.Product.ProductId).FirstOrDefault();
            //cerciorarse que esta OK
            if (oldProduct!=null)
            {
                //removemos el viejo producto
                productsviewModel.MyProducts.Remove(oldProduct);
             
            }
            //agregamos el nuevo
            productsviewModel.MyProducts.Add(newProduct);
            //invocamos el refresco para la lista ordenada
            productsviewModel.RefreshList();
            //VIEWMODEL tiene la coleccion de productos y se debe adicionar


            //devolvernos al listado  por codigo un back 
            this.IsRunning = false;
            this.IsEnabled = true;
            //hacer un desapilamiento entre las PAGE (para regresar a listado de productos PAGE)
             await App.Navigator.PopAsync();
        }
        #endregion
    }
}
