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
using Microsoft.Identity.Client;

namespace TestAAD.Droid
{
    [Activity]
    [IntentFilter(new[] { Intent.ActionView },
           Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
           DataHost = "auth",
           DataScheme = "msalec28d429-40c4-4ebc-8f8f-db9236df8830")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}