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
using Demo.Dialog;
using Android.Support.V4.App;
namespace Demo.Controller
{
    public class ProgressDialogController
    {

        private Android.Support.V4.App.FragmentManager mFragmentManager;
        private ProgressDialogFragment mProgressFragment;

        public ProgressDialogController(Android.Support.V4.App.FragmentManager fragmentManager)
        {
            mFragmentManager = fragmentManager;
            mProgressFragment = ProgressDialogFragment.NewInstance(Resource.String.progressMessage);
        }

        public void SetMessageResource(int resId)
        {
            if (mProgressFragment.IsVisible)
            {
                mProgressFragment.Dismiss();
                mProgressFragment = null;
            }
            mProgressFragment = ProgressDialogFragment.NewInstance(resId);
        }

        public void StartProgress()
        {
            mProgressFragment.Show(mFragmentManager, "progress");
        }

        public void FinishProgress()
        {
            mProgressFragment.Dismiss();
        }
    }

}