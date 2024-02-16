using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace CircularMotionVideoProcessor.Effect
{
#if DEBUG
    [VideoEffect("circular motion", new[] { "Debugs" }, new[] { "Tea", "Debug" }, isAviUtlSupported: false)]
#else
    [VideoEffect("円運動エフェクト", new[] { "アニメーション", }, new[] { "Tea" },isAviUtlSupported:false)]
#endif
    internal class CircularMotionVideoEffect : VideoEffectBase
    {

        /// <summary>
        /// エフェクト名
        /// </summary>
        public override string Label => "円運動エフェクト";


        [Display(GroupName= "エフェクト",Name = "半径", Description = "基本設定のXYを中心とした円運動の半径")]
        [AnimationSlider("F0", "px", 2, 300)]
        public Animation r { get; } = new Animation(0, 1, 4000);
        [Display(GroupName = "エフェクト", Name = "軌道の縦横比", Description = "回転移動の縦横比を調整します。プラスの値で縦長に、マイナスで横長の軌道になります。")]
        [AnimationSlider("F1", "", -100, 100)]
        public Animation aspect { get; } = new Animation(0, -100, 100);
        [Display(GroupName = "エフェクト", Name = "軌道の角度", Description = "回転軌道の角度を定義します")]
        [AnimationSlider("F1", "", -3600, 3600)]
        public Animation movementAngle { get; } = new Animation(0,-360, 360);
        [Display(GroupName = "エフェクト", Name = "回転速度", Description = "円運動の速度")]
        [TextBoxSlider("F0", "度", -360, 360)]
        [Range(-3600, 3600)]
        [DefaultValue(100)]
        public double Speed { get => speed; set => Set(ref speed, value); }
        double speed = 1;
        [Display(GroupName = "エフェクト", Name = "開始角度", Description = "円運動の速度")]
        [TextBoxSlider("F0", "度", 0, 360)]
        [Range(0, 360)]
        [DefaultValue(100)]
        public double BaseAngle { get => baseAngle; set => Set(ref baseAngle, value); }
        double baseAngle = 1;

        [Display(GroupName = "エフェクト", Name = "角度の自動調整", Description = "回転に合わせて角度を調整します。")]
        [ToggleSlider]
        public bool AutAdjustmentAngle { get => autAdjustmentAngle; set => Set(ref autAdjustmentAngle, value); }
        bool autAdjustmentAngle = false;

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return Enumerable.Empty<string>();
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new CircularMotionVideoProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => new IAnimatable[] { r ,aspect, movementAngle };
    }
}