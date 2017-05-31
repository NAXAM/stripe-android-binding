using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace Demo
{
  
    public class MainActivity : Android.App.Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Intent intent = new Intent(this, typeof(LauncherActivity));
            StartActivity(intent);
            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }
    }
}

