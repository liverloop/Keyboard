using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Text;

namespace Sino.Keyboard.Droid.Views
{
    public class AbstractKeyBoard
    {

        #region Events

        public event Action<string> InputFinish;
        public event Action<string> InputProcess;

        #endregion

        #region Fields

        private Context mContext;
        private PopupWindow mPopopWindow;

        #endregion


        #region Constructor

        public AbstractKeyBoard(Context context)
        {
            mContext = context;

            mPopopWindow = new PopupWindow(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent);
            mPopopWindow.Focusable = true;
            mPopopWindow.OutsideTouchable = false;
            mPopopWindow.SetBackgroundDrawable(new BitmapDrawable());
        }


        #endregion

        #region Inner Api

        protected View PutContentView(int layoutResId)
        {
            View view = View.Inflate(mContext, layoutResId, null);
            mPopopWindow.ContentView = view;
            return view;
        }

        protected virtual string GetInput(TextView[] inputs)
        {
            StringBuilder buff = new StringBuilder(inputs.Length);
            foreach (var item in inputs)
            {
                string text = item.Text;
                if (!TextUtils.IsEmpty(text))
                {
                    buff.Append(text);
                }
            }
            return buff.ToString();
        }

        protected virtual void OnShow()
        {

        }

        protected virtual void OnDismiss()
        {

        }

        protected virtual void OnProcess(string numer)
        {
            InputProcess?.Invoke(numer);
        }

        protected virtual void OnFinish(string number)
        {
            InputFinish?.Invoke(number);
        }

        #endregion

        #region Public Interface

        public virtual void Show(View anchorView)
        {

            mPopopWindow.InputMethodMode = InputMethod.Needed;
            mPopopWindow.SoftInputMode = SoftInput.AdjustResize;
            mPopopWindow.ShowAtLocation(anchorView, 
                GravityFlags.Bottom | GravityFlags.CenterHorizontal, 0, 0);

            OnShow();
        }


        public void Dismiss()
        {
            OnDismiss();
            mPopopWindow.Dismiss();
        }

        #endregion
    }
}