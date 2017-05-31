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
    public class ErrorDialogHandler
    {

        Android.Support.V4.App.FragmentManager mFragmentManager;

        public ErrorDialogHandler(Android.Support.V4.App.FragmentManager fragmentManager)
        {
            mFragmentManager = fragmentManager;
        }

        public void ShowError(String errorMessage)
        {
            Android.Support.V4.App.DialogFragment fragment = ErrorDialogFragment.NewInstance(
                    Resource.String.validationErrors, errorMessage);
            fragment.Show(mFragmentManager, "error");
        }
    }
}