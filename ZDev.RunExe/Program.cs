using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZDev.RunExe
{
    class Program
    {

        public static void Main(string[] args)
        {
            if (args.Length == 0) return;
            string argsCode = string.Join(" ", args);
            string cmd = args[0];
            if (cmd == "runexe")
            {
                try
                {
                    RunExe(args[1]);
                }
                catch (RunCmdExecption ex)
                {
                    Console.WriteLine("argsCode:" + argsCode);
                    Console.WriteLine("args length:" + args.Length);
                    Console.WriteLine("args[0]:" + args[0]);
                    Console.WriteLine("args[1]:" + args[1]);
                }
            }
        }
        /*
        private static void Load(string dll,AppDomain exeDomain)
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dll);
            exeDomain.Load(dllPath);
        }*/
        private static LocalLoader loader = null;
        private static void Load(string dll)
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dll);
            //exeDomain.Load(dllPath);
            loader.LoadAssembly(dll);
        }

        public static void RunExe(string exearg)
        {
            string exe = exearg;
            try
            {
                AppDomainSetup setup = new AppDomainSetup();
                setup.ApplicationName = "RunExe";
                setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
                setup.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "private");
                setup.CachePath = setup.ApplicationBase;
                setup.ShadowCopyFiles = "true";
                setup.ShadowCopyDirectories = setup.ApplicationBase;

                AppDomain domain = AppDomain.CreateDomain("RunExeDomain", null, setup);
                //domain = AppDomain.CurrentDomain;
                if (exearg[0] == '"' || exearg[0] == '\'')
                    exe = exearg.Substring(1, exearg.Length - 2);

                loader = new LocalLoader(domain);
                Load("ZLangRT.dll");
                Load("Lib/Z语言系统.dll");
                Load("Lib/Z文件系统.dll");
                Load("Lib/Z桌面控件.dll");
                Load("Lib/Z互联网.dll");
                Load("Lib/Z绘图.dll");
                Load("Lib/Z操作系统.dll");
                Load("Lib/NSoup.dll");
                domain.ExecuteAssembly(exe);
                loader.Unload();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("exearg:" + exearg);
                Console.WriteLine("exe:" + exe);
                throw new RunCmdExecption(ex.Message);
            }
        }

        public class RunCmdExecption : Exception
        {
            public RunCmdExecption(string Message)
                : base(Message)
            {

            }
        }
    }
}