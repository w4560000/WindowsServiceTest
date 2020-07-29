using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceTest
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new WindowsServiceTest()
            };
            ServiceBase.Run(ServicesToRun);


            // 02-使用RunInteractive來執行原有Service功能以進行偵錯
            //RunInteractive(ServicesToRun);
        }

        static void RunInteractive(ServiceBase[] servicesToRun)
        {
            // 利用Reflection取得非公開之 OnStart() 方法資訊
            MethodInfo onStartMethod = typeof(ServiceBase).GetMethod("OnStart",
                BindingFlags.Instance | BindingFlags.NonPublic);

            // 執行 OnStart 方法
            foreach (ServiceBase service in servicesToRun)
            {
                Console.Write("Starting {0}...", service.ServiceName);
                onStartMethod.Invoke(service, new object[] { new string[] { } });
                Console.Write("Started");
            }

            DateTime startNow = DateTime.Now;

            Console.WriteLine("Press P key to stop the services");
            while (true)
            {
                if (DateTime.Now > startNow.AddMinutes(10))
                {

                    // 利用Reflection取得非公開之 OnStop() 方法資訊
                    MethodInfo onStopMethod = typeof(ServiceBase).GetMethod("OnStop",
                        BindingFlags.Instance | BindingFlags.NonPublic);

                    // 執行 OnStop 方法
                    foreach (ServiceBase service in servicesToRun)
                    {
                        Console.Write("Stopping {0}...", service.ServiceName);
                        onStopMethod.Invoke(service, null);
                        Console.WriteLine("Stopped");
                    }
                    break;
                }
            }
        }
    }
}
