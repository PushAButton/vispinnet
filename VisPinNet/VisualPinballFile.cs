using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMcdf;
using System.Security.Cryptography;
using HashLib;

namespace VisPinNet
{
    //This will be used to store individual table elements (GameItems, Sounds etc.)
    public class TableElement
    {
        public string Name;
        public enum ElementType { image, sound, other };
        public ElementType Style;
    }

    //Used to manipulate the table script
    public class TableScript : VBAScript
    {
        
    }

    //Represents a single pinball table
    public class VisualPinballTable
    {
        List<TableElement> Elements = new List<TableElement>();

        public string Name = "";
        public string Version = "";
        public string Author = "";
        public string Description = "";
        public string ROM = "";
        public string Controller = "";
        public string ControllerVar = "";
        public string ROMNameVar = "";

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

            GetScriptDetails();

            if (ROMNameVar == "")
            {
                ROMNameVar = "\"" + ROM + "\"";
            }
        }

        void GetScriptDetails()
        {
            //Find out the name of the controller, AND the variable its stored in.
            VBALine[] L = Script.GetLinesContaining("CreateObject");
            foreach(VBALine Ln in L)
            {
                if (Ln.Tokens[0] == "'")
                {
                    continue;
                }

                Controller = Ln.Tokens[Ln.TokenID + 1];
                ControllerVar = Ln.Tokens[1];
            }

            //OK - grab the ROM name
            L = Script.GetLinesContaining(".GameName");
            foreach (VBALine Ln in L)
            {
                if (Ln.Tokens[0] == "'")
                {
                    continue;
                }

                ROM = Ln.Tokens[Ln.TokenID + 2];
                if (ROM[0] != '"')
                {
                    ROMNameVar = ROM;
                    //Lookup this value...
                    ROM = Script.GetConstantValue(ROM).Replace("\"","");                    
                }
            }         
        }

        //Used to interpret the BIFF chunks found in the GameData file
        bool BlowChunk(string Code, byte[] Content)
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
                case "SEDT":
                    Convert = "Total Objects";
                    Value = BitConverter.ToInt32(Content, 0).ToString();
                    break;
                case "SSND":
                    Convert = "Total Sounds";
                    Value = BitConverter.ToInt32(Content, 0).ToString();
                    break;
                case "SIMG":
                    Convert = "Total Images";
                    Value = BitConverter.ToInt32(Content, 0).ToString();
                    break;
                case "SFNT":
                    Convert = "Total Fonts";
                    Value = BitConverter.ToInt32(Content, 0).ToString();
                    break;
                case "SCOL":
                    Convert = "Total Collections";
                    Value = BitConverter.ToInt32(Content, 0).ToString();
                    break;

            }
            if (Convert != "")
            {
                Properties.Add(Convert, Value);
            }
            return true;
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
            Strm.SetData(Content);                   
        }

        //Writes binary to storage
        void WriteBinaryFile(byte[] b, CFItem Itm)
        {
            CFStream Strm = (CFStream)Itm;
            Strm.SetData(b);            
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
                Strm.SetData(Buf);                             
            }
        }

        //Variables used in hashing.
        HashAlgorithm Hasher;
        List<byte> HashData = new List<byte>();

        void HashRawFile(string Name, ref HashAlgorithm Ha)
        {
            IList<CFItem> Lst = CF.GetAllNamedEntries(Name);
            foreach (CFItem Itm in Lst)
            {
                byte[] buf = GetFile(Itm);
                //Hasher.TransformBlock(buf, 0, buf.Length, null, 0);
                HashData.AddRange(buf);                
            }                        
        }

        bool HashChunk(String S, byte[] Val)
        {
            byte[] b = Encoding.ASCII.GetBytes(S);
            HashData.AddRange(b);            
            if (S == "FONT")
                return true;
            HashData.AddRange(Val);
            return true;
        }

        void HashBIFFFile(string Name, ref HashAlgorithm Ha)
        {
            IList<CFItem> Lst = CF.GetAllNamedEntries(Name);
            foreach (CFItem Itm in Lst)
            {
                BIFFFile B = GetBIFF(Itm);                
                B.IterateChunks(HashChunk);
            }
        }

        //Calculates the MD5 hash for the VPT file.
        //Note that order is important.

        //In BIFF files, the chunk length is NOT added to the hash, although the chunk
        //four-letter code and the contents of the chunk ARE.    
            
        void CalculateHash()
        {
            //MD5 md5 = new MD5CryptoServiceProvider();            
            //Hasher = md5;

            byte[] bf = Encoding.ASCII.GetBytes("Visual Pinball");            
            HashData.AddRange(bf);

            HashRawFile("Version",ref Hasher);
            HashRawFile("TableName", ref Hasher);
            HashRawFile("AuthorName", ref Hasher);
            HashRawFile("TableVersion", ref Hasher);
            HashRawFile("ReleaseDate", ref Hasher);
            HashRawFile("AuthorEmail", ref Hasher);
            HashRawFile("AuthorWebSite", ref Hasher);
            HashRawFile("TableBlurb", ref Hasher);
            HashRawFile("TableDescription", ref Hasher);
            HashRawFile("TableRules", ref Hasher);
            HashRawFile("Screenshot", ref Hasher);
           
            HashBIFFFile("CustomInfoTags", ref Hasher);
            HashBIFFFile("GameData", ref Hasher);

            //Hasher.TransformFinalBlock(new byte[0], 0, 0);

            int Ttl = Int32.Parse(Properties["Total Objects"]);
            for (int x= 0;x< Ttl;x++)
            {
                IList<CFItem> Lstx = CF.GetAllNamedEntries("GameItem" + x);
                foreach (CFItem Itm in Lstx)
                {
                    BIFFFile F = GetBIFF(Itm);
                    
                    byte[] NewBuffer = new byte[F.Buffer.Length - 4];
                    Buffer.BlockCopy(F.Buffer, 4, NewBuffer, 0, NewBuffer.Length);
                    F.Buffer = NewBuffer;
                    F.IterateChunks(HashChunk);

                    //Console.WriteLine("   Hashed GameItem BFF File: " + Itm.Name);
                }
            }

            Ttl = Int32.Parse(Properties["Total Collections"]);
            for (int x = 0; x < Ttl; x++)
            {
                IList<CFItem> Lstx = CF.GetAllNamedEntries("Collection" + x);
                foreach (CFItem Itm in Lstx)
                {
                    BIFFFile F = GetBIFF(Itm);
                    
                    F.IterateChunks(HashChunk);

                    //Console.WriteLine("   Hashed GameItem BFF File: " + Itm.Name);
                }
            }

            var hash = HashFactory.Crypto.CreateMD2();
            var result = hash.ComputeBytes(HashData.ToArray());

            /*System.IO.BinaryWriter Wri = new System.IO.BinaryWriter(System.IO.File.Create("c:\\tmp\\hashb.bin"));
            Wri.Write(HashData.ToArray(), 0, HashData.Count);
            Wri.Flush();
            Wri.Close();*/

            MD5Hash = result.GetBytes();

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
            ModifyScript();   
            CalculateHash();          
            CF.Save(Filename);
            CF.Close();
        }

    }
}
