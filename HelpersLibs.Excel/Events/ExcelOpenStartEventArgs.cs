using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.Excel.Events {
    public class ExcelOpenStartEventArgs : EventArgs {
        public string FileName { get; private set; }

        public ExcelOpenStartEventArgs(string fileName) {
            FileName = fileName;
        }
    }

    public delegate void ExcelOpenStartEventHandler(object sender, ExcelOpenStartEventArgs e);
}
