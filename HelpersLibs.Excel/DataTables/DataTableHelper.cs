using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.Excel.DataTables {
    public static class DataTableHelper {

        public static DataTable? ToDataTable<T>(this IEnumerable<T> list) {
            DataTable dataTable = new(typeof(T).Name);

            Type t = typeof(T);
            bool isDict = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>);

            if (isDict) {
                var item = list.FirstOrDefault() as IDictionary;
                var keys = item.Keys;

                foreach (var key in keys) {
                    dataTable.Columns.Add($"{key}");
                }

                for (int indexList = 0; indexList < list.Count(); indexList++) {
                    var itemList = list.ElementAt(indexList) as IDictionary;
                    DataRow row = dataTable.Rows.Add();

                    var values = itemList.Values;
                    object[] valuesArray = new object[values.Count];
                    values.CopyTo(valuesArray, 0);

                    for (int rowNum = 0; rowNum < valuesArray.Length; rowNum++) {
                        var rowValue = valuesArray[rowNum];
                       // rowValue = Convert.ChangeType(rowValue, rowValue.GetType());
                        row[rowNum] = rowValue;
                    }
                }

            } else {
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < Props.Length; i++) {
                    dataTable.Columns.Add(Props[i].Name);
                }

                for (int indexList = 0; indexList < list.Count(); indexList++) {
                    var item = list.ElementAt(indexList);
                    DataRow row = dataTable.Rows.Add();

                    for (int rowNum = 0; rowNum < Props.Length; rowNum++) {
                        var rowValue = Props[rowNum].GetValue(item, null);
                       // rowValue = Convert.ChangeType(rowValue, Props[rowNum].PropertyType);

                        if (Props[rowNum].PropertyType == typeof(DateTime))
                        {
                            var dt = (DateTime)Props[rowNum].GetValue(item, null);
                            rowValue = dt.ToString("dd/MM/yyyy");
                        }

                        row[rowNum] = rowValue;
                    }
                }
            }



            return dataTable;
        }

        //public static DataTable? ToDataTable<T>(this IEnumerable<T> list) {
        //    var json = JsonConvert.SerializeObject(list, Formatting.Indented);
        //    var dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
        //    return dt;

        //    //DataTable dataTable = new DataTable(typeof(T).Name);

        //    //PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //    //foreach (PropertyInfo prop in Props) {
        //    //    dataTable.Columns.Add(prop.Name);
        //    //}
        //    //foreach (T item in list) {
        //    //    var values = new object[Props.Length];
        //    //    for (int i = 0; i < Props.Length; i++) {
        //    //        values[i] = Props[i].GetValue(item, null);
        //    //    }
        //    //    dataTable.Rows.Add(values);
        //    //}

        //    //return dataTable;
        //}

        public static IEnumerable<T>? ToIEnumerable<T>(this DataTable dt) {
            var json = JsonConvert.SerializeObject(dt, Formatting.Indented);
            var list = (IEnumerable<T>)JsonConvert.DeserializeObject(json, (typeof(IEnumerable<T>)));
            return list;
        }

        public static Dictionary<string, object>? ToDictionary(this DataTable dt) {
            var json = JsonConvert.SerializeObject(dt, Formatting.Indented);
            var list = (Dictionary<string, object>)JsonConvert.DeserializeObject(json, (typeof(Dictionary<string, object>)));
            return list;
        }
    }
}
