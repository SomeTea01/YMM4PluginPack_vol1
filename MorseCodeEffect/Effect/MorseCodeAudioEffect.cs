using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Audio.Effects;
using YukkuriMovieMaker.Plugin.Effects;

namespace MorseCodeEffect.Effect
{

#if DEBUG
    [AudioEffect("モールス信号エフェクト", new[] { "Debug", "加工" }, new string[] { "Tea", "モールス信号" }, isAviUtlSupported: false)]
#else
        [AudioEffect("モールス信号エフェクト", new[] {"加工" }, new string[] { "Tea", "モールス信号" },isAviUtlSupported:false)]
#endif

    public class MorseCodeAudioEffect : AudioEffectBase

    {
        public override string Label => "モールス信号";


        [Display(GroupName = "エフェクト", Name = "音源をかぶせる", Description = "元の音源にかぶせて再生します")]
        [ToggleSlider]
        public bool IsAppend { get => isAppend; set => Set(ref isAppend, value); }
        bool isAppend = false;
        [Display(GroupName = "エフェクト", Name = "テキスト", Description = "モールス信号に変換するテキスト(漢字は未対応です)")]
        [TextEditor(AcceptsReturn = true)]
        public string Text { get => text; set => Set(ref text, value); }
        string text = string.Empty;

        [Display(GroupName = "エフェクト", Name = "健打速度", Description = "生成するモールス信号の速度を調整します。")]
        [TextBoxSlider("F1", "WPM", 1, 40)]
        [DefaultValue(18)]
        [Range(1, 40)]
        public double WPM { get => wpm; set => Set(ref wpm, value); }
        double wpm = 18;

        [Display(GroupName = "エフェクト", Name = "音量", Description = "生成するモールス信号の音量を調節します")]
        [AnimationSlider("F2", "", 0, 10)]
        public Animation Volume { get; } = new Animation(1, 0, 100);

        [Display(GroupName = "エフェクト", Name = "ループ", Description = "モールス信号をループ")]
        [ToggleSlider]
        public bool IsLoop { get => isLoop; set => Set(ref isLoop, value); }
        bool isLoop = false;

        [Display(GroupName = "エフェクト", Name = "周波数", Description = "トンツーの音の高さを変更します")]
        [AnimationSlider("F1", "Hz", 300, 800)]
        public Animation Hz { get; } = new Animation(440, 300, 900);

        public override IAudioEffectProcessor CreateAudioEffect(TimeSpan duration)
        {
            return new MorseCodeAudioEffectProcessor(this, duration);
        }

        public override IEnumerable<string> CreateExoAudioFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return Enumerable.Empty<string>();
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => new[] { Volume, Hz };
    }
}
