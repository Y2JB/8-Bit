using System;
using System.Collections.Generic;

namespace Simulator
{

    public abstract class Component
    {
        public Component()
        {
        }

        protected List<IControlLine> controlLines = new List<IControlLine>();
    }

}
