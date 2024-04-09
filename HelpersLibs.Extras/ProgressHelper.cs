using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.Extras;
public class ProgressHelper {

    public static double CalculeProgress(double actualLine, double totalLines) {
        var razao = (double)actualLine / (double)totalLines;
        var percent = razao * 100;
        percent = Math.Round(percent, 2);

        return percent;
    }
}