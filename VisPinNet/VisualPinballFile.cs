using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMcdf;
using System.Security.Cryptography;

namespace VPT
{
    //This will be used to store individual table elements (GameItems, Sounds etc.)
    public class TableElement
    {
        public string Name;
        public enum ElementType { image, sound, other };
        public ElementType Style;
    }

    //Used to manipulate the table script
    public class TableScript
    {
        public string Script;
        public void RemoveLine(string S)
        {
            int i = Script.IndexOf(S);
            if (i != -1)
            {
                int v = Script.LastIndexOf('\r', i);
                if (v != -1)
                {
                    int n = Script.IndexOf('\r', i);
                    string Line = Script.Substring(v, n - v);
                    //Console.WriteLine("Found Line: " + Line);
                    bool Commented = false;
                    for (int x = 0; x < Line.Length; x++)
                    {
                        if ((Line[x] == '\r') || (Line[x] == '\n') || (Line[x] == ' ') || (Line[x] == '\t')) continue;
                        if (Line[x] == '\'')
                        {
                            Commented = true;
                            break;
                        }
                        break;
                    }

                    while ((Script[v] == '\r') || (Script[v] == '\n') || (Script[v] == ' '))
                        v++;

                    if (Commented == false)
                    {
                        Script = Script.Substring(0, v) + "'" + Script.Substring(v);
                    }
                }
            }
        }

        public void Substitute(string S, string N)
        {
            Script = Script.Replace(S, N);
        }
    }

    //Represents a single pinball table
    public class VisualPinballTable
    {
        List<TableElement> Elements = new List<TableElement>();

        public string Name = "";
        public string Version = "";
        public string Author = "";
        public string Description = "";
        public TableScript Script;
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        private float m_VPVersion = 0;

        CompoundFile CF = null;

        //Loads a table from a VPT file
        public void Load(string Filename)
        {
            CF = new CompoundFile(Filename);

            IList<CFItem> Lst = CF.GetAllNamedEntries("TableName");
            foreach (CFItem Itm in Lst)
            {
                Name = ReadTextFileUnicode(Itm.Size, Itm);
            }
            Lst = CF.GetAllNamedEntries("AuthorName");
            foreach (CFItem Itm in Lst)
            {
                Author = ReadTextFileUnicode(Itm.Size, Itm);
            }
            Lst = CF.GetAllNamedEntries("TableVersion");
            foreach (CFItem Itm in Lst)
            {
                Version = ReadTextFileUnicode(Itm.Size, Itm);
            }
            Lst = CF.GetAllNamedEntries("TableDescription");
            foreach (CFItem Itm in Lst)
            {
                Description = ReadTextFileUnicode(Itm.Size, Itm);
            }
            Lst = CF.GetAllNamedEntries("Version");
            foreach (CFItem Itm in Lst)
            {
                m_VPVersion = ReadIntBinary(Itm) / 100;
            }
            Lst = CF.GetAllNamedEntries("MAC");
            foreach (CFItem Itm in Lst)
            {
                MD5Hash = GetFile(Itm);
            }

            Lst = CF.GetAllNamedEntries("GameData");
            foreach (CFItem Itm in Lst)
            {
                BIFFFile F = GetBIFF(Itm);
                F.IterateChunks(BlowChunk);
            }

            Script = GetScript();
        }

        //Used to interpret the BIFF chunks found in the GameData file
        void BlowChunk(string Code, byte[] Content)
        {
            string Convert = "";
            string Value = "";

            switch(Code)
            {
                case "LEFT":
                    Convert = "Left";
                    Value = BitConverter.ToInt32(Content,0).ToString();
                    break;
                case "RGHT":
                    Convert = "Right";
                    Value = BitConverter.ToInt32(Content,0).ToString();
                    break;
                case "TOPX":
                    Convert = "Top";
                    Value = BitConverter.ToInt32(Content,0).ToString();
                    break;
                case "BOTM":
                    Convert = "Bottom";
                    Value = BitConverter.ToInt32(Content,0).ToString();
                    break;

            }
            if (Convert != "")
            {
                Properties.Add(Convert, Value);
            }
        }

        public float VPVersion { get { return m_VPVersion; } }

        //Gets a raw byte-stream from storage
        public byte[] GetFile(CFItem Itm)
        {
            byte[] buf = new byte[Itm.Size];
            CFStream Strm = (CFStream)Itm;
            Strm.Read(buf, 0, (int)Itm.Size);
            return buf;
        }

        //Gets a BIFF file from storage
        public BIFFFile GetBIFF(CFItem Itm)
        {
            byte[] buf = new byte[Itm.Size];
            CFStream Strm = (CFStream)Itm;
            Strm.Read(buf, 0, (int)Itm.Size);

            return new BIFFFile(buf);
        }

        //Loads the script from GameData
        public TableScript GetScript()
        {
            if (CF == null) return null;

            IList<CFItem> Lst = CF.GetAllNamedEntries("GameData");
            foreach (CFItem Itm in Lst)
            {                
                BIFFFile F = GetBIFF(Itm);
                string Code = Encoding.ASCII.GetString(F.GetChunk("CODE"));
                TableScript TS = new TableScript();
                TS.Script = Code;
                return TS;
            }

            return null;
        }

        //Returns the full table name
        public string FullName { get { return Name + " " + Version + " by " + Author; } }

        //Reads a unicode text file from storage (Table Name / Author etc.)
        string ReadTextFileUnicode(long Sz, CFItem Itm)
        {
            string S = "";
            CFStream Strm = (CFStream)Itm;
            byte[] buf = new byte[Sz];
            int n = Strm.Read(buf, 0, (int)Sz);
            S = Encoding.Unicode.GetString(buf, 0, (int)Sz);
            return S;
        }

        //Reads a single binary integer from storage (Version Info)
        int ReadIntBinary(CFItem Itm)
        {            
            CFStream Strm = (CFStream)Itm;
            byte[] buf = new byte[4];
            int n = Strm.Read(buf, 0, 4);
            return BitConverter.ToInt32(buf, 0);            
        }

        //Renames the table
        public void Rename(string S)
        {
            IList<CFItem> Lst = CF.GetAllNamedEntries("TableName");
            foreach (CFItem Itm in Lst)
            {                
                WriteUnicodeTextFile(S, Itm);
            }
        }

        //Closes the file
        public void Close()
        {
            CF.Close();
        }

        //Writes a unicode text file to storage (ie. renaming a table)
        void WriteUnicodeTextFile(string S, CFItem Itm)
        {
            CFStream Strm = (CFStream)Itm;
            byte[] Content = Encoding.Unicode.GetBytes(S);            
            Strm.Write(Content, 0);
            Strm.Resize(Content.Length);            
            //Strm.SetData(Content);
        }

        //Writes binary to storage
        void WriteBinaryFile(byte[] b, CFItem Itm)
        {
            CFStream Strm = (CFStream)Itm;            
            Strm.Write(b, 0);
            Strm.Resize(b.Length);
            //Strm.SetData(Content);
        }

        //Saves a modified script back to the GameData BIFF file.
        public void ModifyScript()
        {
            IList<CFItem> Lst = CF.GetAllNamedEntries("GameData");
            foreach (CFItem Itm in Lst)
            {
                BIFFFile F = GetBIFF(Itm);
                F.SetChunk("CODE", Encoding.ASCII.GetBytes(Script.Script));
                
                CFStream Strm = (CFStream)Itm;
                byte[] Buf = F.GetBuffer();
                Strm.Write(Buf, 0);
                Strm.Resize(Buf.Length);
            }
        }

        //Temporary class, used to determine the correct order for generating the
        //  MAC SHA hash.
        class OrderedItem : IComparable<OrderedItem>
        {
            public CFItem File;
            public int BaseScore;
            public string Name;

            public OrderedItem(CFItem I)
            {
                Name = I.Name;
                BaseScore = -1;
                File = I;
                switch (Name)
                {                    
                    case "Version":
                        BaseScore = 0;
                        break;
                    case "TableInfo":
                        BaseScore = 1;
                        break;
                    case "TableName":
                        BaseScore = 10;
                        break;
                    case "AuthorName":
                        BaseScore = 11;
                        break;
                    case "TableVersion":
                        BaseScore = 12;
                        break;
                    case "ReleaseDate":
                        BaseScore = 13;
                        break;
                    case "AuthorEmail":
                        BaseScore = 14;
                        break;
                    case "AuthorWebSite":
                        BaseScore = 15;
                        break;
                    case "TableBlurb":
                        BaseScore = 16;
                        break;
                    case "TableDescription":
                        BaseScore = 17;
                        break;
                    case "TableRules":
                        BaseScore = 18;
                        break;
                    case "CustomInfoTags":
                        BaseScore = 98;
                        break;
                    case "GameData":
                        BaseScore = 99;
                        break;
                }

                int ps = Name.IndexOf("GameItem");
                if (ps != -1) BaseScore = 100;

                ps = Name.IndexOf("Sound");
                if (ps != -1) BaseScore = 200;

                ps = Name.IndexOf("Image");
                if (ps != -1) BaseScore = 300;

                ps = Name.IndexOf("Font");
                if (ps != -1) BaseScore = 400;

                ps = Name.IndexOf("Collection");
                if (ps != -1) BaseScore = 500;
            }

            public int CompareTo(OrderedItem other)
            {
                int cmp = BaseScore.CompareTo(other.BaseScore);
                if (cmp != 0) return cmp;

                return Name.CompareTo(other.Name);
            }
        }

        //Variables used in hashing.
        HashAlgorithm Hasher;
        List<OrderedItem> HashOrder = null;

        //Called for every file - adds each file to the hashing order list.
        void EstablishHashOrder(CFItem item)
        {
            if (item.IsStream == false) return;
            if (item.Name == "MAC") return;

            if (item.Name.IndexOf("Sound") != -1)
            {
                //Sound files aren't BIFFs and aren't hashed.
                return;
            }

            HashOrder.Add(new OrderedItem(item));
        }

        /*void CalculateHashOnStoredFile(CFItem item)
        {
            if (item.IsStream == false) return;
            if (item.Name == "MAC") return;

            switch(item.Name)
            {
                case "TableName":                
                case "AuthorName":
                case "TableBlurb":
                case "TableRules":
                case "AuthorEmail":
                case "ReleaseDate":
                case "TableVersion":
                case "AuthorWebSite":
                case "TableDescription":
                case "Version":
                    byte[] buf = GetFile(item);
                    Hasher.TransformBlock(buf, 0, buf.Length, null,0);
                    return;                    
            }

            if (item.Name.IndexOf("Sound") != -1)
            {
                //Sound files aren't BIFFs and aren't hashed.
                return;
            }            

            BIFFFile F = GetBIFF(item);
            if (item.Name.IndexOf("GameItem") != -1)
            {
                //Gameitems have a rubbish Int32 at the start. Absorb it.
                Hasher.TransformBlock(F.Buffer, 0, 4, null, 0);
                byte[] NewBuffer = new byte[F.Buffer.Length - 4];
                Buffer.BlockCopy(F.Buffer, 4, NewBuffer, 0, NewBuffer.Length);
                F.Buffer = NewBuffer;
            }
            F.Hash(ref Hasher);
        }*/

        //Calculates the MD5 hash for the VPT file.
        //Note that order is important.
        //In BIFF files, the chunk length is NOT added to the hash, although the chunk
        //four-letter code and the contents of the chunk ARE.
        //TODO - not working
        void CalculateHash()
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Hasher = md5;

            byte[] bf = Encoding.ASCII.GetBytes("Visual Pinball");
            Hasher.TransformBlock(bf, 0, bf.Length, null, 0);

            HashOrder = new List<OrderedItem>();
            CF.RootStorage.VisitEntries(EstablishHashOrder,true);
            HashOrder.Sort();

            foreach(OrderedItem OI in HashOrder)
            {
                if (OI.BaseScore == -1)
                {
                    continue;
                }
                if (OI.BaseScore < 100)
                {
                    byte[] buf = GetFile(OI.File);
                    Hasher.TransformBlock(buf, 0, buf.Length,null,0);
                    continue;
                }

                BIFFFile F = GetBIFF(OI.File);

                if (OI.BaseScore == 100)
                {
                    Hasher.TransformBlock(F.Buffer, 0, 4, null, 0);
                    byte[] NewBuffer = new byte[F.Buffer.Length - 4];
                    Buffer.BlockCopy(F.Buffer, 4, NewBuffer, 0, NewBuffer.Length);
                    F.Buffer = NewBuffer;
                }

                F.Hash(ref Hasher);
            }

            Hasher.TransformFinalBlock(new byte[0], 0, 0);

            MD5Hash = Hasher.Hash;

            IList<CFItem> Lst = CF.GetAllNamedEntries("MAC");
            foreach (CFItem Itm in Lst)
            {
                WriteBinaryFile(MD5Hash, Itm);
            }

        }

        byte[] MD5Hash = null;
        public string CalculatedHash {
            get {
                if (MD5Hash == null) return "";
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < MD5Hash.Length; i++)
                {
                    sBuilder.Append(MD5Hash[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        public void Save(string Filename)
        {
            //ModifyScript is temporarily disabled while we figure out the damn hash.

            //ModifyScript();   
            CalculateHash();          
            CF.Save(Filename);
            CF.Close();
        }

    }
}
