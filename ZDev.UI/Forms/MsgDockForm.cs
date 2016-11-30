using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZCompileCore.Reports;

namespace ZDev.Forms
{
    public partial class MsgDockForm : DockFormBase
    {
        public MsgDockForm()
        {
            InitializeComponent();
        }

        public void ResetConsoles()
        {
            //outputConsole.Text = string.Empty;
            //errorConsole.Text = string.Empty;
            this.padConsole.Text = string.Empty;
            this.errorlistView.Items.Clear();
        }

        public void ShowErrors(ProjectCompileResult result)
        {      
            if (result.Errors.Count > 0)
            {
                foreach (var error in result.Errors)
                {
                    ListViewItem item = new ListViewItem();
                    //item.SubItems.Add("错误");
                    item.SubItems.Add(error.FileName);
                    item.SubItems.Add(error.Line.ToString());
                    item.SubItems.Add(error.Col.ToString());
                    item.SubItems.Add(error.Text);
                    item.ImageIndex = 0;
                    errorlistView.Items.Add(item);
                }
                this.msgTab.SelectedTab = this.errorTabPage;
            }

            if (result.Warnings.Count > 0)
            {
                foreach (var error in result.Warnings)
                {
                    ListViewItem item = new ListViewItem();
                    //item.SubItems.Add("警告");
                    item.SubItems.Add(error.FileName);
                    item.SubItems.Add(error.Line.ToString());
                    item.SubItems.Add(error.Col.ToString());
                    item.SubItems.Add(error.Text);
                    item.ImageIndex = 1;
                    errorlistView.Items.Add(item);
                }
                //foreach (var str in result.Warnings)
                //{
                //    outputConsole.Text += str.Text + "\r\n";
                //}
                this.msgTab.SelectedTab = this.errorTabPage;
            }
        }
    }
}
