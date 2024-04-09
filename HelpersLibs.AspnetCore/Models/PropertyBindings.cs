using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.AspnetCore.Models {
    public class PropertyBindings {
        public string? DisplayFrom { get; set; }
        public string? ValueFrom { get; set; }
        public string? Binding { get; set; }
        public IEnumerable<object>? SeletedItens { get; set; }
        public bool Principal { get; set; }
    }
}
