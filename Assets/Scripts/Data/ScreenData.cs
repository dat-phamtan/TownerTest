using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Data
{
    public class ScreenData
    {
        public int ScreenHeight { get; set; }
        public int ScreenWidth { get; set; }
        public ScreenData(int width, int height)
        {
            ScreenHeight = height;
            ScreenWidth = width;
        }
    }
}
