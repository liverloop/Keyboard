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
using Android.InputMethodServices;
using Android.Text;

namespace Sino.Keyboard.Droid.Views
{
    public class VehiclePlateKeyboard : AbstractKeyBoard
    {
        #region Fields

        private const int NUMBER_LENGTH = 7;
        private const string WJ_PREFIX = "WJ";
        private const string PROVINCE_CHINESE = "@¾©½ò½ú¼½ÃÉÁÉ¼ªºÚ»¦ËÕÕãÍîÃö¸ÓÂ³Ô¥¶õÏæÔÁ¹ðÇíÓå´¨¹óÔÆ²ØÉÂ¸ÊÇàÄþÐÂÎä";
        private const string EXTRA_CHINESE = "@¸Û°Ä¾¯Ñ§¹Ò";

        private KeyboardView mKeyboardView;
        private TextView[] mNumbersTextView = new TextView[NUMBER_LENGTH];
        private View mCommitButton;
        private TextView mSelectedTextView;
        private Android.InputMethodServices.Keyboard mProvinceKeyboard_0;
        private Android.InputMethodServices.Keyboard mProvinceKeyboard_1;
        private Android.InputMethodServices.Keyboard mCityCodeKeyboard;
        private Android.InputMethodServices.Keyboard mNumberKeyboard;
        private Android.InputMethodServices.Keyboard mNUmberExtraKeyboard;

        private int mShowingKeyboard = 0;
        private string mDefaultPlateNumber;

        #endregion

        #region Constructor

        public VehiclePlateKeyboard(Context context) : base(context)
        {
            var contentView = PutContentView(
                Resource.Layout.keyboard_vehicle_plate);

            mNumbersTextView[0] = contentView.FindViewById<TextView>(
                Resource.Id.keyboard_number_0);
            mNumbersTextView[1] = contentView.FindViewById<TextView>(
                Resource.Id.keyboard_number_1);
            mNumbersTextView[2] = contentView.FindViewById<TextView>(
                Resource.Id.keyboard_number_2);
            mNumbersTextView[3] = contentView.FindViewById<TextView>(
                Resource.Id.keyboard_number_3);
            mNumbersTextView[4] = contentView.FindViewById<TextView>(
                Resource.Id.keyboard_number_4);
            mNumbersTextView[5] = contentView.FindViewById<TextView>(
                Resource.Id.keyboard_number_5);
            mNumbersTextView[6] = contentView.FindViewById<TextView>(
                Resource.Id.keyboard_number_6);

            foreach (var item in mNumbersTextView)
            {
                item.SoundEffectsEnabled = false;
                item.Click += TextViewItemClick;
            }

            mProvinceKeyboard_0 = new Android.InputMethodServices.Keyboard(
                context, Resource.Xml.keyboard_vehicle_province_0);
            mProvinceKeyboard_1 = new Android.InputMethodServices.Keyboard(
                context, Resource.Xml.keyboard_vehicle_province_1);
            mCityCodeKeyboard = new Android.InputMethodServices.Keyboard(
                context, Resource.Xml.keyboard_vehicle_code);
            mNumberKeyboard = new Android.InputMethodServices.Keyboard(
                context, Resource.Xml.keyboard_vehicle_number);
            mNUmberExtraKeyboard = new Android.InputMethodServices.Keyboard(
                context, Resource.Xml.keyboard_vehicle_number_extra);

            mKeyboardView = contentView.FindViewById<KeyboardView>(
                Resource.Id.keyboard_view);
         
            mKeyboardView.Key += (sender, keyCodes) =>
            {              
                int code = (int)keyCodes.KeyCodes[0];
                char charCode = (char)keyCodes.PrimaryCode;
                if (400 < code && code < 500)
                {
                    charCode = PROVINCE_CHINESE[code - 400];
                }
                else if (500 < code)
                {
                    charCode = EXTRA_CHINESE[code - 500];
                }
                mSelectedTextView.Text = ((char)charCode).ToString();
                NextNumber();
            };
            mKeyboardView.PreviewEnabled = false;

            mCommitButton = contentView.FindViewById<Button>(
                Resource.Id.keyboard_commit);
            mCommitButton.Click += (s, e) => 
            {
                string number = GetInput(mNumbersTextView);
                if (number.Length == mNumbersTextView.Length)
                {
                    OnFinish(number);
                    Dismiss();
                }
            };
        }



        private void NextNumber()
        {
            string number = GetInput(mNumbersTextView);
            OnProcess(number);

            int viewId = mSelectedTextView.Id;

            if (viewId == Resource.Id.keyboard_number_0)
            {
                mNumbersTextView[1].PerformClick();
            }
            else if (viewId == Resource.Id.keyboard_number_1)
            {
                mNumbersTextView[2].PerformClick();
            }
            else if (viewId == Resource.Id.keyboard_number_2)
            {
                mNumbersTextView[3].PerformClick();
            }
            else if (viewId == Resource.Id.keyboard_number_3)
            {
                mNumbersTextView[4].PerformClick();
            }
            else if (viewId == Resource.Id.keyboard_number_4)
            {
                mNumbersTextView[5].PerformClick();
            }
            else if (viewId == Resource.Id.keyboard_number_5)
            {
                mNumbersTextView[6].PerformClick();
            }
            //else if (viewId == Resource.Id.keyboard_number_6)
            //{
            //    mCommitButton.PerformClick();
            //}
        }
        #endregion


        #region  Inner Operation

        protected override void OnShow()
        {
            mNumbersTextView[0].PerformClick();
        }

        protected override string GetInput(TextView[] inputs)
        {
            var number = base.GetInput(inputs);
            return number.Replace("Îä",WJ_PREFIX);
        }

        public override void Show(View anchorView)
        {
            if (!TextUtils.IsEmpty(mDefaultPlateNumber))
            {
                char[] numbers = mDefaultPlateNumber.ToUpper().ToCharArray();
                int limited = Math.Min(NUMBER_LENGTH, numbers.Length);

                for (int i = 0; i < limited; i++)
                {
                    mNumbersTextView[i].Text = numbers[i].ToString();
                }
            }
            base.Show(anchorView);
        }

        private void TextViewItemClick(object sender, EventArgs e)
        {
            if (mSelectedTextView != null)
            {
                mSelectedTextView.Activated = false;
            }

            mSelectedTextView = (TextView)sender;
            mSelectedTextView.Activated = true;

            int id = mSelectedTextView.Id;

            if (id == Resource.Id.keyboard_number_0)
            {
                if (mShowingKeyboard != Resource.Xml.keyboard_vehicle_province_1)
                {
                    mShowingKeyboard = Resource.Xml.keyboard_vehicle_province_1;
                    mKeyboardView.Keyboard = mProvinceKeyboard_1;
                }
            }
            else if (id == Resource.Id.keyboard_number_1)
            {
                string number = GetInput(mNumbersTextView);
                if (number.StartsWith(WJ_PREFIX))
                {
                    mShowingKeyboard = Resource.Xml.keyboard_vehicle_province_0;
                    mKeyboardView.Keyboard = mProvinceKeyboard_0;
                }
                else
                {
                    if (mShowingKeyboard != Resource.Xml.keyboard_vehicle_code)
                    {
                        mShowingKeyboard = Resource.Xml.keyboard_vehicle_code;
                        mKeyboardView.Keyboard = mCityCodeKeyboard;
                    }
                }
            }
            else if (id == Resource.Id.keyboard_number_6)
            {
                if (mShowingKeyboard != Resource.Xml.keyboard_vehicle_number_extra)
                {
                    mShowingKeyboard = Resource.Xml.keyboard_vehicle_number_extra;
                    mKeyboardView.Keyboard = mNUmberExtraKeyboard;
                }
            }
            else
            {
                if (mShowingKeyboard != Resource.Xml.keyboard_vehicle_number)
                {
                    mShowingKeyboard = Resource.Xml.keyboard_vehicle_number;
                    mKeyboardView.Keyboard = mNumberKeyboard;
                }
            }

            mKeyboardView.InvalidateAllKeys();
            mKeyboardView.Invalidate();
        }

        #endregion

        #region Public Interface

        public void SetDefaultPlateNumber(string number)
        {
            if (!TextUtils.IsEmpty(number))
            {
                if (number.StartsWith(WJ_PREFIX))
                {
                    mDefaultPlateNumber = "Îä" + number.Substring(
                        number.Length > 2 ? 2 : 0);
                }
                else
                {
                    mDefaultPlateNumber = number;
                }
            }
        }

        public static void Show(Activity activity)
        {
            new VehiclePlateKeyboard(activity).Show(
                activity.Window.DecorView.RootView);
        }

        public static VehiclePlateKeyboard Create(Context context)
        {
            return new VehiclePlateKeyboard(context);
        }

        #endregion
    }
}