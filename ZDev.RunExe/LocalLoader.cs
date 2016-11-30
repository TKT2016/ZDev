using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZDev.RunExe
{
    public class LocalLoader
    {
        private AppDomain appDomain;
        private RemoteLoader remoteLoader;

        public LocalLoader(AppDomain appDomain)
        {
            this.appDomain = appDomain;
            /*AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = "Test";
            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            setup.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "private");
            setup.CachePath = setup.ApplicationBase;
            setup.ShadowCopyFiles = "true";
            setup.ShadowCopyDirectories = setup.ApplicationBase;*/

            //appDomain = AppDomain.CreateDomain("TestDomain", null, setup);
            string name = Assembly.GetExecutingAssembly().GetName().FullName;
            remoteLoader = (RemoteLoader)appDomain.CreateInstanceAndUnwrap(
                name,
                typeof(RemoteLoader).FullName);
        }

        public void LoadAssembly(string fullName)
        {
            remoteLoader.LoadAssembly(fullName);
        }

        public void Unload()
        {
            AppDomain.Unload(appDomain);
            appDomain = null;
        }

        public string FullName
        {
            get
            {
                return remoteLoader.FullName;
            }
        }
    }  
}
