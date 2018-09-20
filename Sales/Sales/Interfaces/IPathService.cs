using System;
using System.Collections.Generic;
using System.Text;

namespace Sales.Interfaces
{
    //cuando queremos obtener la ruta tanto de android o en IOS se 
    //Debe de usar una interfaz
    public interface IPathService
    {
        string GetDatabasePath();
    }

}
