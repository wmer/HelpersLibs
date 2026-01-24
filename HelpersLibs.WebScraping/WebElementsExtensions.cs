using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HelpersLibs.WebScraping; 
public static class WebElementsExtensions {
    public static bool TryClickOn(this IWebElement? el, By by) {
        try {
            var element = el?.FindElement(by);
            element?.Click();
            return true;
        } catch {
            return false;
        }
    }

    public static bool TryClickOn(this IWebDriver? el, By by) {
        try {
            var element = el?.FindElement(by);
            element?.Click();
            return true;
        } catch {
            return false;
        }
    }
    public static bool TryFindElement(this IWebElement? el, By by, out IWebElement? element) {
        try {
            element = el?.FindElement(by);
            return true;
        } catch {
            element = null;
            return false;
        }
    }

    public static bool TryFindElement(this IWebDriver? el, By by, out IWebElement? element) {
        try {
            element = el?.FindElement(by);
            return true;
        } catch {
            element = null;
            return false;
        }
    }

    public static bool TryFindElements(this IWebElement? el, By by, out ReadOnlyCollection<IWebElement>? element) {
        try {
            element = el?.FindElements(by); 
            return true;
        } catch {
            element = null;
            return false;
        }
    }

    public static bool TryFindElements(this IWebDriver? el, By by, out ReadOnlyCollection<IWebElement>? element) {
        try {
            element = el?.FindElements(by);
            return true;
        } catch {
            element = null;
            return false;
        }
    }
}
