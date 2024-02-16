using Regularpolygon.Struct;
using System.Numerics;
using System.Windows.Media;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using static Regularpolygon.Object.RegularPolygonParameter;
using Regularpolygon.Common;

namespace Regularpolygon.Object
{
    internal class RegularPolygonSource : IShapeSource
    {
        readonly private IGraphicsDevicesAndContext devices;
        readonly private RegularPolygonParameter regularPolygonParameter;

        ID2D1SolidColorBrush lineBrush;
        ID2D1SolidColorBrush fillBrush;
        ID2D1CommandList? commandList;

        double size;
        PolygonBaseInfo PolygonInfo;

        double lineThin;
        Color lineColor;

        bool fillRequired;
        Color fillColor;

        FillType fillMethod;
        bool isAdjustmentPolygonAngle;

        ID2D1Image IDrawable.Output => commandList ?? throw new Exception($"{nameof(commandList)}がnullです。事前にUpdateを呼び出す必要があります。");

        public RegularPolygonSource(IGraphicsDevicesAndContext decvices, RegularPolygonParameter regularPolygonParameter)
        {
            devices = decvices;
            this.regularPolygonParameter = regularPolygonParameter;
        }


        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // マネージド状態を破棄します (マネージド オブジェクト)

                    commandList?.Dispose();
                    /*
                    whiteBrush?.Dispose();
                    */
                }

                // アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~SampleShapeSource()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        void IShapeSource.Update(TimelineItemSourceDescription timelineItemSourceDescription)
        {
            var fps = timelineItemSourceDescription.FPS;
            var frame = timelineItemSourceDescription.ItemPosition.Frame;
            var length = timelineItemSourceDescription.ItemDuration.Frame;
            var polygonInfos = regularPolygonParameter.PolygonInfo;
            var size = regularPolygonParameter.Size.GetValue(frame, length, fps);
            var fillRequired = regularPolygonParameter.FillRequired;
            var lineThin = regularPolygonParameter.LineThin;
            var lineColor = regularPolygonParameter.LineColor;
            var fillColor = regularPolygonParameter.FillColor;
            var fillMethod = regularPolygonParameter.FillMethod;
            var isAdjustmentPolygonAngle = regularPolygonParameter.IsAdjustmentPolygonAngle;
            //サイズが変わっていない場合は何もしない
            if (commandList != null)
            {
                bool isChanged = this.size != size;
                isChanged |= (!this.PolygonInfo.Equals(polygonInfos));
                isChanged |= this.fillRequired != fillRequired;
                isChanged |= this.lineThin != lineThin;
                isChanged |= this.lineColor != lineColor;
                isChanged |= this.fillColor != fillColor;
                isChanged |= this.fillMethod != fillMethod;
                isChanged |= this.isAdjustmentPolygonAngle != isAdjustmentPolygonAngle;
                if (!isChanged) return;
            }
            lineBrush = devices.DeviceContext.CreateSolidColorBrush(new Vortice.Mathematics.Color4(lineColor.R / 255f, lineColor.G / 255f, lineColor.B / 255f, 1));
            fillBrush = devices.DeviceContext.CreateSolidColorBrush(new Vortice.Mathematics.Color4(fillColor.R / 255f, fillColor.G / 255f, fillColor.B / 255f, 1));
            var dc = devices.DeviceContext;

            commandList?.Dispose();//新規作成前に、前回のCommandListを必ず破棄する
            commandList = dc.CreateCommandList();

            var geometry = CreatePolygonGeometry(polygonInfos, size, fillMethod, isAdjustmentPolygonAngle);
            if (geometry is null) return;
            dc.Target = commandList;
            dc.BeginDraw();
            dc.Clear(null);
            dc.DrawGeometry(geometry, lineBrush, (float)lineThin);
            if (fillRequired) dc.FillGeometry(geometry, fillBrush);
            dc.EndDraw();
            dc.Target = null;//Targetは必ずnullに戻す。
            commandList.Close();//CommandListはEndDraw()の後に必ずClose()を呼んで閉じる必要がある

            this.size = size;
            this.PolygonInfo = polygonInfos;
            this.fillRequired = fillRequired;
            this.lineThin = lineThin;
            this.lineColor = lineColor;
            this.fillColor = fillColor;
            this.fillMethod = fillMethod;
            this.isAdjustmentPolygonAngle = isAdjustmentPolygonAngle;
        }

        private ID2D1Geometry? CreatePolygonGeometry(PolygonBaseInfo polygon_infor, double size, FillType fillType, bool IsAdjustmentPolygonAngle)
        {
            var geometory = devices.DeviceContext.Factory.CreatePathGeometry();
            var base_angle = 0;
            if(polygon_infor.PolygoNum %2 == 0 && IsAdjustmentPolygonAngle)
            {
                base_angle = 180 / polygon_infor.PolygoNum;
            }
            Vector2[][] vecs = PolygonCaluculator.GetPolygonsVectors(polygon_infor,size, base_angle);
            
            if (vecs.Count() == 0) return null;
            using (var sink = geometory.Open())
            {
                for(int i=0;i<vecs.Length; i++)
                {
                    var vec = vecs[i].ToList();
                    var startVec = vec.First();
                    vec.AddRange(vec.GetRange(0, 2));
                    vec.RemoveAt(0);

                    if (fillType == FillType.輪郭で塗りつぶし) sink.SetFillMode(FillMode.Winding);
                    else sink.SetFillMode(FillMode.Alternate);

                    sink.BeginFigure(startVec, FigureBegin.Filled);
                    sink.AddLines(vec.ToArray());
                    sink.EndFigure(FigureEnd.Open);

                }
                sink.Close();

                return geometory;
            }
        }

        private decimal GetCornersFromNGon(decimal n_gon, int count = 0)
        {
            if (!IsDecimal(n_gon)) return n_gon;
            decimal afterPoint = n_gon - (int)n_gon;

            decimal returnValue = 1 / afterPoint;
            decimal DecimalPlaceNum = (decimal)Math.Pow(10, 15 - count);
            returnValue = Math.Floor(returnValue * DecimalPlaceNum) / DecimalPlaceNum;
            if (!IsDecimal(returnValue)) return n_gon * returnValue;
            if (count > 4) return returnValue;
            decimal nest = GetCornersFromNGon(returnValue, count + 1);
            if (count == 0) nest = Math.Round(nest);
            return nest * n_gon;

        }
        bool IsDecimal(decimal num)
        {
            return num - (int)num != 0;
        }
        double ConvertToRad(double Angle)
        {
            double angle = Angle % 360;
            return angle / 180 * Math.PI;
        }

    }
}