using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using ZCompileCore.Analys.AContexts;
using ZCompileCore.Analys.EContexts;
using ZCompileCore.Builders;
using ZCompileCore.Reports;

namespace ZDev.UI.Compilers
{
    class ZXMParser
    {
        private FileInfo xmFileInfo;

        public ZXMParser(FileInfo xmFileInfo)
        {
            this.xmFileInfo = xmFileInfo;
        }

        public ProjectCompileResult Parse(ZCompileProjectModel zCompileProject/*, ProjectCompileResult result*/)
        {
            ProjectCompileResult result = new ProjectCompileResult();
            string[] lines = File.ReadAllLines(xmFileInfo.FullName);
            var folderPath = zCompileProject.ProjectRootDirectoryInfo.FullName;
            //ProjectContext context = new ProjectContext();
            //Messager.Clear();
            //context.Analy();

            for (int i = 0; i < lines.Length; i++)
            {
                string code = lines[i];
                if (string.IsNullOrEmpty(code))
                {
                    continue;
                }
                else if (code.StartsWith("//"))
                {
                    continue;
                }
                else if (code.StartsWith("包名称:"))
                {
                    string name = code.Substring(4);
                    zCompileProject.ProjectPackageName = name;
                }
                else if (code.StartsWith("生成类型:"))
                {
                    string lx = code.Substring(5);
                    if (lx == "开发包")
                    {
                        zCompileProject.BinaryFileKind = PEFileKinds.Dll;
                    }
                    else if (lx == "控制台程序")
                    {
                        zCompileProject.BinaryFileKind = PEFileKinds.ConsoleApplication;
                    }
                    else if (lx == "桌面程序")
                    {
                        zCompileProject.BinaryFileKind = PEFileKinds.WindowApplication;
                    }
                }
                else if (code.StartsWith("编译:"))
                {
                    string src = code.Substring(3);
                    string srcPath = Path.Combine(folderPath, src);
                    FileInfo srcFileInfo = new FileInfo(srcPath);
                    //zCompileProject.SouceFileList.Add(srcFileInfo);
                    var classModel = new ZCompileClassModel();
                    classModel.SourceFileInfo = srcFileInfo;
                    zCompileProject.AddClass(classModel);
                }
                else if (code.StartsWith("设置启动:"))
                {
                    string name = code.Substring(5);
                    zCompileProject.EntryClassName = name;
                }
                else
                {
                    //throw new CompileException("无法识别项目编译指令:" + code);
                    errorf(result, xmFileInfo.Name, i + 1, 1, "无法识别项目编译指令:'{0}'", code);
                    continue;
                }
            }
            return result;
        }

        void errorf(ProjectCompileResult result, string file, int line, int col, string formatstring,
            params string[] args)
        {
            CompileMessage msg = new CompileMessage()
            {
                FileName = file,
                Line = line,
                Col = col,
                Text = string.Format(formatstring, args)
            };
            result.Errors.Add(msg);
        }
    }
}
