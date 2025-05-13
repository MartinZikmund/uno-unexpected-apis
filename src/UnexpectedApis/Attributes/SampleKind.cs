using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnexpectedApis.Attributes;

public enum SampleKind
{
    /// <summary>
    /// A sample that is not a UI element.
    /// </summary>
    NonUI,
    /// <summary>
    /// A sample that is a UI element.
    /// </summary>
    UI,
}
