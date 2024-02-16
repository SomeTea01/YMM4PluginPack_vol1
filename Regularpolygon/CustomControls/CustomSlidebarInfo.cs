using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Controls;

namespace Regularpolygon.CustomControls
{
    public class CustomSlidebarInfo
    {
		public string Label { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public string Unit { get; set; }

        public CustomSlidebarInfo(string label,string unit,double start,double end) {
            this.Label = label;
            this.Start = start;
            this.End = end;
            this.Unit = unit;
        }

	}
}
