using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZCompileCore.Builders;
using ZCompileCore.Reports;
using ZDev.UI.Forms;
using ZLangRT;
using ZLangRT.Utils;

namespace ZDev.UI.Compilers
{
    public class ZDevCompiler
    {
        public const string ZYYExt = ".zyy";
        public const string ZXMExt = ".zxm";

        private ZCompileProjectModel projectModel;
        //private ZCompileClassModel classModel;
        private FileInfo srcFileInfo;

        public ProjectCompileResult CompileResult { get; private set;}

        public ZDevCompiler(FileInfo zlogoFileInfo)
        {
            srcFileInfo = zlogoFileInfo;
            if (srcFileInfo.Name.EndsWith(ZXMExt, StringComparison.OrdinalIgnoreCase))
            {
                createXmFileProject(zlogoFileInfo);
            }
            else
            {
                createSingleFileProject(zlogoFileInfo);
            }
        }

        void createSingleFileProject(FileInfo zlogoFileInfo)
        {
            initProject();
            projectModel.EntryClassName = Path.GetFileNameWithoutExtension(srcFileInfo.FullName);
            projectModel.BinaryFileNameNoEx = Path.GetFileNameWithoutExtension(srcFileInfo.FullName);
            projectModel.BinaryFileKind = PEFileKinds.Dll;
            projectModel.ProjectPackageName = "ZDevGen";
            projectModel.ProjectFileName = zlogoFileInfo.Name;
            var classModel = new ZCompileClassModel();
            classModel.SourceFileInfo = srcFileInfo;
            projectModel.AddClass(classModel);
        }

        void createXmFileProject(FileInfo zlogoFileInfo)
        {
            initProject();
            projectModel.ProjectFileName = zlogoFileInfo.Name;
            ZXMParser xmZxmParser = new ZXMParser(zlogoFileInfo);
            ProjectCompileResult compileResult2 = xmZxmParser.Parse(projectModel);
            if (compileResult2.HasError())
            {
                CompileResult = compileResult2;
            }
        }

        void initProject()
        {
            CompileResult = null;
            projectModel = new ZCompileProjectModel();
           
            projectModel.ProjectRootDirectoryInfo = srcFileInfo.Directory;        
            projectModel.BinarySaveDirectoryInfo = srcFileInfo.Directory;
            projectModel.NeedSave = true;
            projectModel.AddRefPackage("Z语言系统");
            projectModel.AddRefDll(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib/Z文件系统.dll")));
            projectModel.AddRefDll(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib/Z桌面控件.dll")));
            projectModel.AddRefDll(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib/Z互联网.dll")));
            projectModel.AddRefDll(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib/Z绘图.dll")));
            projectModel.AddRefDll(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib/Z操作系统.dll")));
            //projectModel.AddRefDll(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib/NSoup.dll")));
        }

        public ProjectCompileResult Compile()
        {
            if (CompileResult == null)
            {
                ZCompileBuilder builder = new ZCompileBuilder();
                ProjectCompileResult result = builder.CompileProject(projectModel);
                CompileResult = result;
                //return result;
            }
            return CompileResult;
        }

        public void Run()
        {
            if (CompileResult == null)
            {
                Compile();
            }
            if (CompileResult == null)
            {
                return;
            }
            if (CompileResult.CompiledTypes.Count > 0)
            {
                RunProcess(CompileResult.BinaryFilePath);
                /*ConsoleForm consoleForm = new ConsoleForm();
                consoleForm.RunAction = () =>
                {
                    string name = projectModel.EntryClassName;
                    Type type = CompileResult.GetProjectType(name);
                    Invoker.Call(type, "启动");
                };
                consoleForm.Show();*/
            }
        }
        
        private void RunProcess(string exe)
        {
            string exeFile = Path.Combine(Application.StartupPath, "ZDev.RunExe.exe");
            string exeArgs = string.Format("runexe \"{0}\"", exe);
            Process CurrentProcess = new Process();
            CurrentProcess.StartInfo.FileName = exeFile;
            CurrentProcess.StartInfo.Arguments = exeArgs;
            CurrentProcess.StartInfo.UseShellExecute = true;
            CurrentProcess.StartInfo.UseShellExecute = true;
            //CurrentProcess.Exited += new EventHandler(CurrentProcess_Exited);
            CurrentProcess.EnableRaisingEvents = true;
            CurrentProcess.Start();
        }

     
    }
}
