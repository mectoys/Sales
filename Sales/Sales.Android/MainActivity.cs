


namespace Sales.Droid
{
    using Android.App;
    using Android.Content.PM;
    using Android.OS;
    using Android.Runtime;
    using ImageCircle.Forms.Plugin.Droid;
    using Plugin.CurrentActivity;
    using Plugin.Permissions;

    [Activity(Label = "Sales",
            Icon = "@drawable/ic_launcher",
            Theme = "@style/MainTheme", 
            MainLauncher = false, 
            ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            //paso 2 para tomar foto
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            //INICIALIZMOS el renderizador de imagenes
            ImageCircleRenderer.Init();
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        //paso 1 para tomar foto
        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(
                requestCode,
                permissions,
                grantResults);
        }
    }
}