using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using HelpersLibs.AspnetCore.Models;
using HelpersLibs.Collection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HelpersLibs.AspnetCore.HTMLHelpers;
public static class MultiSelectBoxSearchableExtension {
    public static IHtmlContent MultiSelectBoxSearchable<T>(this IHtmlHelper helper, List<T> itens, string title, List<PropertyBindings>? bindings, bool multipleSelection = false) {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        IDocument dropBoxBody = GetDropBoxListBody(path);

        var bttn = dropBoxBody.QuerySelector("#dropdownMenuButton1");
        bttn.InnerHtml = title;

        var dropContent = dropBoxBody.QuerySelector(".dropMenu-itens");

        if (multipleSelection) {
            dropContent.SetAttribute("multipleSelection", "true");
        }

        var itnsList = "";

        if (itens != null && itens.Count() > 0) {
            var dropBoxItemHTML = GetDropBoxListItemHTML(path);
            var bindingPrincipal = bindings.FirstOrDefault(x => x.Principal);
            var extraBindings = bindings.Where(x => !x.Principal).ToList();

            if (extraBindings?.Count() > 0) {
                dropContent.SetAttribute("extraValue", "true");
            }

            foreach (var item in itens) {
                object value = item;
                var labelTxt = item.ToString();

                if (!string.IsNullOrEmpty(bindingPrincipal?.DisplayFrom)) {
                    labelTxt = GetPropValue(item, bindingPrincipal.DisplayFrom)?.ToString();
                    value = labelTxt;
                }

                if (!string.IsNullOrEmpty(bindingPrincipal?.ValueFrom)) {
                    value = GetPropValue(item, bindingPrincipal.ValueFrom);
                }

                using var dropBoxItem = ParseHTML(dropBoxItemHTML);
                itnsList = SetCheckBox(itnsList, dropBoxItem, bindingPrincipal.Binding, labelTxt, value, value?.ToString(), bindingPrincipal.SeletedItens);

                if (extraBindings != null && extraBindings.Count() > 0) {
                    foreach (var extraBinding in extraBindings) {
                        var extraValue = GetPropValue(item, extraBinding.ValueFrom);

                        using var dropBoxItemExtra = ParseHTML(dropBoxItemHTML);
                        itnsList = SetCheckBox(itnsList, dropBoxItemExtra, extraBinding.Binding, labelTxt, extraValue, value?.ToString(), extraBinding.SeletedItens, true, true);
                    }
                }
            }
        }


        itnsList += "<div id=\"empty\" class=\"dropdown-header\">Item não encontrado</div>";

        dropContent.InnerHtml = itnsList;
        return new HtmlString(dropBoxBody.ToHtml());
    }

    public static IHtmlContent MultiSelectBoxSearchable<T>(this IHtmlHelper helper, PaginatedList<T> itens, string title, List<PropertyBindings>? bindings, bool multipleSelection = false) =>
        MultiSelectBoxSearchable(helper, itens?.Source, title, bindings, multipleSelection);

    private static IDocument GetDropBoxListBody(string? path) {
        var rootPath = new DirectoryInfo(path);
        var htmlDropBox = $"{rootPath.FullName}\\HTMLTemplates\\DropBox.html";
        var htmlContent = File.ReadAllText(htmlDropBox);
        IDocument dropBoxBody = ParseHTML(htmlContent);

        return dropBoxBody;
    }

    private static string GetDropBoxListItemHTML(string? path) {
        var rootPath = new DirectoryInfo(path);
        var htmlDropBoxItem = $"{rootPath.FullName}\\HTMLTemplates\\DropBox-Item.html";
        var htmlContent = File.ReadAllText(htmlDropBoxItem);
        return htmlContent;
    }

    private static IDocument ParseHTML(string htmlContent) {
        var context = BrowsingContext.New(Configuration.Default.WithCss());
        var htmlParsed = context.OpenAsync(req => req.Content(htmlContent)).Result;
        return htmlParsed;
    }

    private static string SetCheckBox(string itnsList, IDocument dropBoxItem, string nameItens, string? labelTxt, object valu, string dataFor, IEnumerable<object>? selectedValue, bool itemExtra = false, bool oculted = false) {
        var checkBox = dropBoxItem.QuerySelector(".searchItemSelect");
        var label = dropBoxItem.QuerySelector(".searchItem");

        checkBox.SetAttribute("value", valu?.ToString());
        checkBox.SetAttribute("name", nameItens);
        checkBox.SetAttribute("data-for", dataFor);
        label.SetAttribute("for", nameItens);
        label.InnerHtml = labelTxt;

        if (itemExtra) {
            checkBox.SetAttribute("itemExtra", "true");
        }

        if (oculted) {
            var styleCheck = dropBoxItem.QuerySelector(".itemListDiv").GetStyle();
            styleCheck.SetProperty("display", "none", "important");
        }

        if (selectedValue != null && selectedValue.Count() > 0) {
            foreach (var v in selectedValue) {
                if (valu.Equals(v)) {
                    checkBox.SetAttribute("checked", "true");
                }
            }
        }

        itnsList += $"{dropBoxItem.ToHtml()}{Environment.NewLine}";
        return itnsList;
    }


    private static object GetPropValue(object instance, string propName) {
        PropertyInfo property = instance.GetType().GetProperty(propName);
        object value = property.GetValue(instance, null);

        return value;
    }
}
