using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.ApplicationModel.Chat;

namespace Regularpolygon.Struct
{
    public struct PolygonBaseInfo
    {

        public int PolygoNum { get; set; } = 5;
        public int DrawMode { get; set; } = 0;
        public int NumberOfPolygonForDraw { get; set; } = -1;
        public int NumberOfSkipVertex { get; set; } = 0;

        public bool Equals(PolygonBaseInfo value)
        {
            return this.DrawMode == value.DrawMode && this.PolygoNum == value.PolygoNum && this.NumberOfPolygonForDraw == value.NumberOfPolygonForDraw && this.NumberOfSkipVertex == value.NumberOfSkipVertex;
        }
        public PolygonBaseInfo(int polygonNum,int drawMode,int numberOfpolygonForDraw,int numberOfSkipVertex)
        {
            this.PolygoNum = polygonNum;
            this.DrawMode = drawMode;
            this.NumberOfPolygonForDraw = numberOfpolygonForDraw;
            this.NumberOfSkipVertex = numberOfSkipVertex;
        }
        public PolygonBaseInfo CopyWithOverWrite(int? polygonNum = null, int? drawMode = null,int? numberOfPolygonForDraw = null,int? numberOfSkipVertex = null)
        {
            PolygonBaseInfo copied = new PolygonBaseInfo();
            this.CopyTo(out copied);
            if(polygonNum.HasValue) copied.PolygoNum=polygonNum.Value;
            if(drawMode.HasValue) copied.DrawMode=drawMode.Value;
            if(numberOfPolygonForDraw.HasValue) copied.NumberOfPolygonForDraw=numberOfPolygonForDraw.Value;
            if(numberOfSkipVertex.HasValue) copied.NumberOfSkipVertex=numberOfSkipVertex.Value;
            return copied;
        }
        public void CopyTo(out PolygonBaseInfo dest)
        {
            dest = new PolygonBaseInfo();
            dest.PolygoNum = this.PolygoNum;
            dest.DrawMode = this.DrawMode;
            dest.NumberOfPolygonForDraw= this.NumberOfPolygonForDraw;
            dest.NumberOfSkipVertex= this.NumberOfSkipVertex;
        }

	}
}
