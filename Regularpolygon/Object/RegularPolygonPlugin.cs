using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;

namespace Regularpolygon.Object
{
    internal class RegularPolygonPlugin : IShapePlugin
    {
        public bool IsExoShapeSupported => false;

        public bool IsExoMaskSupported => false;

        public string Name => "正多角形";

        public IShapeParameter CreateShapeParameter(SharedDataStore? sharedData)
        {
            return new RegularPolygonParameter(sharedData);
        }
    }
}
