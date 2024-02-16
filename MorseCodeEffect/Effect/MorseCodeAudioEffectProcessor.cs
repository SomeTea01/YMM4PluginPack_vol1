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
    internal class MorseCodeAudioEffectProcessor : AudioEffectProcessorBase
    {
        readonly MorseCodeAudioEffect item;
        readonly TimeSpan duration;
        private int[] codes = Enumerable.Empty<int>().ToArray();
        private int before_ringingIndex = 0;
        private int loopCount = 0;
        public override int Hz => Input?.Hz ?? 0;

        public override long Duration => (long)(duration.TotalSeconds * Input?.Hz ?? 0) * 2;

        public MorseCodeAudioEffectProcessor(MorseCodeAudioEffect item, TimeSpan duration)
        {
            this.item = item;
            this.duration = duration;
        }

        protected override int read(float[] destBuffer, int offset, int count)
        {
            Input?.Read(destBuffer, offset, count);
            for (int i = 0; i < count; i += 2)
            {
                var wpm = item.WPM;
                var hz = item.Hz.GetValue((Position + i) / 2, Duration / 2, Hz);
                var volume = (float)item.Volume.GetValue((Position + i) / 2, Duration / 2, Hz);
                var value = (float)Math.Round(ConvertTextToMorseCode(wpm, (Position + i) / 2, item.Text, hz) * Math.Pow(10, 8)) / (float)Math.Pow(10, 8);
                var write = 0.01f * value * volume;

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
        private double ConvertTextToMorseCode(double wpm, long count, string text, double hz)
        {
            var dummy_codes = new List<int>() { };
            char[] chars = ConvertTextForMorse(text).ToCharArray();
            foreach (var ch in chars)
            {
                if (!codeTable.ContainsKey(ch)) continue;
                dummy_codes.AddRange(codeTable[ch]);
                dummy_codes.Add(-1);
            }
            codes = dummy_codes.ToArray();

            return GetValueFromCode(wpm, count, hz);
        }
        private double TimeToangle(double hz, double time)
        {
            return 2 * Math.PI * (time / (1 / hz));
        }
        private double GetValueFromCode(double wpm, long count, double hz)
        {
            if (codes.Length == 0) return 0;
            double ringingSpan = 1 / (wpm * 5 / 6.0);//1打音単位の長さ(s)
            double processingTime = count / (double)Hz;//再生時間(s)
            double tonePosition = processingTime / ringingSpan;//何打音目か
            int codeSum = codes.Select((item) => { if (item == -1) return 3; else return item + 1; }).Sum();
            tonePosition -= codeSum * loopCount;

            if (codeSum < tonePosition)
            {
                if (!item.IsLoop) return 0;
                loopCount++;
                tonePosition -= codeSum;
                before_ringingIndex = 0;
            }

            int toneSum = codes.Where((item, index) => index <= before_ringingIndex).Select((item) => { if (item == -1) return 3; else return item + 1; }).Sum();//再生中のサウンドの終了打音数
            int currentIndex = before_ringingIndex;
            if (toneSum < tonePosition)
            {
                currentIndex++;
                toneSum += codes[currentIndex % codes.Length] + 1;
                before_ringingIndex = currentIndex;

            }
            toneSum += codeSum * (currentIndex / codes.Length);
            int currentCode = codes[currentIndex % codes.Length];
            //
            bool is_ringing = currentCode != -1 && toneSum - tonePosition > 1;//休符のタイミングの時
            if (!is_ringing) return 0;

            double ringingTime = (tonePosition - (toneSum - currentCode - 1)) * ringingSpan;
            return Math.Sin(TimeToangle(hz, ringingTime));

        }

        private string ConvertTextForMorse(string text)
        {
            string converted = string.Empty;
            foreach (char ch in text)
            {
                if (IsRoman(ch))
                {
                    converted += char.ToUpper(ch);
                    continue;
                }
                if (IsHiragana(ch))
                {
                    char Katakana = (char)(ch + ('ァ' - 'ぁ'));
                    if (Katakana - 'カ' > 0 && Katakana - 'ド' <= 0)//カ、サ、タ行の文字だったら
                    {
                        int threhold = Katakana > 'ツ' ? 0 : 1;
                        if ((Katakana - 'カ') % 2 == threhold)//濁点がついていたら
                        {
                            converted += (char)(Katakana - 1);
                            converted += '゛';
                            continue;
                        }
                    }
                    if (Katakana - 'ハ' > 0 && Katakana - 'ポ' < 0)//ハ行の文字だったら
                    {
                        int mark_num = (Katakana - 'ハ') % 3;
                        if (mark_num != 0)
                        {
                            converted += (char)(Katakana - mark_num);//
                            converted += mark_num % 2 == 1 ? '゛' : '゜';
                            continue;
                        }

                    }
                    converted += Katakana;
                    continue;
                }
                converted += ch;
            }
            return converted.ToString();
        }

        private bool IsHiragana(char a)
        {

            int range = 'ゔ' - 'ぁ';
            int charRange = 'ゔ' - a;
            return charRange > 0 && charRange <= range;

        }
        private bool IsRoman(char a)
        {
            return a >= 'A' && a <= 'Z' || a >= 'a' && a <= 'z';
        }
        private readonly Dictionary<char, int[]> codeTable = new()
        {
            {'A',new int[]{1,3}},
            {'B',new int[]{3,1,1,1}},
            {'C',new int[]{3,1,3,1}},
            {'D',new int[]{3,1,1}},
            {'E',new int[]{1}},
            {'F',new int[]{1,1,3,1}},
            {'G',new int[]{3,3,1}},
            {'H',new int[]{1,1,1,1}},
            {'I',new int[]{1,1}},
            {'J',new int[]{1,3,3,3}},
            {'K',new int[]{3,1,3}},
            {'L',new int[]{1,3,1,1}},
            {'M',new int[]{3,3}},
            {'N',new int[]{3,1}},
            {'O',new int[]{3,3,3}},
            {'P',new int[]{1,3,3,1}},
            {'Q',new int[]{3,3,1,3}},
            {'R',new int[]{1,3,1}},
            {'S',new int[]{1,1,1}},
            {'T',new int[]{3}},
            {'U',new int[]{1,1,3}},
            {'V',new int[]{1,1,1,3}},
            {'W',new int[]{1,3,3}},
            {'X',new int[]{3,1,1,3}},
            {'Y',new int[]{3,1,3,3}},
            {'Z',new int[]{3,3,1,1}},
            {'.' ,new int[]{1,3,1,3,1,3}},
            {',' ,new int[]{3,3,1,1,3,3}},
            {':' ,new int[]{3,3,3,1,1,1}},
            {'?' ,new int[]{1,1,3,3,1,1}},
            {'\'' ,new int[]{1,3,3,3,3,1}},
            {'-' ,new int[]{3,1,1,1,1,3}},
            {'(' ,new int[]{3,1,3,3,1}},
            {')' ,new int[]{3,1,3,3,1,3}},
            {'/' ,new int[]{3,1,1,3,1}},
            {'=' ,new int[]{3,1,1,1,3}},
            {'+' ,new int[]{1,3,1,3,1}},
            {'"' ,new int[]{1,3,1,1,3,1}},
            {'*' ,new int[]{3,1,1,3}},
            {'@' ,new int[]{1,3,3,1,3,1}},
            {'ア',new int[]{3,3,1,3,3}},
            {'イ',new int[]{1,3}},
            {'ウ',new int[]{1,1,3}},
            {'エ',new int[]{3,1,3,3,3}},
            {'オ',new int[]{1,3,1,1,1}},
            {'カ',new int[]{1,3,1,1}},
            {'キ',new int[]{3,1,3,1,1}},
            {'ク',new int[]{1,1,1,3}},
            {'ケ',new int[]{3,1,3,3}},
            {'コ',new int[]{3,3,3,3}},
            {'サ',new int[]{3,1,3,1,3}},
            {'シ',new int[]{3,3,1,3,1}},
            {'ス',new int[]{3,3,3,1,3}},
            {'セ',new int[]{1,3,3,3,1}},
            {'ソ',new int[]{3,3,3,1}},
            {'タ',new int[]{3,1}},
            {'チ',new int[]{1,1,3,1}},
            {'ツ',new int[]{1,3,3,1}},
            {'テ',new int[]{1,3,1,3,3}},
            {'ト',new int[]{1,1,3,1,1}},
            {'ナ',new int[]{1,3,1}},
            {'ニ',new int[]{3,1,3,1}},
            {'ヌ',new int[]{1,1,1,1}},
            {'ネ',new int[]{3,3,1,3}},
            {'ノ',new int[]{1,1,3,3}},
            {'ハ',new int[]{3,1,1,1}},
            {'ヒ',new int[]{3,3,1,1,3}},
            {'フ',new int[]{3,3,1,1}},
            {'ヘ',new int[]{1}},
            {'ホ',new int[]{3,1,1}},
            {'マ',new int[]{3,1,1,3}},
            {'ミ',new int[]{1,1,3,1,3}},
            {'ム',new int[]{3}},
            {'メ',new int[]{3,1,1,1,3}},
            {'モ',new int[]{3,1,1,3,1}},
            {'ヤ',new int[]{1,3,3}},
            {'ユ',new int[]{3,1,1,3,3}},
            {'ヨ',new int[]{3,3}},
            {'ラ',new int[]{1,1,1}},
            {'リ',new int[]{3,3,1}},
            {'ル',new int[]{3,1,3,3,1}},
            {'レ',new int[]{3,3,3}},
            {'ロ',new int[]{1,3,1,3}},
            {'ワ',new int[]{3,1,3}},
            {'ヰ',new int[]{1,3,1,1,3}},
            {'ヲ',new int[]{1,3,3,3}},
            {'ヱ',new int[]{1,3,3,1,1}},
            {'ン',new int[]{1,3,1,3,1}},
            {'゛',new int[]{1,1}},
            {'゜',new int[]{1,1,3,3,1}},
            {'ー',new int[]{1,3,3,1,3}},
            {'、',new int[]{1,3,1,3,1,3}},
            {'（',new int[]{3,1,3,3,1,3}},
            {'）',new int[]{1,3,1,1,3,1}}
        };


    }
}
