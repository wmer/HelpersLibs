using HelpersLibs.Excel;
using HelpersLibs.Web.Helpers;
using System.Data;
using System.Net;

namespace HelperLibs.Test; 
[TestClass]
public class UnitTest1 {
    [TestMethod]
    public void TestMethod1() {
        var excelHelper = new ExcelHelper();

        var roootPath = $"{Environment.CurrentDirectory}\\Ips";

        if (!Directory.Exists(roootPath)) {
            Directory.CreateDirectory(roootPath);
        }
;
        var baseOriPath = $"{roootPath}\\4c.xlsx";

        var scoreDt = excelHelper.GetDataTableFromExcel(baseOriPath);
        var dt = scoreDt.AsEnumerable();

        foreach ( var t in dt ) {
            var ip = t.Field<string>("IP");
            var ipP = IPAddress.Parse(ip.Trim());
            var test = IPHelper.IsInSubnet(ipP, "186.233.104.0/24");

            if (!test) {
                var fdfd = "";
            }
        }


        Assert.IsNotNull( dt );
    }
}