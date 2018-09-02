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

namespace GHC.Data
{
    public class HealthService
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SwahiliName { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}