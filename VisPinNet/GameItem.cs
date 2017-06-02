using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPinNet
{
    public class GameItem
    {
        VisualPinballTable Table = null;
        public int ID = 0;
        public string Name = "";

        public int TypeID = 0;
        public enum TypeName { Flipper, Gate, Light, Plunger, Primitive, Wall, Bumper, Unknown,Ramp,Kicker,Text,Timer,Decal };
        public TypeName Type = TypeName.Unknown;

        //10 = Gate
        //1 = Flipper
        //7 = Light

        public BIFFFile GetFile()
        {
            BIFFFile B = Table.GetBIFFFile("GameItem" + ID);

            byte[] tmp = new byte[B.Buffer.Length - 4];
            Buffer.BlockCopy(B.Buffer, 4, tmp, 0, tmp.Length);
            B.Buffer = tmp;

            return B;
        }

        public bool Save(BIFFFile f)
        {
            byte[] buf = new byte[f.Buffer.Length + 4];
            Buffer.BlockCopy(f.Buffer, 0, buf, 4, f.Buffer.Length);
            Buffer.BlockCopy(BitConverter.GetBytes((int)TypeID), 0, buf, 0, 4);
            Table.WriteFile(buf, "GameItem" + ID);
            return true;
        }

        public GameItem(int TableID, VisualPinballTable Tbl)
        {
            ID = TableID;
            Table = Tbl;

            //Load Name...
            BIFFFile Fl = Table.GetBIFFFile("GameItem" + ID.ToString());
            if (Fl == null)
            {
                Name = "[UNKNOWN]";
                return;
            }

            //First, there's a LONG identifying the TYPE of game object...
            int Typ = BitConverter.ToInt32(Fl.Buffer, 0);
            TypeID = Typ;
            switch(Typ)
            {
                case 1:
                    Type = TypeName.Flipper;
                    break;
                case 9:
                    Type = TypeName.Decal;
                    break;
                case 0:
                    Type = TypeName.Wall;
                    break;
                case 10:
                    Type = TypeName.Gate;
                    break;
                case 7:
                    Type = TypeName.Light;
                    break;
                case 5:
                    Type = TypeName.Bumper;
                    break;
                case 8:
                    Type = TypeName.Kicker;
                    break;
                case 4:
                    Type = TypeName.Text;
                    break;
                case 3:
                    Type = TypeName.Plunger;
                    break;
                case 2:
                    Type = TypeName.Timer;
                    break;
                case 12:
                    Type = TypeName.Ramp;
                    break;
            }

            byte[] tmp = new byte[Fl.Buffer.Length - 4];
            Buffer.BlockCopy(Fl.Buffer, 4, tmp, 0, tmp.Length);
            Fl.Buffer = tmp;

            byte[] Contents = Fl.GetChunk("NAME");
            Name = Encoding.Unicode.GetString(Contents,4,Contents.Length-4);
            Fl = null;
        }        
    }
}
