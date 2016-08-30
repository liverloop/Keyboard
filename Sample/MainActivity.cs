using Android.App;
using Android.Widget;
using Android.OS;
using Sino.Keyboard.Droid.Views;

namespace Sample
{
    [Activity(Label = "Sample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        TextView tv;
        Button btn;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            tv = FindViewById<TextView>(Resource.Id.textView1);
            btn = FindViewById<Button>(Resource.Id.MyButton);

            btn.Click += (s, e) =>
            {
                VehiclePlateKeyboard keyboard = new VehiclePlateKeyboard(this);
                keyboard.InputFinish += (number) =>
                {
                    tv.Text = number;
                };

                keyboard.InputProcess += (number) => 
                {
                    tv.Text = "Processing: " + number;
                };

                keyboard.SetDefaultPlateNumber("京A00000");
                keyboard.Show(Window.DecorView.RootView);
            };
        }
    }
}

