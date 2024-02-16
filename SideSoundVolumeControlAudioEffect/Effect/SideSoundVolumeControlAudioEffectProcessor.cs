using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Preview.InkWorkspace;
using YukkuriMovieMaker.Player.Audio.Effects;

namespace SideSoundVolumeControlAudioEffect.Effect

{
    internal class SideSoundVolumeControlAudioEffectProcessor : AudioEffectProcessorBase
    {
        readonly SideSoundVolumeControlAudioEffect item;
        readonly TimeSpan duration;

        public override int Hz => Input?.Hz ?? 0;

        public override long Duration => (long)(duration.TotalSeconds * Input?.Hz ?? 0) * 2;

        public SideSoundVolumeControlAudioEffectProcessor(SideSoundVolumeControlAudioEffect item, TimeSpan duration)
        {
            this.item = item;
            this.duration = duration;
        }

        protected override int read(float[] destBuffer, int offset, int count)
        {
            Input?.Read(destBuffer, offset, count);
            for (int i = 0; i < count; i += 2)
            {
                var raw_balance = (int)item.Balance.GetValue((Position + i) / 2, Duration / 2, Hz);
                bool is_right = raw_balance > 0;
                //destBuffer[offset + i + 0] *= is_right?0:1;
                //destBuffer[offset + i + 1] *= is_right?1:0;
            }
            return count;
        }

        protected override void seek(long position)
        {
            Input?.Seek(position);
        }
    }
}
