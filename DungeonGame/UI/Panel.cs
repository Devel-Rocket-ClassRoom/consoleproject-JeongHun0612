using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGame
{
    internal class Panel
    {
        public Rect Bounds { get; private set; }
        public Thickness Margin { get; private set; }
        public Thickness Padding { get; private set; }
        public bool HasBorder { get; private set; }

        public Panel(Rect bounds, Thickness margin, Thickness padding, bool hasBorder = true)
        {
            Margin = margin;
            Padding = padding;
            HasBorder = hasBorder;

            Bounds = bounds.Shrink(margin);
        }

        public Rect GetContentRect()
        {
            Rect content = Bounds;

            if (HasBorder)
                content = content.Shrink(new Thickness(1));

            content = content.Shrink(Padding);
            return content;
        }
    }
}
