using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace HelpersLibs.Windows; 
public class WindowHelper {
    public const int BN_CLICKED = 245;
    public const int WM_LBUTTONDOWN = 0x201;
    public const int WM_LBUTTONUP = 0x202;
    public const int IDOK = 1;


    public enum GetWindowType : uint {
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6
    }

    public static IntPtr GetWindowHandle(string windowName) {
        IntPtr hWnd = IntPtr.Zero;

        foreach (Process pList in Process.GetProcesses()) {
            if (pList.MainWindowTitle.Contains(windowName)) {
                hWnd = pList.MainWindowHandle;
            }
        }
        return hWnd;
    }

    public static IntPtr GetDialogBox(IntPtr winHandler) {
        Win32API.SetForegroundWindow(winHandler);
        var openFileDialog = IntPtr.Zero;
        var trys = 0;

        while (openFileDialog == IntPtr.Zero) {
            openFileDialog = Win32API.GetWindow(winHandler, GetWindowType.GW_ENABLEDPOPUP);

            if (openFileDialog == IntPtr.Zero) {
                Thread.Sleep(2000);
                trys++;


                if (trys > 8) {
                    break;
                }
            }
        }

        return openFileDialog;
    }

    public static List<nint> GetEditBoxes(IntPtr winHandler, nint openFileDialog) {
            Win32API.SetForegroundWindow(openFileDialog);
            var editBox = new List<IntPtr>();
            var trys = 0;

            while (editBox.Count() == 0) {
                editBox = WindowHelper.GetControls(openFileDialog, "Edit");

                if (editBox.Count() == 0) {
                    Thread.Sleep(2000);

                    trys++;

                    if (trys == 5) {
                        openFileDialog = GetDialogBox(winHandler);
                    }

                    if (trys > 8) {
                        break;
                    }
                }
            }

            return editBox;

    }

    public static List<IntPtr> GetAllChildHandles(IntPtr mainHandle) {
        List<IntPtr> childHandles = new List<IntPtr>();

        GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
        IntPtr pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

        try {
            Win32API.EnumWindowProc childProc = new Win32API.EnumWindowProc(EnumWindow);
            Win32API.EnumChildWindows(mainHandle, childProc, pointerChildHandlesList);
        } finally {
            gcChildhandlesList.Free();
        }

        return childHandles;
    }

    private static bool EnumWindow(IntPtr hWnd, IntPtr lParam) {
        GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

        if (gcChildhandlesList == null || gcChildhandlesList.Target == null) {
            return false;
        }

        List<IntPtr> childHandles = gcChildhandlesList.Target as List<IntPtr>;
        childHandles.Add(hWnd);

        return true;
    }

    public static String WndClassName(IntPtr handle) {
        int length = 1024;

        StringBuilder sb = new StringBuilder(length);

        Win32API.GetClassName(handle, sb, length);

        return sb.ToString();
    }

    public static bool IsDialogClassName(IntPtr handle) {
        return "#32770".Equals(WndClassName(handle));
    }

    public static string GetWindowTitle(IntPtr handle) {
        int capacity = Win32API.GetWindowTextLength(handle) * 2;
        StringBuilder stringBuilder = new StringBuilder(capacity);
        Win32API.GetWindowText(handle, stringBuilder, stringBuilder.Capacity);

        return stringBuilder.ToString();
    }

    public static List<IntPtr> GetControls(IntPtr parentHandle, string className) {
        List<IntPtr> childHandles = new List<IntPtr>();
        var allcontrols = GetAllChildHandles(parentHandle);
        foreach (var control in allcontrols) {
            if (className.Equals(WndClassName(control))) {
                childHandles.Add(control);
            }
        }
        return childHandles;
    }

    public static void Click(IntPtr handle) {
        Win32API.SendMessage(handle, WM_LBUTTONDOWN, 0, IntPtr.Zero);
        Win32API.SendMessage(handle, WM_LBUTTONUP, 0, IntPtr.Zero);
        Win32API.SendMessage(handle, WM_LBUTTONDOWN, 0, IntPtr.Zero);
        Win32API.SendMessage(handle, WM_LBUTTONUP, 0, IntPtr.Zero);
    }
}