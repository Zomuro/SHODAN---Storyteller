using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Zomuro.SHODANStoryteller
{
    public class CompProperties_SubversionObserver : CompProperties
    {
        public CompProperties_SubversionObserver()
        {
            compClass = typeof(Comp_SubversionObserver);
        }

        public float range = 3.9f;
    }
}
