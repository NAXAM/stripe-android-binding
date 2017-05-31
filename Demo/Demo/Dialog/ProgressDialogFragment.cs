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
using Android.Support.V4.App;

namespace Demo.Dialog
{
    public class ProgressDialogFragment : Android.Support.V4.App.DialogFragment
    {
        public static ProgressDialogFragment NewInstance(int msgId)
        {
            ProgressDialogFragment fragment = new ProgressDialogFragment();
            Bundle args = new Bundle();
            args.PutInt("msgId", msgId);
            fragment.Arguments = args;
            return fragment;
        }
        public ProgressDialogFragment()
        { 
        }
        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            int msgId = Arguments.GetInt("msgId");
            ProgressDialog dialog = new ProgressDialog(Activity);
            dialog.SetMessage(Activity.Resources.GetString(msgId));
            return dialog;
        }
    }

}