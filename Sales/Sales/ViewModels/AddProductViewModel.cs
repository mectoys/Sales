
namespace Sales.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Plugin.Media;
    using Plugin.Media.Abstractions;
    using Sales.Common.Models;
    using Services;
    using Xamarin.Forms;

    public class AddProductViewModel : BaseViewModel
    {
        //todas las viewmodel heredan de BaseViewModel
        #region Attributes
        //este atributo es importante para las fotos pues cuando es tomada se almacena en la variable file
        private MediaFile file;

        //TIPO DE DATO ESPECIAL para definir el origen de la imagen url, arreglo de bites, etc
        private ImageSource imageSource;

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

        public ImageSource ImageSource
        {
            get { return this.imageSource; }
            set { this.SetValue(ref this.imageSource, value); }
        }
        #endregion

        #region Constructors
        public AddProductViewModel()
        {
            //inicializamos los valores
            this.apiService = new ApiService();
            this.IsEnabled = true;
            //por default tendra la cajita de imagen predeterminada .
            this.ImageSource = "noproduct";
        }
        #endregion

        #region Commands
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
            }

            //ARMAMOS EL OBJETOS con los valores que ingreso el usuario en el formulario
            var product = new Product
            {
                Description=this.Description,
                Price=price,
                Remarks=this.Remarks,
                //convierte la imagen a un string  base 64
                ImageArray=imageArray,
            };

            //consumir el API
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();
            //NOTA el VS simplifica el nombre Post y no se coloca en la notacion diamante Product
            //pues ya esta dentro de los parametros.
            var response = await this.apiService.Post(url, prefix, controller,product, Settings.TokkenType, Settings.AccessToken);
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
            productsviewModel.MyProducts.Add(newProduct);
            //invocamos el refresco para la lista ordenada
            productsviewModel.RefreshList();
            //VIEWMODEL tiene la coleccion de productos y se debe adicionar
           
            
            //devolvernos al listado  por codigo un back 
            this.IsRunning = false;
            this.IsEnabled = true;
            //hacer un desapilamiento entre las PAGE (para regresar a listado de productos PAGE)
            // await Application.Current.MainPage.Navigation.PopAsync();
            await App.Navigator.PopAsync();
        }
        #endregion
    }

   
    }
