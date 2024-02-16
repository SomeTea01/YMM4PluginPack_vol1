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
    [AudioEffect("音声波形生成エフェクト", new[] { "Debug", "加工","再生成" }, new string[] { "Tea", "生成" }, isAviUtlSupported: false)]
#else
    [AudioEffect("音声波形生成エフェクト", new[] {"加工" }, new string[] { "Tea", "生成" },isAviUtlSupported:false)]
#endif

    public class SoundWaveEffect : AudioEffectBase

    {
        public enum WaveType
        {
            正弦波,
            ノコギリ波,
            三角波,
            二分の一パルス波,
            四分の一パルス波,
            八分の一パルス波
        }
        public override string Label => "音声波形生成エフェクト";

        [Display(GroupName = "エフェクト", Name = "音源をかぶせる", Description = "元の音源にかぶせて再生します")]
        [ToggleSlider]
        public bool IsAppend { get => isAppend; set => Set(ref isAppend, value); }
        bool isAppend = false;
        [Display(GroupName = "エフェクト", Name = "波形", Description = "発生させる波形を指定します")]
        [EnumComboBox]
        public WaveType WaveTypeEnum { get => waveTypeEnum; set => Set(ref waveTypeEnum, value); }
        WaveType waveTypeEnum = WaveType.正弦波;

        [Display(GroupName = "エフェクト", Name = "音量", Description = "音の大きさを変更します")]
        [AnimationSlider("F2", "%", 0, 400)]
        public Animation Volume { get; } = new Animation(100, 0, 2000);

        [Display(GroupName = "エフェクト", Name = "音階", Description = "記述した音階で音を鳴らします。入力例：ド#->C4# or D4b")]
        [TextEditor(AcceptsReturn = true)]
        public string Text { get => text; set => Set(ref text, value); }
        string text = string.Empty;

        [Display(GroupName = "エフェクト", Name = "周波数を有効にする", Description = "周波数を用いて音の高さを変更します")]
        [ToggleSlider] 
        public bool EnableHz { get => enableHz; set => Set(ref enableHz, value); }
        bool enableHz = false;

        [Display(GroupName = "エフェクト", Name = "周波数", Description = "音の高さを変更します")]
        [AnimationSlider("F4", "Hz", 100, 800)]
        public Animation Hz { get; } = new Animation(440, 100, 4200);

        public override IAudioEffectProcessor CreateAudioEffect(TimeSpan duration)
        {
            return new SoundWaveEffectProcessor(this, duration);
        }

        public override IEnumerable<string> CreateExoAudioFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return Enumerable.Empty<string>();
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => new[] { Volume, Hz };
    }
}
