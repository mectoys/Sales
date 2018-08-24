

namespace Sales.Interfaces
{
    using System.Globalization;

   public interface ILocalize
    {
        //obtiene el idioma del celular
        CultureInfo GetCurrentCultureInfo();
        //obtiene el formato del idioma
        void SetLocale(CultureInfo ci);

    }
}
