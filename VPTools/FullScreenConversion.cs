using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisPinNet;

namespace VPTools
{
    //INCOMPLETE AND EXPERIMENTAL
    public class FullScreenConversion
    {
        public List<string> Log = new List<string>();

        public bool ConvertTable(VisualPinballTable VP)
        {
            //Get existing table inclination...

            BIFFFile F = VP.GetGameDataFile();
            if (F == null) return false;

            byte[] buf = F.GetChunk("INCL");
            if (buf != null)
            {
                float f = BitConverter.ToSingle(buf,0);
                Log.Add("Table Incline: " + f);
                if (Math.Abs(f) < 0.1)
                {
                    Log.Add("Table is ALREADY Full-Screen!");
                    return false;
                }
            }

            buf = BitConverter.GetBytes((float)270);
            F.SetChunk("ROTA", buf);

            buf = BitConverter.GetBytes((float)11);
            F.SetChunk("INCL", buf);

            buf = BitConverter.GetBytes((float)50);
            F.SetChunk("FOVX", buf);

            buf = BitConverter.GetBytes((float)70);
            F.SetChunk("LAYB", buf);

            buf = BitConverter.GetBytes((float)1.3);
            F.SetChunk("SCLX", buf);

            buf = BitConverter.GetBytes((float)1.9);
            F.SetChunk("SCLY", buf);

            buf = BitConverter.GetBytes((float)-450);
            F.SetChunk("OFFX", buf);

            buf = BitConverter.GetBytes((float)230);
            F.SetChunk("OFFY", buf);

            Log.Add("Set Rotation, Incline and FOV");

            float Width = BitConverter.ToSingle(F.GetChunk("RGHT"),0);
            float Height = BitConverter.ToSingle(F.GetChunk("BOTM"),0);

            Log.Add("Table Width / Height = " + Width + " x " + Height);

            VP.WriteFile(F.Buffer, "GameData");

            //OK - find me a GO.

            GameItemCollection GIC = VP.GetGameItems();
            BIFFFile B = GIC.GameItems["ScoreText"].GetFile();

            byte[] bxa = B.GetChunk("VER1");
            float V1A = BitConverter.ToSingle(bxa, 0);
            float V2A = BitConverter.ToSingle(bxa, 4);

            byte[] bxb = B.GetChunk("VER2");            
            float V1B = BitConverter.ToSingle(bxb, 0);
            float V2B = BitConverter.ToSingle(bxb, 4);

            B.SetChunk("VER2", bxa);

            Log.Add("Feature Not Complete");

            return true;
        }
    }
}
