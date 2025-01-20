﻿using System.Drawing;

namespace Timspect.Core.Graphics
{
    public struct Pixel
    {
        public Color Color;
        public int Index;

        public Pixel(Color color, int index)
        {
            Color = color;
            Index = index;
        }

        public Pixel(Color color)
        {
            Color = color;
            Index = -1;
        }

        public Pixel(int index)
        {
            Color = Color.FromArgb(0);
            Index = index;
        }
    }
}
