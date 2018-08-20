

namespace Sales.Infrastructure
{
    using Sales.ViewModels;

    //instancia la mainviewmodel
    public class InstanceLocator
    {
        public MainViewModel Main { get; set; }
        public InstanceLocator()
        {
            this.Main = new MainViewModel();
        }
    }
}
