using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

using Android.Gms.Vision;
using Android.Gms.Vision.Texts;
using Android.Util;
using Android.Graphics;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;
using static Android.Gms.Vision.Detector;
using System.Text;
using System.Collections.Generic;
using Android.Content.Res;
using Android.Media.TV;
using static Android.Views.TextureView;

namespace ARVision
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ISurfaceHolderCallback, IProcessor
    {
        private SurfaceView cameraView;
        private CameraSource cameraSource;
        private DrawingView drawingView;
        private const int RequestCameraPermissionID = 1001;

        private CheckBox checkBox;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            cameraView = FindViewById<SurfaceView>(Resource.Id.surface_view);
            drawingView = FindViewById<DrawingView>(Resource.Id.drawing_View);
            checkBox = FindViewById<CheckBox>(Resource.Id.checkbox);

            TextRecognizer txtRecognizer = new TextRecognizer.Builder(ApplicationContext).Build();
            if (!txtRecognizer.IsOperational)
            {
                Log.Error("Main Activity", "Detector dependencies are not yet available");
            }
            else
            {
                cameraSource = new CameraSource.Builder(ApplicationContext, txtRecognizer)
                .SetFacing(CameraFacing.Back)
                .SetRequestedPreviewSize(Resources.DisplayMetrics.WidthPixels, Resources.DisplayMetrics.HeightPixels)
                .SetRequestedFps(2.0f)
                .SetAutoFocusEnabled(true).Build();
                cameraView.Holder.AddCallback(this);
                txtRecognizer.SetProcessor(this);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }



        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height) { }
        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            {
                //Request permission  
                ActivityCompat.RequestPermissions(this, new string[] {
                    Android.Manifest.Permission.Camera
                }, RequestCameraPermissionID);
                return;
            }
            cameraSource.Start(cameraView.Holder);
            drawingView.Camera = cameraSource;
        }
        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            cameraSource.Stop();
        }
        public void ReceiveDetections(Detections detections)
        {
            SparseArray items = detections.DetectedItems;

            List<DrawItem> drItems = new List<DrawItem>();

            if (items.Size() != 0)
            {


                Paint backPaint = new Paint();
                backPaint.Color = Color.DarkGray;

                Paint textPaint = new Paint();
                textPaint.Color = Color.White;

                for (int i = 0; i < items.Size(); i++)
                {
                    TextBlock tb = (TextBlock)items.ValueAt(i);

                    foreach (IText line in tb.Components)
                    {
                        string temp = line.Value;

                        if (checkBox.Checked)
                        {
                            // Test for Base64
                            try
                            {
                                temp = Encoding.UTF8.GetString(Convert.FromBase64String(temp));
                            }
                            catch
                            {
                                temp = line.Value;
                            }
                        }


                        drItems.Add(new DrawItem()
                        {
                            Text = temp,
                            Top = line.BoundingBox.Top,
                            Left = line.BoundingBox.Left,
                            Bottom = line.BoundingBox.Bottom,
                            Right = line.BoundingBox.Right
                        });
                    }
                }

                drawingView.Items.Clear();
                drawingView.Items.AddRange(drItems);
                drawingView.Invalidate();
            }
            else
            {
                drawingView.Items.Clear();
                drawingView.Invalidate();
            }
            //List<DrawItem> drItems = new List<DrawItem>();

            //if (items.Size() != 0)
            //{
            //    for (int i = 0; i < items.Size(); ++i)
            //    {
            //        TextBlock tb = (TextBlock)items.ValueAt(i);
            //        drItems.Add(new DrawItem()
            //        {
            //            Text = tb.Value,
            //            Left = tb.BoundingBox.Left,
            //            Top = tb.BoundingBox.Top,
            //            Right = tb.BoundingBox.Right,
            //            Bottom = tb.BoundingBox.Bottom
            //        });
            //    }
            //}

            //if (drItems.Count > 0)
            //{
            //    drawView.Post(() =>
            //    {
            //        drawView.Items.Clear();
            //        drawView.Items.AddRange(drItems);
            //        drawView.PostInvalidate();
            //    });
            //}

            //if (items.Size() != 0)
            //{
            //    //txtView.Post(() => {
            //    //    StringBuilder strBuilder = new StringBuilder();
            //    //    for (int i = 0; i < items.Size(); ++i)
            //    //    {
            //    //        strBuilder.Append(((TextBlock)items.ValueAt(i)).Value);
            //    //        strBuilder.Append("\n");
            //    //    }
            //    //    txtView.Text = strBuilder.ToString();
            //    //});
            //}
        }
        public void Release() { }
    }
}
