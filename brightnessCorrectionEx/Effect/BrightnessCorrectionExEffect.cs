using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace brightnessCorrectionEx.Effect
{
#if DEBUG
    [VideoEffect("BrightnessCorrectionExEffect", new[] { "Debugs" }, new[] { "Tea", "Debug" }, isAviUtlSupported: false)]
#else
    [VideoEffect("明るさ補正Ex", new[] { "加工", }, new[] { "Tea" },isAviUtlSupported:false)]
#endif
    public class BrightnessCorrectionExEffect : VideoEffectBase
    {
        public override string Label => "明るさ補正Ex";

        [Display(Name = "明るさ上限", Description = "明るさの上限")]
        [AnimationSlider("F0", "", 0, 255)]
        public Animation MaxBrightness { get; } = new Animation(255, 0, 255);

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return Enumerable.Empty<string>();
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new BrightnessCorrectionExEffectProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => new IAnimatable[] { };
    }
}
