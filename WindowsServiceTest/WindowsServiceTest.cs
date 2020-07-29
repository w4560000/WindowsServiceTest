using System.IO;
using System.ServiceProcess;

namespace WindowsServiceTest
{
    public partial class WindowsServiceTest : ServiceBase
    {
        private string path = "C:\\log.txt"; //寫檔路徑

        public WindowsServiceTest()
        {
            InitializeComponent();
            this.CanHandleSessionChangeEvent = true;
            this.CanHandlePowerEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            Windowshook.writeLog("Start");
            Windowshook.SetWindowsHook();

        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            Windowshook.writeLog("OnSessionChange");
            //if (changeDescription.Reason == SessionChangeReason.SessionLock)
            //{
            //    Windowshook.writeLog("locked");
            //    Windowshook.SetWindowsHook();

            //    // 屏幕锁定
            //}
            //else if (changeDescription.Reason == SessionChangeReason.SessionUnlock)
            //{
            //    Windowshook.writeLog("UnLocked");
            //    //Mail.Send(message);
            //    Windowshook.UnhookWindowsHookEx(Windowshook.m_HookHandle);
            //    Windowshook.m_HookHandle = 0;
            //}
        }

        protected override void OnStop()  //Service停止時要做什麼
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("Service stop!");
            }

            Windowshook.UnhookWindowsHookEx(Windowshook.m_HookHandle);
            Windowshook.m_HookHandle = 0;
        }
    }
}