using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecApp
{
    public class MySpisok
    {
        private string[] spis;

        public string[] Spis => spis;
        
        public MySpisok()
        {
            spis = new string[100];
            for (int i = 0; i < 100; i++) spis[i] = "test" + i;
            spis[0] = "WhatSize";
            spis[1] = "Finger123";
            spis[2] = "PointerLog";
            spis[3] = "Finger45";
            spis[4] = "Whirligig";
            spis[5] = "Piano";
            spis[6] = "ManipulTracker";
            spis[7] = "FlickAndBounce";
            spis[8] = "XYSlider";
            spis[9] = "CenteredTransforms";
            spis[10] = "DialSketch";
        }
    }
}
