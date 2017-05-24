using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisPinNet;
using NDesk.Options;
using System.IO;

namespace VPTable
{
    class Program
    {        
        static void Main(string[] args)
        {
            VPTweak VP = new VPTweak();
            VP.Run(args);
        }
    }
}
