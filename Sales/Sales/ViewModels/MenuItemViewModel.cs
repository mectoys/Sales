

using GalaSoft.MvvmLight.Command;
using Sales.Helpers;
using Sales.Views;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sales.ViewModels
{
   public class MenuItemViewModel
    {
        #region Properties

        public string Icon { get; set; }

        public string Title { get; set; }

        public string PageName { get; set; }

        #endregion


        #region Commands
        public ICommand GotoCommand
        {
            get
            {
                return new RelayCommand(Goto);

            }
        }

        private void Goto()
        {
            if (this.PageName=="LoginPage")
            {
                //con esto limpiamos los accesos al token 
                Settings.AccessToken = string.Empty;
                Settings.TokkenType = string.Empty;
                Settings.IsRemembered = false;
                //isntanciamos la loginview model
                MainViewModel.GetInstance().Login = new LoginViewModel();
                //llamamos la pagina de login
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }


        #endregion
    }

}
