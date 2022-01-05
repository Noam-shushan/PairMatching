using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui
{
    public static class GuiTools
    {
        public static bool SearchText(this string text, string subtext)
        {
            return text.ToLower().Contains(subtext.ToLower());
        }
    }
}
