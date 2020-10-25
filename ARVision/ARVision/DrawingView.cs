using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Vision;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Javax.Security.Auth;

namespace ARVision
{
    public class DrawingView : View
    {
        #region Member
        #endregion
        #region Constructor, Destructor & Disposing
        public DrawingView(Context context) : base(context)
        {
            this.Items = new List<DrawItem>();
        }
        public DrawingView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            this.Items = new List<DrawItem>();
        }
        #endregion
        #region Properties
        public List<DrawItem> Items { get; private set; }

        public CameraSource Camera { get; set; }
        #endregion
        #region Methods
        protected override void OnDraw(Canvas canvas)
        {
            float widthOffset = this.Width / (this.Camera != null ? this.Camera.PreviewSize.Width : 1.0f);
            float heightOffset = this.Height / (this.Camera != null ? this.Camera.PreviewSize.Height : 1.0f);

            //base.OnDraw(canvas);

            Paint bgPaint = new Paint();
            bgPaint.Color = Color.DarkGray;

            Paint txtPaint = new Paint();
            txtPaint.Color = Color.White;

            foreach(DrawItem item in this.Items)
            {
                Rect bound = new Rect();

                txtPaint.TextSize = item.Bottom - item.Top;
                txtPaint.GetTextBounds(item.Text, 0, item.Text.Length, bound);
                canvas.DrawRect(item.Left * widthOffset, item.Top * heightOffset, item.Right * widthOffset, item.Bottom * heightOffset, bgPaint);
                canvas.DrawText(item.Text, item.Left * widthOffset, (item.Top * heightOffset) + bound.Height(), txtPaint);
            }

            //Paint paint = new Paint();
            //paint.Color = Color.Red;
            //canvas.DrawRect(0, 0, 300, 300, paint);
            //this.Invalidate();
        }
        #endregion
        #region Events
        #endregion
    }

    public class DrawItem
    {
        #region Member
        #endregion
        #region Constructor, Destructor & Disposing
        #endregion
        #region Properties
        public string Text { get; set; }
        public float Left { get; set; }
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        #endregion
        #region Methods
        #endregion
        #region Events
        #endregion
    }
}