using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.Windows;
[StructLayout(LayoutKind.Sequential)]
public struct INPUT {
    public int type;
    public INPUTKEYBDINPUT ki;
}
