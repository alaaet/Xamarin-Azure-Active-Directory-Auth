﻿using System;
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
           DataScheme = "msalCLIENT-ID-GOES-HERE")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}