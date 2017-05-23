using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisPinNet;
using NDesk.Options;

namespace VPTable
{
    class Program
    {
        static void Main(string[] args)
        {
            string data = null;
            bool help = false;
            int verbose = 0;

            string infile = "";
            string outfile = "";
            string cabfile = "";
            
            bool enableb2s = false;
            bool hidedmd = false;
            bool cleardmd = false;
            bool showdmd = false;

            string DisplayComponents = "nr";
            Dictionary<string, string> SetComponents = new Dictionary<string, string>();

            OptionSet p = new OptionSet()
              .Add("v|verbose", delegate (string v) { if (v != null) ++verbose; })
              .Add("enb2s", delegate (string v) { enableb2s = true; })
              .Add("hidedmd", delegate (string v) { hidedmd = true; })
              .Add("showdmd", delegate (string v) { showdmd = true; })
              .Add("cleardmd", delegate (string v) { cleardmd = true; })
              .Add("show", delegate (string v) { /*DisplayComponents = v; */})
              .Add("h|?|help", delegate (string v) { help = v != null; })
              .Add("i|in=", delegate (string v) { infile = v; })
              .Add("c|cab=", delegate (string v) { cabfile = v; })
              .Add("o|out=", delegate (string v) { outfile = v; })
              .Add("setname=", delegate (string v) { SetComponents.Add("name", v); })
              .Add("setrom=", delegate (string v) { SetComponents.Add("rom", v); });

            List<string> extra = p.Parse(args);

            VisualPinballTable VP = new VisualPinballTable();
            VP.Load(infile);
            
            Console.WriteLine("Opened VP Table '" + VP.FullName + "'");
            Console.WriteLine("Original Hash: " + VP.CalculatedHash);

            foreach(char S in DisplayComponents)
            {
                if (S == 'n')
                    Console.WriteLine("Name: " + VP.Name);
                if (S == 'r')
                {                    
                    Console.WriteLine("ROM: " + VP.ROM);
                }
            }

            //Handle all of the SETTINGS
            foreach(KeyValuePair<string,string> KV in SetComponents)
            {
                if (KV.Key == "name")
                {
                    VP.Rename(KV.Value);
                }
                if (KV.Key == "rom")
                {
                    //Grab the line where the game name is set...
                    VP.Script.Substitute("\"" + VP.ROM + "\"", "\"" +KV.Value + "\"");
                }
            }

            if (cleardmd == true)
            {
                VP.Script.RemoveLine("\"dmd_pos_x\"");
                VP.Script.RemoveLine("\"dmd_pos_y\"");
                VP.Script.RemoveLine("\"dmd_width_x\"");
                VP.Script.RemoveLine("\"dmd_width_y\"");
                VP.Script.RemoveLine("\"rol\"");
            }

            if (hidedmd == true)
            {
                VP.Script.Substitute(".Hidden = 0", ".Hidden = 1");
            }

            if (showdmd == true)
            {
                VP.Script.Substitute(".Hidden = 1", ".Hidden = 0");
            }

            if (enableb2s == true)
            {
                VP.Script.Substitute("VPinMAME.Controller", "B2S.Server");
            }

            if (outfile != "")
            {
                VP.Save(outfile);
                Console.WriteLine("Saved VP Table With Hash: " + VP.CalculatedHash);
            }
            VP.Close();

            //string VV = Console.ReadLine();
        }
    }
}
