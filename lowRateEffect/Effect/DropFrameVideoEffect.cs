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
    [VideoEffect("LowRateEffect", new[] { "Debugs" }, new[] { "Tea", "Debug" }, isAviUtlSupported: false)]
#else
    [VideoEffect("コマ落ちエフェクト", new[] { "描画", }, new[] { "Tea" },isAviUtlSupported:false)]
#endif
    internal class DropFrameVideoEffect : VideoEffectBase
    {

        /// <summary>
        /// エフェクト名
        /// </summary>
        public override string Label => "コマ落ちエフェクト";


        /*[Display(Name = "半径", Description = "基本設定のXYを中心とした円運動の半径")]
        [AnimationSlider("F0", "px", 2, 300)]
        public Animation r { get; } = new Animation(0, 1, 4000);*/
        [Display(GroupName = "エフェクト", Name = "落とすコマ数", Description = "")]
        [TextBoxSlider("F0", "", 0, 120)]
        [Range(0, 120)]
        [DefaultValue(0)]
        public double DropFrame { get => dropFrame; set => Set(ref dropFrame, value); }
        double dropFrame = 1;

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return Enumerable.Empty<string>();
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new DropFrameVideoProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => new IAnimatable[] {};
    }
}