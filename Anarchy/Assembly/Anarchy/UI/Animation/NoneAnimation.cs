using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarchy.UI.Animation
{
    class NoneAnimation : Animation
    {
        public NoneAnimation(GUIBase @base) : base(@base)
        {
        }

        protected override bool Close()
        {
            return false;
        }

        protected override bool Open()
        {
            myBase.Draw();
            return false;
        }
    }
}
