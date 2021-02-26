using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace WendigoProject
{
    [StaticConstructorOnStartup]
    public static class WendigoProgramUtils
    {
        static WendigoProgramUtils()
        {
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs.Where(x => x.race?.Humanlike ?? false))
            {
                thingDef.comps.Add(new CompProperties_WendigoProgram());
            }
        }
    }
}
