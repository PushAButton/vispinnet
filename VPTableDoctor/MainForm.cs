using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace VPTableDoctor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            int q = 0;
        }

        public List<string> FilesToProcess = new List<string>();

        public enum DMDAct { all, rotation, none };
        public DMDAct DMDAction = DMDAct.all;

        private void MainForm_Load(object sender, EventArgs e)
        {
            DMDOption.SelectedIndex = 0;
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.CheckFileExists = true;
            Dlg.CheckPathExists = true;
            Dlg.DefaultExt = "vpt";
            Dlg.Filter = "Visual Pinball 9 (*.vpt)|*.vpt";
            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                string[] FNs = Dlg.FileNames;
                foreach (string F in FNs)
                {
                    FilesToProcess.Add(F);
                }
            }

            UpdateList();
        }

        void UpdateList()
        {
            FileListBox.Items.Clear();
            foreach (string S in FilesToProcess)
            {
                string Sx = Path.GetFileNameWithoutExtension(S);
                FileListBox.Items.Add(Sx);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DMDOption.SelectedIndex == 0)
                DMDAction = DMDAct.all;

            if (DMDOption.SelectedIndex == 1)
                DMDAction = DMDAct.rotation;

            if (DMDOption.SelectedIndex == 2)
                DMDAction = DMDAct.none;

            Processing P = new Processing();
            P.Settings = this;
            P.ShowDialog();
        }

        private void FolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog Dlg = new FolderBrowserDialog();
            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                string[] Files = Directory.GetFiles(Dlg.SelectedPath);
                foreach (string Fl in Files)
                {
                    if (Fl[0] == '.') continue;
                    string Ext = Path.GetExtension(Fl);
                    if (Ext.ToLower() == ".vpt")
                    {
                        FilesToProcess.Add(Fl);
                    }
                }
                UpdateList();
            }
        }
    }
}
