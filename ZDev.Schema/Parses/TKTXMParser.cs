using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDev.Schema.Models.TKTXM;

namespace ZDev.Schema.Parses
{
    public class TKTXMParser
    {
        public TKTXMFileModel Parse(FileInfo projectFileInfo)
        {
            //ProjectCompileResult result = new ProjectCompileResult();
            string[] lines = File.ReadAllLines(projectFileInfo.FullName);
            //FileInfo projectFileInfo = new FileInfo(projfile);
            string folder = projectFileInfo.Directory.FullName;
            TKTXMFileModel model = CompileLines(lines);
            return model;
        }

        public TKTXMFileModel CompileLines(string[] lines)
        {
            TKTXMFileModel model = new TKTXMFileModel();

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
                    //context.RootNameSpace = name;
                    model.PackageName = name;
                }
                else if (code.StartsWith("生成类型:"))
                {
                    string lx = code.Substring(5);
                    model.GenerateType = lx;
                }
                else if (code.StartsWith("编译:"))
                {
                    string src = code.Substring(3);
                    model.SrcFiles.Add(src);
                }
                else if (code.StartsWith("设置启动:"))
                {
                    string name = code.Substring(5);
                    model.Entry = name;
                }
                else
                {
                    //throw new CompileException("无法识别项目编译指令:" + code);
                    //errorf(result, file, i + 1, 1, "无法识别项目编译指令:'{0}'", code);
                    //continue;
                }
            }
            return model;
        }
    }
}
