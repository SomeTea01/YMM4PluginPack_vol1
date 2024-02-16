using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace PosterizationVideoEffect.Effect
{
#if DEBUG
    [VideoEffect("Postarization", new[] { "Debugs" }, new[] { "Tea", "Debug" }, isAviUtlSupported: false)]
#else
    [VideoEffect("ポスタリゼーション", new[] { "加工", }, new[] { "Tea" },isAviUtlSupported:false)]
#endif
    internal class PosterizationVideoEffect : VideoEffectBase
    {

        /// <summary>
        /// エフェクト名
        /// </summary>
        public override string Label => "ポスタリゼーション";


        /// <summary>
        /// アイテム編集エリアに表示するエフェクトの設定項目。
        /// [Display]と[AnimationSlider]等のアイテム編集コントロール属性の2つを設定する必要があります。
        /// [AnimationSlider]以外のアイテム編集コントロール属性の一覧はSamplePropertyEditorsプロジェクトを参照してください。
        /// </summary>
        [Display(Name = "階調", Description = "階調の数")]
        [AnimationSlider("F0", "", 2, 16)]
        public Animation GrayLevel { get; } = new Animation(0, 2, 16);

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return Enumerable.Empty<string>();
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new PosterizationVideoProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => new IAnimatable[] { GrayLevel };
    }
}