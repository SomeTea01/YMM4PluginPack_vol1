using SoundWaveEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using YukkuriMovieMaker.Player.Audio.Effects;

namespace MorseCodeEffect.Effect
{
    internal class SoundWaveEffectProcessor : AudioEffectProcessorBase
    {
        readonly SoundWaveEffect item;
        readonly TimeSpan duration;
        public override int Hz => Input?.Hz ?? 0;

        public override long Duration => (long)(duration.TotalSeconds * Input?.Hz ?? 0) * 2;

        public SoundWaveEffectProcessor(SoundWaveEffect item, TimeSpan duration)
        {
            this.item = item;
            this.duration = duration;
        }

        protected override int read(float[] destBuffer, int offset, int count)
        {
            Input?.Read(destBuffer, offset, count);
            for (int i = 0; i < count; i += 2)
            {
                var waveType = item.WaveTypeEnum;
                var hz = item.EnableHz ? item.Hz.GetValue((Position + i) / 2, Duration / 2, Hz) : MakeSoundWave.GetHzFromNoteString(item.Text);
                if (!item.EnableHz && hz == 0)
                {
                    item.Text = "(A-Gの半角アルファベット)(#またはb)(0-8の数字)　になっている必要があります";
                    hz = 440;
                }
                var volume = (float)item.Volume.GetValue((Position + i) / 2, Duration / 2, Hz);
                double value = waveType switch
                {
                    SoundWaveEffect.WaveType.正弦波 => MakeSoundWave.GetValueAsSinWave(Hz, hz, (Position + i) / 2, 0),
                    SoundWaveEffect.WaveType.ノコギリ波 => (double)MakeSoundWave.GetValueAsSawToothWave(Hz, hz, (Position + i) / 2),
                    SoundWaveEffect.WaveType.三角波 => (double)MakeSoundWave.GetValueAsTriangleWave(Hz, hz, (Position + i) / 2),
                    SoundWaveEffect.WaveType.二分の一パルス波 => (double)MakeSoundWave.GetValueAsPulseWave(Hz, hz, (Position + i) / 2, 0.5),
                    SoundWaveEffect.WaveType.四分の一パルス波 => (double)MakeSoundWave.GetValueAsPulseWave(Hz, hz, (Position + i) / 2, 0.25),
                    SoundWaveEffect.WaveType.八分の一パルス波 => (double)MakeSoundWave.GetValueAsPulseWave(Hz, hz, (Position + i) / 2, 0.125),
                    _ => 0
                };

                var write = (float)(0.01f * value * volume);

                if (item.IsAppend)
                {
                    destBuffer[offset + i + 0] += write;
                    destBuffer[offset + i + 1] += write;
                }
                else
                {
                    destBuffer[offset + i + 0] = write;
                    destBuffer[offset + i + 1] = write;
                }
            }
            return count;
        }

        protected override void seek(long position)
        {
            Input?.Seek(position);
        }
    }
}
