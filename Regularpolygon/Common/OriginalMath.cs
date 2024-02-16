using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regularpolygon.Common
{
    public static class OriginalMath
    {
        public static int GetCoefficientToLeastCommonMultiple(int a, int b)
        {
            Queue<decimal> queues = new Queue<decimal>();
            decimal root = (decimal)b / a;
            while (!IsDecimal(root))
            {
                queues.Enqueue(root);
                root -= Math.Floor(root);
                root = 1 / root;
            }
            while (queues.Count > 0)
            {
                root *= queues.Dequeue();
            }
            return (int)root;
        }
        private static bool IsDecimal(decimal value, int accuracy = 10)
        {
            return Math.Round(value) == Math.Round(value, accuracy);
        }
    }
}
