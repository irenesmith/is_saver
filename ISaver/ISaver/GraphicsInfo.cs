using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ISaver
{
    public class GraphicsInfo
    {
        public Graphics Context { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public GraphicsInfo(Graphics context, int width, int height)
        {
            Context = context;
            Width = width;
            Height = height;
        }
    }
}
