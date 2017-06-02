using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisPinNet;
using VPTools;

namespace VPTable
{
    class VPTweak
    {
        static void ShowHelp()
        {
            Console.WriteLine("Visual Pinball Table Tweaker");
            Console.WriteLine("Originally Developed by Steven Harding / Push-A-Button Software");
            Console.WriteLine("-----");
            Console.WriteLine("Parameters:");
            Console.WriteLine(" -i <filename> / --in <filename>: Specify the VPT file to load.");
            Console.WriteLine(" -o <filename> / --out <filename>: The name of the VPT file to save to.");
            Console.WriteLine(" -c <filename> / --cab <filename>: An XML file that describes your cabinet.");
            Console.WriteLine("");
            Console.WriteLine("Checking Metadata:");
            Console.WriteLine(" -show [n|r]: Show one or more VPT properties.");
            Console.WriteLine("     n = Name");
            Console.WriteLine("     r = ROM");
            Console.WriteLine("     You can combine these values - --show nr will display both name AND ROM.");
            Console.WriteLine("");
            Console.WriteLine("Changing Metadata:");
            Console.WriteLine(" -setname <name>: Sets the name of the VPT table.");
            Console.WriteLine(" -setrom <romname>: Updates the ROM used by the table.");
            Console.WriteLine("");
            Console.WriteLine("Customisation:");
            Console.WriteLine(" -enb2s: Enable a B2S backglass");
            Console.WriteLine(" -hidedmd: Hide the DMD (Dot-Matrix-Display)");
            Console.WriteLine(" -showdmd: Unhide the DMD (Dot-Matrix-Display)");
            Console.WriteLine(" -cleardmd: Remove lines adjusting the DMD");
            Console.WriteLine(" -cleanexit: Close the controller on table close");
            Console.WriteLine(" -cleanup: Includes 'cleardmd' and 'cleanup'");
        }


        Toolbox TBx = null;
        VisualPinballTable VP = null;

        public void Run(string[] args)
        {
            bool help = false;
            string infile = "";
            string outfile = "";
            string cabfile = "";

            bool enableb2s = false;
            bool hidedmd = false;
            bool cleardmd = false;
            bool showdmd = false;
            bool cleanexit = false;
            bool cleanup = false;
            bool verbose = false;
            bool enableplunger = true;

            bool GIEdit = false;

            string DisplayComponents = "nr";
            Dictionary<string, string> ControllerParams = new Dictionary<string, string>();

            Dictionary<string, string> SetComponents = new Dictionary<string, string>();

            OptionSet p = new OptionSet()
              .Add("v|verbose", delegate (string v) { verbose = true; })
              .Add("enb2s", delegate (string v) { enableb2s = true; })
              .Add("hidedmd", delegate (string v) { hidedmd = true; })
              .Add("showdmd", delegate (string v) { showdmd = true; })
              .Add("cleardmd", delegate (string v) { cleardmd = true; })
              .Add("cleanexit", delegate (string v) { cleanexit = true; })
              .Add("cleanup", delegate (string v) { cleardmd = true; cleanexit = true; })
              .Add("show", delegate (string v) { DisplayComponents = v; })
              .Add("h|?|help", delegate (string v) { help = v != null; })
              .Add("i|in=", delegate (string v) { infile = v; })
              .Add("c|cab=", delegate (string v) { cabfile = v; })
              .Add("o|out=", delegate (string v) { outfile = v; })
              .Add("plunger", delegate (string v) { enableplunger = true; })
              .Add("setname=", delegate (string v) { SetComponents.Add("name", v); })
              .Add("setrom=", delegate (string v) { SetComponents.Add("rom", v); });

            if (help == true)
            {
                ShowHelp();
                return;
            }

            List<string> extra = p.Parse(args);

            if (infile == "")
            {
                Console.WriteLine("You must include an input file name (-i <filename>).");
                return;
            }

            bool FileChanges = false;            

            VP = new VisualPinballTable();
            VP.Load(infile);

            TBx = new Toolbox(VP);

            if (verbose == true) Console.WriteLine("Opened VP Table '" + VP.FullName + "'");

            //Display Information
            foreach (char S in DisplayComponents)
            {
                if (S == 'n')
                    Console.WriteLine("Name: " + VP.Name);
                if (S == 'r')
                {
                    Console.WriteLine("ROM: " + VP.ROM);
                }
            }

            //Handle all of the SETTINGS
            foreach (KeyValuePair<string, string> KV in SetComponents)
            {
                if (KV.Key == "name")
                {
                    TBx.RenameTable(KV.Value);
                }
                if (KV.Key == "rom")
                {
                    TBx.ChangeRom(KV.Value);
                }
            }

            if ((cleardmd == true) || (cleanup == true))
            {
                TBx.ClearDMDSettings();
            }

            if ((cleanexit == true) || (cleanup == true))
            {
                TBx.ControllerExit();
            }

            if (hidedmd == true)
            {
                TBx.HideDMD();
            }

            if (showdmd == true)
            {
                TBx.ShowDMD();
            }

            if (enableb2s == true)
            {
                TBx.EnableB2S();
            }

            if (enableplunger == true)
            {
                TBx.EnableMechanicalPlunger();
            }

            foreach(string S in TBx.Log)
            {
                Console.WriteLine(S);
            }

            if (TBx.ChangesMade == true)
            {
                if (outfile != "")
                {
                    VP.Save(outfile);
                }
                else
                {
                    if (!File.Exists(infile + ".bak"))
                    {
                        File.Copy(infile, infile + ".bak");
                    }
                    VP.Save(infile);
                }
            }
            VP.Close();

            //string VV = Console.ReadLine();
        }
    }
}
