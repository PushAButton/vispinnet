using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisPinNet;

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

        void ControllerSetting(string name, string value)
        {
            if (VP.Controller == "") return;

            //Check to see if the controller setting already exists...
            VBALine[] Lines = VP.Script.GetLinesContaining("." + name);            
            foreach(VBALine L in Lines)
            {
                //OK - this line already exists. Now, determine if this is part of a 'With' block...
                bool Commented = false;
                if (L.Tokens[0][0] == '\'')
                {
                    Commented = true;
                    L.Tokens[0] = L.Tokens[0].Substring(1);
                }
                if (L.Tokens[0][0] == '.')                
                {
                    //This is a 'With' block.
                    if (value == "")
                    {
                        //Make sure this block isn't already commented.
                        if (!Commented)
                        {
                            string Tmp = VP.Script.Script;
                            VP.Script.Script = VP.Script.Script.Substring(0, L.StartPos) + VP.Script.Script.Substring(L.EndPos);
                            if (Tmp == VP.Script.Script)
                            {
                                int qv = 0;
                            }
                        }
                    }
                    else
                    {                                                
                        string NewContent = VP.ControllerVar + "." + name + " = " + value;
                        if (value == "") NewContent = "";
                        VP.Script.Script = VP.Script.Script.Substring(0, L.StartPos) + NewContent + VP.Script.Script.Substring(L.EndPos);
                    }
                }
                else
                {
                                        
                }
            }
            if (Lines.Length == 0)
            {
                //There's no existing line - I'll have to add my own to INIT.

                int ps = VP.Script.Script.IndexOf("_Init");
                if (ps == -1)
                {
                    //This table doesn't have an init sub - make one.
                    VP.Script.Script += "\r\nSub Table1_Init\r\n   With " + VP.ControllerVar + "\r\n      ." + name + " = " + value;
                }
                else
                {
                    //Add the call at the end of the init.
                    int xs = VP.Script.Script.IndexOf("End Sub", ps);
                    VP.Script.Script = VP.Script.Script.Substring(0, xs) + "\r\n" + VP.ControllerVar + "." + name + " = " + value + "\r\n" + VP.Script.Script.Substring(xs);
                }
            }
        }

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
              .Add("setname=", delegate (string v) { SetComponents.Add("name", v); })
              .Add("setrom=", delegate (string v) { SetComponents.Add("rom", v); });

            if (help == true)
            {
                ShowHelp();
                return;
            }

            bool FileChanges = false;

            List<string> extra = p.Parse(args);

            VP = new VisualPinballTable();
            VP.Load(infile);

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
                    VP.Rename(KV.Value);
                    FileChanges = true;
                }
                if (KV.Key == "rom")
                {
                    //Grab the line where the game name is set...
                    VP.Script.Substitute("\"" + VP.ROM + "\"", "\"" + KV.Value + "\"");
                    FileChanges = true;
                }
            }

            if ((cleardmd == true) || (cleanup == true))
            {                
                ControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"rol\")", "");
                ControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"dmd_pos_x\")", "");
                ControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"dmd_pos_y\")", "");
                ControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"dmd_width\")", "");
                ControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"dmd_height\")", "");
                FileChanges = true;
            }

            if ((cleanexit == true) || (cleanup == true))
            {
                if (VP.Controller != "")
                {
                    VBALine Ln = VP.Script.GetLineContaining("Table1_Exit");
                    if (Ln == null)
                    {
                        VP.Script.Script += "\r\nSub Table1_Exit\r\n    " + VP.ControllerVar + ".Stop\r\nEnd Sub\r\n";
                    }
                    else
                    {
                        if (VP.Script.GetLineContaining(VP.Controller + ".Stop") == null)
                        {
                            int n = VP.Script.Script.IndexOf("End Sub", Ln.EndPos);
                            if (n != -1)
                            {
                                VP.Script.Script = VP.Script.Script.Substring(0, n) + "\r\n   " + VP.ControllerVar + ".Stop\r\n" + VP.Script.Script.Substring(n);
                            }
                        }
                    }
                    FileChanges = true;
                }
            }

            if (hidedmd == true)
            {
                if (!VP.Script.Substitute(".Hidden = 0", ".Hidden = 1"))
                {
                    //We need to add this line in the code.
                }
                FileChanges = true;
            }

            if (showdmd == true)
            {
                if (VP.Script.Substitute(".Hidden = 1", ".Hidden = 0")) FileChanges = true;
            }

            if (enableb2s == true)
            {
                VP.Script.Substitute("VPinMAME.Controller", "B2S.Server");
                VP.Controller = "B2S.Server";
                FileChanges = true;
            }

            if (FileChanges == true)
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
                if (verbose == true)
                {

                }


            }
            VP.Close();

            //string VV = Console.ReadLine();
        }
    }
}
