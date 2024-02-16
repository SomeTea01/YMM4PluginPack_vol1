using Regularpolygon.Struct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Regularpolygon.Common
{
    public static class PolygonCaluculator
    {
        public static int[] GetPolygonsForDrawPolygon(int polygonNum)
        {
            List<int> answer = new List<int>();
            foreach (int num in Enumerable.Range(2, (int)Math.Round(Math.Pow(polygonNum, 0.5)) - 1))
            {
                if (polygonNum % num != 0) continue;
                if (num != 2) answer.Add(num);
                answer.Add(polygonNum / num);
            }
            answer.Remove(2);//2角形は存在しないため
            return answer.Distinct().ToArray();
        }
        public static int[] GetSkippableVertexForDrawStarPolygon(int polygonNum)
        {
            var polygons = GetPolygonsForDrawPolygon(polygonNum);
            var answer = Enumerable.Repeat(1,polygonNum-2).ToArray();
            answer = answer.Where(item => !polygons.Contains(item)).ToArray();
            return answer;
        }
        public static Vector2[][] GetPolygonsVectors(PolygonBaseInfo info, double size,double baseAngle = 0)
        {
            switch (info.DrawMode)
            {
                case 0:
                    return [GetPolygonVectors(info.PolygoNum, size, baseAngle)];
                case 1:
                    return [GetStarPolygonVectors(info.PolygoNum, size, info.NumberOfSkipVertex, baseAngle)];
                case 2:
                    List<Vector2[]> polygonVectors = new List<Vector2[]>();
                    int loop_count = info.PolygoNum / info.NumberOfPolygonForDraw;
                    double VertexAngle = 360d / info.PolygoNum;
                    foreach (int num in Enumerable.Range(0, loop_count))
                    {
                        polygonVectors.Add(GetPolygonVectors(info.NumberOfPolygonForDraw, size, baseAngle + VertexAngle * num));
                    }
                    return [.. polygonVectors];
            }
            throw new InvalidOperationException($"PolygonCalculatorを用いた計算処理に不正な値が代入されました。{Environment.NewLine}PolygonInfo.DrawMode={info.DrawMode}");

        }
        public static Vector2[] GetPolygonVectors(int polygonNumber,double size,double baseAngle = 0)
        {
            List<Vector2> answer = new List<Vector2>();
            double PolyAngle = 360d / polygonNumber;
            foreach (int num in Enumerable.Range(0, polygonNumber))
            {
                double pointAngle = ConvertAngleToRad(PolyAngle * num + baseAngle);
                answer.Add(new Vector2((float)(Math.Cos(pointAngle) * size /2), (float)(Math.Sin(pointAngle) * size / 2)));
            }
            return answer.Select(item => new Vector2(item.X, item.Y)).ToArray();
        }
        public static Vector2[] GetStarPolygonVectors(int polygonNumber,double size,int type,double baseAngle = 0)
        {
            List<Vector2> answer = new List<Vector2>();
            double PolyAngle = 360d / polygonNumber;
            foreach(int num in Enumerable.Range(0, polygonNumber))
            {
                double pointAngle = ConvertAngleToRad(PolyAngle * (num * type) + baseAngle);
                answer.Add(new Vector2((float)(Math.Cos(pointAngle) * size/2), (float)(Math.Sin(pointAngle) * size/2)));
            }
            return answer.ToArray();
        }
        public static double ConvertAngleToRad(double angle)
        {
            angle -= 90;
            if (angle < 0) angle += 360;
            angle %= 360;
            return Math.PI * angle / 180;
        }
    }
}
