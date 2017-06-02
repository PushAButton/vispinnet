using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisPinNet;

namespace VPTools
{
    public class Toolbox
    {

        public bool ChangesMade {  get { return m_ChangesMade; } }

        public Toolbox(VisualPinballTable Tbl)
        {
            VP = Tbl;
        }

        bool m_ChangesMade = false;

        GameItemCollection GIC = null;            

        public List<string> Log = new List<string>();
        public bool verbose = false;

        VisualPinballTable VP;

        bool ChangeControllerSetting(string name, string value)
        {
            if (VP.Controller == "") return false;

            //Check to see if the controller setting already exists...
            VBALine[] Lines = VP.Script.GetLinesContaining("." + name);
            foreach (VBALine L in Lines)
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
                            if (Tmp != VP.Script.Script)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        string NewContent = VP.ControllerVar + "." + name + " = " + value;
                        if (value == "") NewContent = "";
                        VP.Script.Script = VP.Script.Script.Substring(0, L.StartPos) + NewContent + VP.Script.Script.Substring(L.EndPos);
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            if (value != "")
            {
                if (Lines.Length == 0)
                {
                    //There's no existing line - I'll have to add my own to INIT.

                    int ps = VP.Script.Script.IndexOf("_Init");
                    if (ps == -1)
                    {
                        //This table doesn't have an init sub - make one.
                        VP.Script.Script += "\r\nSub Table1_Init\r\n   With " + VP.ControllerVar + "\r\n      ." + name + " = " + value;
                        return true;
                    }
                    else
                    {
                        //Add the call at the end of the init.
                        int xs = VP.Script.Script.IndexOf("End Sub", ps);
                        VP.Script.Script = VP.Script.Script.Substring(0, xs) + "\r\n" + VP.ControllerVar + "." + name + " = " + value + "\r\n" + VP.Script.Script.Substring(xs);
                        return true;
                    }
                }
            }
            return false;
        }

        public void RenameTable(string S)
        {
            VP.Rename(S);
            m_ChangesMade = true;
        }

        public void ChangeRom(string S)
        {
            //Grab the line where the game name is set...
            VP.Script.Substitute("\"" + VP.ROM + "\"", "\"" + S + "\"");
            m_ChangesMade = true;
        }

        public void ClearDMDSettings()
        {
            bool Changed = false;
            if (ChangeControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"rol\")", "")) Changed = true;
            if (ChangeControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"dmd_pos_x\")", "")) Changed = true;
            if (ChangeControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"dmd_pos_y\")", "")) Changed = true;
            if (ChangeControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"dmd_width\")", "")) Changed = true;
            if (ChangeControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"dmd_height\")", "")) Changed = true;
            if (Changed == true)
            {
                Log.Add("Cleaned DMD Settings");
                m_ChangesMade = true;
            }
        }

        public void ClearDMDRotation()
        {
            bool Changed = false;
            if (ChangeControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"rol\")", "")) Changed = true;
            if (ChangeControllerSetting("Games(" + VP.ROMNameVar + ").Settings.Value(\"ror\")", "")) Changed = true;
            if (Changed == true)
            {
                m_ChangesMade = true;
                Log.Add("Cleaned DMD Rotation Settings");
            }
            }

        public void ControllerExit()
        {
            if (VP.Controller != "")
            {
                VBALine Ln = VP.Script.GetLineContaining("Table1_Exit");
                if (Ln == null)
                {
                    VP.Script.Script += "\r\nSub Table1_Exit\r\n    " + VP.ControllerVar + ".Stop\r\nEnd Sub\r\n";
                    Log.Add("Added New Table Exit Handler");
                }
                else
                {
                    if (VP.Script.GetLineContaining(VP.ControllerVar + ".Stop") == null)
                    {
                        int n = VP.Script.Script.IndexOf("End Sub", Ln.EndPos);
                        if (n != -1)
                        {
                            VP.Script.Script = VP.Script.Script.Substring(0, n) + "\r\n   " + VP.ControllerVar + ".Stop\r\n" + VP.Script.Script.Substring(n);
                            Log.Add("Updated Existing Exit Handler");
                        }
                    }
                    else
                    {
                        Log.Add("Table Already Closes the Controller");
                    }
                }
                m_ChangesMade = true;
            }
            else
            {
                Log.Add("No Exit Function - Table Has No Controller");
            }
        }

        public void HideDMD()
        {
            if (!VP.Script.Substitute(".Hidden = 0", ".Hidden = 1"))
            {
                Log.Add("Updated Existing Exit Handler");
            }
            m_ChangesMade = true;
        }

        public void ShowDMD()
        {
            if (VP.Script.Substitute(".Hidden = 1", ".Hidden = 0"))
            {
                Log.Add("DMD Unhidden");
                m_ChangesMade = true;
            }
            else
            {
                Log.Add("The DMD on this machine was not hidden.");
            }
        }

        public void EnableB2S()
        {
            if (VP.Script.Substitute("VPinMAME.Controller", "B2S.Server"))
            {
                Log.Add("B2S Backglass Server Enabled");
                VP.Controller = "B2S.Server";
            }
            else
            {
                Log.Add("No Controller or B2S Already Enabled");
            }            
            m_ChangesMade = true;
        }

        public void EnableMechanicalPlunger()
        {
            if (GIC == null) GIC = VP.GetGameItems();

            bool any = false;
            foreach (KeyValuePair<string, GameItem> KV in GIC.GameItems)
            {                
                if (KV.Value.Type == GameItem.TypeName.Plunger)
                {
                    any = true;
                    Console.WriteLine("GameItem #" + KV.Value.ID + " is a Plunger!");
                    BIFFFile B = KV.Value.GetFile();
                    byte[] buf = B.GetChunk("MECH");
                    if (buf == null)
                    {
                        Log.Add("WARNING: Unable to enable mechanical plunger - please upgrade this table to VP9+");
                    }
                    else
                    {
                        B.SetChunk("MECH", BitConverter.GetBytes((int)1));
                        KV.Value.Save(B);
                        m_ChangesMade = true;
                        Log.Add("Mechanical Plunger Enabled");
                    }
                }
            }

            if (any == false)
            {
                Log.Add("This table does not include a plunger.");
            }
        }
    }
}
