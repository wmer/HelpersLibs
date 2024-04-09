using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.Web.Events {
    public class ApiBaseEventArgs : EventArgs {
        public string EndPoint { get; set; }
        public string Verb { get; set; }
    }
}
