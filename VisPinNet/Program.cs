using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPT;

namespace VPTScriptPatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            //Open a Visual Pinball File
            VisualPinballTable VP = new VisualPinballTable();
            VP.Load("Original.vpt");
            Console.WriteLine("Opened VP Table '" + VP.FullName + "'");
            Console.WriteLine("Original Hash: " + VP.CalculatedHash);
            
            VP.Script.RemoveLine("\"dmd_pos_x\"");
            VP.Script.RemoveLine("\"dmd_pos_y\"");
            VP.Script.RemoveLine("\"dmd_width_x\"");
            VP.Script.RemoveLine("\"dmd_width_y\"");
            VP.Script.RemoveLine("\"rol\"");

            VP.Script.Substitute("VPinMAME.Controller", "B2S.Server");

            VP.Rename("My New Table");

            VP.Save("Output.vpt");
            Console.WriteLine("Saved VP Table With Hash: " + VP.CalculatedHash);
            VP.Close();

            //Console.WriteLine(VP.Script.Script);

            string VV = Console.ReadLine();
        }
    }
}
