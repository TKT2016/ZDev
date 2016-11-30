using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZDev.Schema.Models.TKTXM;
using ZDev.Schema.Parses;

namespace ZDev.Forms
{
    public partial class ProjectDockForm : DockFormBase
    {
        public ProjectDockForm()
        {
            InitializeComponent();
        }

        TKTXMParser parser = new TKTXMParser();

        public void ClearTree()
        {
            this.projTreeView.Nodes.Clear();
        }

        public TKTXMFileModel ShowClass(FileInfo fi)
        {
            TKTXMFileModel model = parser.Parse(fi);
            if(model!=null)
            {
                ShowClass(model, fi);
                return model;
            }
            //string code = File.ReadAllText(fi.FullName);
            //var tokens = scanner.Scan(code);
            //TKTXMFileModel tktclass = parser.Parse(tokens);
            //if (tktclass.NameModel == null)
            //{
            //    tktclass.NameModel = new TKTContentModel() { Content = Path.GetFileName(fi.FullName) };
            //}
            //ShowClass(model, fi);
            return null;
        }

        public void ShowClass(TKTXMFileModel tktclass, FileInfo fi)
        {
            ClearTree();
            if (tktclass == null) return;

            TreeNode xmNode = new TreeNode();
            xmNode.Text = fi.Name;
            xmNode.Tag = fi.FullName;
            projTreeView.Nodes.Add(xmNode);

            foreach (var pp in tktclass.SrcFiles)
            {
                TreeNode pnode = new TreeNode();
                pnode.Text = pp;
                pnode.Tag = Path.Combine(fi.Directory.FullName, pp);
                projTreeView.Nodes.Add(pnode);
            }
            projTreeView.ExpandAll();
        }

        private void projTreeView_DoubleClick(object sender, EventArgs e)
        {
            TreeNode node = this.projTreeView.SelectedNode;
            if (node.Tag != null)
            {
                string tmb = node.Tag as string;
                if (tmb != null)
                {
                    this.MainWindow.OpenFile(tmb,false);
                }
            }
        }
    }
}
