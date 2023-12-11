using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnexpectedApis.Views;

public partial class SamplePage : Page
{
    public Uri IconUri => new Uri($"ms-appx:///UnexpectedApis/Assets/{this.GetType().Name.Substring(0, this.GetType().Name.Length - "Page".Length)}.png");
}
