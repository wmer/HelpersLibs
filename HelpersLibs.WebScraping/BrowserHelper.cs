using HelpersLibs.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.WebScraping;
public class BrowserHelper(string webDriverLocation, string browserId) : IDisposable {
    private readonly string _driverLocation = webDriverLocation;
    private readonly string _browserId = browserId;

    public IWebDriver WebDriver { get; private set; }
    public IntPtr WindowHandler { get; private set; }

    public void InitBrowser(bool initHeadless = false) {
        try {
            EdgeOptions options = DefineOptions(initHeadless);
            WebDriver = new EdgeDriver(_driverLocation, options);
            WebDriver.Manage().Window.Maximize();

            if (!initHeadless) {
                var name = $"{WebDriver.Title}_{_browserId}";
                Console.WriteLine($"Iniciando BOT: {name}");
                WindowHandler = GetWindowHandler(WebDriver, name);

                while (WindowHandler == IntPtr.Zero) {
                    try {
                        Console.WriteLine($"Esperando inicializar BOT!");
                        WebDriver.Quit();
                        WebDriver.Dispose();

                        Thread.Sleep(1000);

                        WebDriver = new EdgeDriver(_driverLocation, options);
                        WebDriver.Manage().Window.Maximize();

                        name = $"{WebDriver.Title}_{_browserId}";
                        WindowHandler = GetWindowHandler(WebDriver, name);
                    } catch {

                    }
                }

            }

            Thread.Sleep(1000);
        } catch { }

    }

    public void Close() => WebDriver.Quit();
    public void Maximize() => WebDriver.Manage().Window.Maximize();
    public void Reload() {
        WebDriver.Navigate().Refresh();
        SetVInitVars();
        while (!PageIsReady()) ;
    }

    public bool IsBrowserClosed() {
        bool isClosed = false;
        try {
            var title = WebDriver.Title;
            if (string.IsNullOrEmpty(title)) {
                isClosed = true;
            }
        } catch (Exception e) {
            isClosed = true;
        }

        return isClosed;
    }

    public bool Navigate(string url) {
        try {
            FirstTab();
            WebDriver.Navigate().GoToUrl(url);
            SetVInitVars();
            SpinWait.SpinUntil(PageIsReady, 1000);
            //GetCurrentWindowHandler();

            return true;
        } catch (Exception e) {
            return false;
        }
    }

    public bool FirstTab() {
        try {
            WebDriver.SwitchTo().Window(WebDriver.WindowHandles.First());

            return true;
        } catch (Exception e) {
            return false;
        }
    }

    public bool PageIsReady() {
        var ready = false;

        try {
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            ready = js.ExecuteScript($"return document.readyState").Equals("complete");
        } catch { }

        return ready;
    }

    public bool WaitElement(By element, TimeSpan delay) {
        var ready = false;

        try {  
            var wait = new WebDriverWait(WebDriver, delay);
            wait.Until(d => d.FindElements(element).Count != 0);

            ready = true; 
        } catch (Exception e) {
            Console.WriteLine("Elemento não apareceu a tempo.");
            ready = false;
        }

        return ready;
    }

    public IWebElement? WaitAndFindElement(By element, TimeSpan delay) {
        try {
            if (!WaitElement(element, delay)) {
                return null;
            }

            return WebDriver.FindElement(element);
        } catch (Exception) {
            Console.WriteLine("Elemento não encontrado.");
            return null;
        }
    }

    public void SetVInitVars() {
        try {
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            js.ExecuteScript("Object.defineProperty(navigator, 'selenium', {get: () => undefined})");
            js.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
            js.ExecuteScript("Object.defineProperty(navigator, 'driver', {get: () => undefined})");
        } catch (Exception e) { }
    }

    public void ScrollPageToEnd(int interval = 1000) {
        while (!ScrollPage(interval)) ;
    }

    public bool ScrollPage(int interval = 1000) {
        var scroll = false;

        try {
            var newHeight = 0;
            var lasteHeight = Convert.ToInt32(((IJavaScriptExecutor)WebDriver).ExecuteScript("return document.body.scrollHeight"));

            ((IJavaScriptExecutor)WebDriver).ExecuteScript($"window.scrollTo(0, {lasteHeight} / 2)");
            _ = new WebDriverWait(WebDriver, TimeSpan.FromMilliseconds(interval));

            newHeight = Convert.ToInt32(((IJavaScriptExecutor)WebDriver).ExecuteScript("return document.body.scrollHeight"));

            if (!newHeight.Equals(lasteHeight)) {
                scroll = true;
            }
        } catch (Exception e) {
            Console.WriteLine($"Erro ao tentar scrollar a página: {e.Message}");
        }

        return scroll;
    }

    public void MinimizeWindow() {
        Win32API.ShowWindow(WindowHandler, Win32API.SW_MINIMIZE);
    }

    public void TrazerFrente() {
        _ = Win32API.SetForegroundWindow(WindowHandler);
    }

    public bool ImageIsLoad(IWebElement image) {
        var ready = false;

        try {
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            ready = (bool)js.ExecuteScript(
                               "return arguments[0].complete && " +
                               "typeof arguments[0].naturalWidth != \"undefined\" && " +
                               "arguments[0].naturalWidth > 0", image);
        } catch { }

        return ready;
    }

    public void ChangeElementCSS(IWebElement element, string css) {

        try {
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            js.ExecuteScript($"arguments[0].style='{css}'", element);
        } catch { }

    }

    public string GetInnerHtml(IWebElement element) {
        var html = "";

        try {
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            html = js.ExecuteScript("return arguments[0].innerHTML;", element) as string ?? string.Empty;
        } catch { }

        return html;
    }

    public string GetOuterHtml(IWebElement element) {
        var html = "";

        try {
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            html = js.ExecuteScript("return arguments[0].outerHTML;", element) as string ?? string.Empty;
        } catch { }

        return html;
    }

    public void ChangeInnerHtml(IWebElement element, string html) {

        try {
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            js.ExecuteScript($"return arguments[0].innerHTML = {html};", element);
        } catch { }
    }

    public void AppendHtml(IWebElement element, string html) {

        try {
            IJavaScriptExecutor js = (IJavaScriptExecutor)WebDriver;
            js.ExecuteScript($"return arguments[0].append({html});", element);
        } catch { }
    }

    public Bitmap GetElementScreenShot(Bitmap pageScreenshot, IWebElement element) {
        if (element == null) {
            throw new ArgumentNullException(nameof(element), "The provided element cannot be null.");
        }

        if (pageScreenshot == null) {
            throw new ArgumentNullException(nameof(pageScreenshot), "The provided page screenshot cannot be null.");
        }

        Bitmap bitmap = new(element.Size.Width, element.Size.Height);
        using var grD = Graphics.FromImage(bitmap);
        grD.DrawImage(pageScreenshot, new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                      new Rectangle(element.Location, pageScreenshot.Size), GraphicsUnit.Pixel);

        return bitmap;
    }

    public Bitmap GetPageScreenShot() {
        var getMaxSide = "return Math.max(document.body.scroll{0}, document.body.offset{0}, document.documentElement.client{0}, document.documentElement.scroll{0}, document.documentElement.offset{0})";
        var jsExecutor = WebDriver as IJavaScriptExecutor ?? throw new InvalidOperationException("WebDriver does not implement IJavaScriptExecutor.");

        // Ensure scrollHeight and scrollWidth are not null
        var scrollHeight = jsExecutor.ExecuteScript(string.Format(getMaxSide, "Height"))?.ToString();
        var scrollWidth = jsExecutor.ExecuteScript(string.Format(getMaxSide, "Width"))?.ToString();

        if (string.IsNullOrEmpty(scrollHeight) || string.IsNullOrEmpty(scrollWidth)) {
            throw new InvalidOperationException("Failed to retrieve scroll dimensions.");
        }

        WebDriver.Manage().Window.Size = new Size(int.Parse(scrollWidth), int.Parse(scrollHeight));
        var byteArray = ((ITakesScreenshot)WebDriver).GetScreenshot().AsByteArray;

        using var memStream = new MemoryStream(byteArray);
        memStream.Seek(0, SeekOrigin.Begin);
        Bitmap bitmap = new(memStream);
        return bitmap;
    }

    public Bitmap GetElementScreenShot(Point location, Size size) {
        Screenshot sc = ((ITakesScreenshot)WebDriver).GetScreenshot();
        var img = Image.FromStream(new MemoryStream(sc.AsByteArray)) as Bitmap;
        Bitmap bitmap = img.Clone(new Rectangle(location, size), img.PixelFormat);
        return bitmap;
    }

    private EdgeOptions DefineOptions(bool initHeadless = false) {
        var userData = CreateUserDataFolder($"User Data1");
        var currentDir = Environment.CurrentDirectory;
        var ublockOrigin = $"{currentDir}\\Extensions\\uBlock-Origin.crx";
        var adBlockPlus = $"{currentDir}\\Extensions\\Adblock-Plus.crx";

        var options = new EdgeOptions();

        options.AddExcludedArgument("enable-automation");
        options.AddExcludedArgument("webdriver");
        options.AddExcludedArgument("selenium");
        options.AddExcludedArgument("driver");
        options.AddArgument("-disable-blink-features=AutomationControlled");
        options.AddArgument("start-maximized");
        // options.AddArgument("--user-data-dir=" + userData);
        options.AddArgument("ignore-certificate-errors");
        options.AddArgument("--disable-bundled-ppapi-flash");
        options.AddArgument("--disable-plugins-discovery");
        options.AddArgument("disable-gpu");
        options.AddArgument("--disable-crash-reporter");
        options.AddArgument("--disable-in-process-stack-traces");
        options.AddArgument("--disable-logging");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--log-level=3");
        options.AddArgument("--output=/dev/null");
        options.AddArgument("no-sandbox");
        options.AcceptInsecureCertificates = true;
        //options.AddExtension(ublockOrigin);
        //options.AddExtension(adBlockPlus);

        if (initHeadless) {
            options.AddArgument("headless");
        }

        return options;
    }

    public string CreateUserDataFolder(string folderName) {
        string userData = $@"C:\Users\{Environment.UserName}\AppData\Local\Microsoft\Edge\{folderName}";

        if (Directory.Exists(userData)) {
            Directory.CreateDirectory(userData);
        }

        return userData;
    }

    public void GetCurrentWindowHandler() {
        var name = WebDriver.Title;
        WindowHandler = GetWindowHandler(WebDriver, name);
    }

    public virtual IntPtr GetWindowHandler(IWebDriver webDriver, string name) {
        IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;

        var windowsHandler = IntPtr.Zero;

        while (windowsHandler == IntPtr.Zero) {
            try {
                var windowsName = (string)js.ExecuteScript($"document.title = '{name}'");
                windowsHandler = WindowHelper.GetWindowHandle(name);
            } catch { }
        }

        return windowsHandler;
    }

    public void Dispose() {
        try {
            WebDriver?.Quit();
            WebDriver?.Dispose();
        } catch { }
    }
}
