using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDev.Schema.Models.TKTXM
{
    public class TKTXMFileModel
    {
        public string PackageName { get; set; }
        public string GenerateType { get; set; }
        public List<string> SrcFiles { get; set; }
        public string Entry { get; set; }

        public TKTXMFileModel()
        {
            SrcFiles = new List<string>();
        }
    }
}
