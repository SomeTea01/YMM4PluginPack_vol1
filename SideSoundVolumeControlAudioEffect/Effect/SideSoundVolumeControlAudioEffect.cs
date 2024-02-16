using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Audio.Effects;
using YukkuriMovieMaker.Plugin.Effects;

namespace SideSoundVolumeControlAudioEffect.Effect
{

#if DEBUG
    [AudioEffect("音量バランス調整", new[] { "Debug", "加工" }, new string[] { "Tea", "モールス信号" }, isAviUtlSupported: false)]
#else
        [AudioEffect("音量バランス調整", new[] {"加工" }, new string[] { "Tea", "モールス信号" },isAviUtlSupported:false)]
#endif

    public class SideSoundVolumeControlAudioEffect : AudioEffectBase

    {
        public override string Label => "音量バランス調整";



        [Display(Name = "左右の音量バランス", Description = "0より左であれば左の音量が、右に設定すれば右の音量に音量をよせます")]
        [AnimationSlider("F0", "", -100, 100)]
        public Animation Balance { get; } = new Animation(0, -100, 100);

        public override IAudioEffectProcessor CreateAudioEffect(TimeSpan duration)
        {
            return new SideSoundVolumeControlAudioEffectProcessor(this, duration);
        }

        public override IEnumerable<string> CreateExoAudioFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return Enumerable.Empty<string>();
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => new[] { Balance };
    }
}
