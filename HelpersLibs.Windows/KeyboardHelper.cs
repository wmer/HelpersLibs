using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HelpersLibs.Windows {
    public class KeyboardHelper {
        public void SendKeysCombination(params int[] keys) {
            var inouts = new List<INPUT>();

            foreach (var key in keys) {
                var input = new INPUT();
                input.type = 1;
                input.ki.wVk = (ushort)key;
                input.ki.wScan = 0;
                input.ki.dwFlags = 0;

                inouts.Add(input);
            }

            foreach (var key in keys.Reverse()) {
                var input = new INPUT();
                input.type = 1;
                input.ki.wVk = (ushort)key;
                input.ki.wScan = 0;
                input.ki.dwFlags = Win32API.KEYEVENTF_KEYUP;

                inouts.Add(input);
            }

            Win32API.SendInput((uint)keys.Count() * 2, inouts.ToArray(), Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
