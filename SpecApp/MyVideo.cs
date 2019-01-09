using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecApp
{
    class MyVideo
    {
        private string[] mstr;
        public string[] MStr => mstr;
        public MyVideo()
        {
            mstr = new string[100];
            for (int i = 0; i < 100; i++) mstr[i] = "video" + i;
        }
    }
}
