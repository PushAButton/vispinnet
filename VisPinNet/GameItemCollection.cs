using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPinNet
{
    public class GameItemCollection
    {
        public Dictionary<string, GameItem> GameItems = new Dictionary<string, GameItem>();

        public GameItemCollection(VisualPinballTable BPF)
        {
            int Unnamed = 1;

            int Ttl = Int32.Parse(BPF.Properties["Total Objects"]);
            for(int x=1;x< Ttl;x++)
            {
                GameItem GI = new GameItem(x, BPF);
                if (GI.Name == "")
                {
                    GI.Name = "Unnamed" + Unnamed;
                    Unnamed++;
                }
                GameItems.Add(GI.Name, GI);
            }
        }
    }
}
