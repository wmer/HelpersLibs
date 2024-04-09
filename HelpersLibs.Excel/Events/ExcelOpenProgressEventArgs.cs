using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.Excel.Events {
    public class ExcelOpenProgressEventArgs : EventArgs {
        public string FileName { get; private set; }
        public int TotalLines { get; private set; }
        public int ActualLine { get; private set; }
        public string Progress { get; private set; }

        public ExcelOpenProgressEventArgs(string fileName, int totalLines, int actualLine, string progress) {
            FileName = fileName;
            TotalLines = totalLines;
            ActualLine = actualLine;
            Progress = progress;
        }

    }


    public delegate void ExcelOpenProgressEventHandler(object sender, ExcelOpenProgressEventArgs e);
}
