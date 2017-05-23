using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VisPinNet
{
    public class VBALine
    {
        public int StartPos;
        public int EndPos;
        public int TokenID = -1;

        public string[] Tokens;
    }

    public class VBAScript
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

        public static List<String> SplitQuoted(string Str)
        {
            List<string> Out = new List<string>();
            MatchCollection parts = Regex.Matches(Str, @"[\""].+?[\""]|[^ ]+");
            foreach(Match M in parts)
            {
                string S = M.Value;
                S = S.Replace("\r", "");
                S = S.Replace("\n", "");
                S = S.Replace("\t", "");
                //S = S.Replace("\"", "");

                string[] Bits = S.Split(new char[] { '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string s in Bits)
                {
                    if (s != "")
                        Out.Add(s);
                }                
            }
            return Out;
        }

        public VBALine GetLineContaining(string S)
        {
            int i = Script.IndexOf(S);
            if (i == -1) return null;

            VBALine SL = new VBALine();

            //Look backwards to see if there's a CR/LF nearby...
            int st = Script.LastIndexOf('\r', i);
            int en = Script.IndexOf('\r', i);
            if (en == -1) en = S.Length;

            SL.StartPos = st;
            SL.EndPos = en;

            //Construct a list of tokens...                       
            string Base = Script.Substring(st, en-st);
            List<string> Tokens = SplitQuoted(Base);
            SL.Tokens = Tokens.ToArray();
            return SL;
        }

        public VBALine[] GetLinesContaining(string S)
        {
            List<VBALine> Lines = new List<VBALine>();

            int prg = 0;

            int i = Script.IndexOf(S,prg);
            while (i != -1)
            {                
                VBALine SL = new VBALine();

                //Look backwards to see if there's a CR/LF nearby...
                int st = Script.LastIndexOf('\r', i);
                int en = Script.IndexOf('\r', i);
                if (en == -1) en = S.Length;

                SL.StartPos = st;
                SL.EndPos = en;

                //Construct a list of tokens...                       
                string Base = Script.Substring(st, en - st);
                List<string> Tokens = SplitQuoted(Base);
                SL.Tokens = Tokens.ToArray();

                for(int x=0;x<SL.Tokens.Length;x++)
                {
                    if (SL.Tokens[x].IndexOf(S) != -1)
                    {
                        SL.TokenID = x;
                        break;
                    }
                }

                prg = en + 1;
                i = Script.IndexOf(S, prg);
                Lines.Add(SL);
            }

            return Lines.ToArray();
        }

        public string GetConstantValue(string Name)
        {
            VBALine[] Lines = GetLinesContaining(Name);
            foreach(VBALine L in Lines)
            {
                if (L.Tokens[0] == "Const")
                {
                    if (L.Tokens[1] == Name)
                    {
                        return L.Tokens[3];
                    }
                }
            }
            return "";
        }

    }
}
