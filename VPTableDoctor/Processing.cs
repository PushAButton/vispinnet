using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using VPTools;
using VisPinNet;
using System.IO;

namespace VPTableDoctor
{
    public partial class Processing : Form
    {
        public MainForm Settings;
        public Processing()
        {
            InitializeComponent();
        }

        Thread Thd = null;

        private void Processing_Load(object sender, EventArgs e)
        {
            Thd = new Thread(new ThreadStart(MainThread));
            Thd.Start();
            ProgBar.Maximum = Settings.FilesToProcess.Count;
        }

        void Progress(string msg, int indx)
        {
            ProgBar.Value = indx;
            label1.Text = msg;

            if (msg == "Complete")
            {
                button1.Text = "Done";
            }
        }

        void Log(string msg)
        {
            LogBox.Items.Add(msg);
        }

        delegate void UpdateProgress(string Msg, int indx);
        delegate void UpdateLog(string Msg);

        private void MainThread()
        {
            int indx = 0;
            object[] args = new object[2];
            object[] logger = new object[1];

            foreach (string S in Settings.FilesToProcess)
            {
                VisualPinballTable VPT = new VisualPinballTable();
                VPT.Load(S);
                
                args[0] = "Loaded " + VPT.Name;
                args[1] = indx;
                Invoke(new UpdateProgress(Progress), args);

                Toolbox T = new Toolbox(VPT);

                if (Settings.B2S.Checked == true)
                {
                    T.EnableB2S();
                }

                if (Settings.DMDAction == MainForm.DMDAct.all)
                {
                    T.ClearDMDSettings();
                }

                if (Settings.DMDAction == MainForm.DMDAct.rotation)
                {
                    T.ClearDMDRotation();
                }                

                if (Settings.ScriptClean.Checked == true)
                    T.ControllerExit();

                if (Settings.FS.Checked == true)
                {
                    FullScreenConversion FSC = new FullScreenConversion();
                    if (FSC.ConvertTable(VPT) == true)
                    {
                        
                    }
                    logger[0] = "Full Screen Conversion on " + VPT.Name;
                    Invoke(new UpdateLog(Log), logger);

                    foreach (string Sq in FSC.Log)
                    {
                        logger[0] = "  " + Sq;
                        Invoke(new UpdateLog(Log), logger);
                    }
                }

                foreach (string Sx in T.Log)
                {
                    logger[0] = Sx + " on " + VPT.Name;
                    Invoke(new UpdateLog(Log), logger);
                }


                if (T.ChangesMade == true)
                {
                    string bf = S.Replace(".vpt", ".bak");
                    if (!File.Exists(bf))
                    {
                        File.Copy(S, bf);
                    }
                    bf = S.Replace(".vpt", "_new.vpt");
                    VPT.Save(bf);
                    File.Delete(S);
                    File.Move(bf, S);
                }
                indx++;
            }
            
            args[0] = "Complete";
            args[1] = indx;
            Invoke(new UpdateProgress(Progress), args);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
