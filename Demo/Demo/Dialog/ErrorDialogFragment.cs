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

namespace Demo.Dialog
{
    public class ErrorDialogFragment : Android.Support.V4.App.DialogFragment
    {
        public static ErrorDialogFragment NewInstance(int titleId, String message)
        {
            ErrorDialogFragment fragment = new ErrorDialogFragment();
            Bundle args = new Bundle();
            args.PutInt("titleId", titleId);
            args.PutString("message", message);
            fragment.Arguments = args;
            return fragment;
        }

        

        public ErrorDialogFragment()
        {
            // Empty constructor required for DialogFragment
        }

 

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            int titleId = Arguments.GetInt("titleId");
            string message = Arguments.GetString("message");

           
            return new AlertDialog.Builder(Context)
                .SetTitle(titleId)
                .SetMessage(message) 
                .Create();
        }
        public class ClickListener : IDialogInterfaceOnClickListener
        {
            public IntPtr Handle { get; }

            public void Dispose()
            {
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                dialog.Dismiss();
            }
        }
    }
}