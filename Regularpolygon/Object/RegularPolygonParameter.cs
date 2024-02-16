
using Regularpolygon.CustomControls;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Shape;
using YukkuriMovieMaker.Project;
using Regularpolygon.Struct;

namespace Regularpolygon.Object
{
    internal class RegularPolygonParameter : ShapeParameterBase
    {
        /// <summary>
        /// 描画する図形のサイズ
        /// </summary>
        [Display(Name = "サイズ")]
        [AnimationSlider("F1", "px", 1, 100)]
        public Animation Size { get; } = new Animation(100, 1, 5000);

        /// <summary>
        /// 正n角形を描画する情報種別と値
        /// </summary>
        /*
        [Display(GroupName = "描画設定", Name = "図形情報", Description = "項目の説明")]
        [RegularValueControls(typeof(ValueType), "[{\"Label\":\"角数\",\"Unit\":null,\"Start\":2.1,\"End\":100},{\"Label\":\"内角\",\"Unit\":\"度\",\"Start\":0.1,\"End\":359.9}]")]
        public (int, double) DrawValue { 
            get{
                return this._draw_value;
            }
            set { 
                Set(ref this._draw_value, value);
            }
        }
        (int,double) _draw_value = ((int)ValueType.描画情報を角数として扱う,2.1);
        */
        [Display(Name = "頂点の数", Description = "描画したい図形の各数を入力する")]
        [RegularValueControl(3, 50)]

        public PolygonBaseInfo PolygonInfo { get => polygonInfo; set => Set(ref polygonInfo, value); }
        PolygonBaseInfo polygonInfo = new PolygonBaseInfo() { PolygoNum = 4,DrawMode =0,NumberOfPolygonForDraw = -1 };

        [Display(GroupName = "描画オプション", Name = "線の太さ", Description = "線画の太さ")]
        [TextBoxSlider("F0", "px", 1, 100)]
        [Range(1, 100)]
        [DefaultValue(1d)]
        public double LineThin { get => lineThin; set => Set(ref lineThin, value); }
        double lineThin = 1;
        [Display(GroupName = "描画オプション", Name = "塗りつぶし", Description = "描画した図形の塗りつぶし")]
        [ToggleSlider]
        public bool FillRequired { get => fillRequired; set => Set(ref fillRequired, value); }
        bool fillRequired = true;
        [Display(GroupName = "描画オプション", Name = "塗りつぶし方法", Description = "塗りつぶしの方法")]
        [EnumComboBox]
        public FillType FillMethod { get => fillMethod; set => Set(ref fillMethod, value); }
        FillType fillMethod = FillType.輪郭で塗りつぶし;
        [Display(GroupName = "描画オプション", Name = "線画色", Description = "描画する線の色")]
        [ColorPicker]
        public Color LineColor { get => lineColor; set => Set(ref lineColor, value); }
        Color lineColor = Colors.White;
        [Display(GroupName = "描画オプション", Name = "塗りつぶし色", Description = "塗りつぶす色")]
        [ColorPicker]
        public Color FillColor { get => fillColor; set => Set(ref fillColor, value); }
        Color fillColor = Colors.White;
        [Display(GroupName = "試験的機能", Name = "自動調整", Description = "図形の角度が中途半端な角度で描画されないように調整します。")]
        [ToggleSlider]
        public bool IsAdjustmentPolygonAngle { get => isAdjustmentPolygonAngle; set => Set(ref isAdjustmentPolygonAngle, value); }
        bool isAdjustmentPolygonAngle = true;


        public enum FillType
        {
            輪郭で塗りつぶし,
            おしゃれに塗りつぶし,
        }
        public enum DrawType
        {
            正方形として描画,
            芒星として描画,
            芒星として描画＿図形の組み合わせ,
        }

        public RegularPolygonParameter() : this(null) { }
        public RegularPolygonParameter(SharedDataStore? sharedData) : base(sharedData) { }

        public override IEnumerable<string> CreateMaskExoFilter(int keyFrameIndex, ExoOutputDescription desc, ShapeMaskExoOutputDescription shapeMaskParameters)
        {
            return Enumerable.Empty<string>();
        }

        public override IEnumerable<string> CreateShapeItemExoFilter(int keyFrameIndex, ExoOutputDescription desc)
        {
            return Enumerable.Empty<string>();
        }

        public override IShapeSource CreateShapeSource(IGraphicsDevicesAndContext devices)
        {
            return new RegularPolygonSource(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => new IAnimatable[] { Size };

        protected override void LoadSharedData(SharedDataStore store)
        {
            var shareData = store.Load<SharedData>();
        }

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new SharedData(this));
        }

        class SharedData
        {
            public Animation Size { get; } = new Animation(100, 0, 5000);

            public double LineThin;
            public bool FillRequired;
            public FillType FillMethod;
            public Color LineColor;
            public Color FillColor;
            public PolygonBaseInfo PolybaseInfo;
            public bool isAdjustmentPolygonAngle;

            public SharedData(RegularPolygonParameter param)
            {
                Size.CopyFrom(param.Size);
                LineThin = param.LineThin;
                FillRequired = param.FillRequired;
                LineColor = param.LineColor;
                FillColor = param.FillColor;
                FillMethod = param.FillMethod;
                PolybaseInfo = param.PolygonInfo;
                isAdjustmentPolygonAngle = param.IsAdjustmentPolygonAngle;
            }
            public void CopyTo(RegularPolygonParameter param)
            {
                param.Size.CopyFrom(Size);
                param.LineThin = LineThin;
                param.FillRequired = FillRequired;
                param.LineColor = LineColor;
                param.FillColor = FillColor;
                param.FillMethod = FillMethod;
                param.PolygonInfo = PolybaseInfo;
                param.isAdjustmentPolygonAngle = isAdjustmentPolygonAngle;
            }
        }
    }
}
