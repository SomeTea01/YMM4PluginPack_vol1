using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoundWaveEx
{
    internal　static class MakeSoundWave
    {
        private static decimal GetPercentageOfPosition(long countPerSec,double hz, long position)
        {
            decimal countPerHz = countPerSec / (decimal)hz;
            return  (position%countPerHz) / countPerHz;
        }
        public static double ConvertDgreeToRad(double degree, bool enableZeroToUp = false)
        {
            if (enableZeroToUp) degree -= 90;
            if (degree < 0) degree += 360;
            degree %= 360;
            return Math.PI * degree / 180;
        }
        /// <summary>
        /// return 0 to 1
        /// </summary>
        /// <param name="countPerSec"></param>
        /// <param name="hz"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static decimal GetValueAsSawToothWave(long countPerSec,double hz,long position)
        {
            decimal percentage = GetPercentageOfPosition(countPerSec,hz,position);

            return percentage - Math.Floor(percentage);
        }
        /// <summary>
        /// return 0 to 1
        /// </summary>
        /// <param name="countPerSec"></param>
        /// <param name="hz"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static decimal GetValueAsTriangleWave(long countPerSec,double hz,long position)
        {
            decimal base_line = GetValueAsSawToothWave(countPerSec, hz, position);
            decimal wave = 0.5m - Math.Abs(base_line - 0.5m);
            return wave*2;
        }
        /// <summary>
        /// return 0 or 1.
        /// </summary>
        /// <param name="countPerSec"></param>
        /// <param name="hz"></param>
        /// <param name="position"></param>
        /// <param name="pulseRate">1-0</param>
        /// <returns></returns>
        public static decimal GetValueAsPulseWave(long countPerSec,double hz,long position,double pulseRate)
        {
            double percentage = (double)GetPercentageOfPosition(countPerSec, hz, position);

            return percentage<=pulseRate ? 1m : 0m;
        }
        public static double GetValueAsSinWave(long countPerSec,double hz,long position,double moveAngle)
        {
            double percentage = (double)GetPercentageOfPosition(countPerSec,hz, position);
            double radian = ConvertDgreeToRad(moveAngle + 360 * percentage);

            return Math.Sin(radian) / 2 + 0.5 ;
        }
        public static double GetHzFromNoteString(string note,int relation_count = 0)
        {
            note = note.Replace(" ", "");
            var match = Regex.Match(note, "([A-G])(#|b|)([0-8])");
            if(!match.Success || match.Length != note.Length)
            {
                return 0;
            }

            char noteChar = match.Groups[1].Value.Single();
            string blackKeyString = match.Groups[2].Value;
            int noteLevel = int.Parse(match.Groups[3].Value);

            double base_hz = 27.5;
            bool isBlackKey = !String.IsNullOrEmpty(blackKeyString);
            int blackKeyCorrection = isBlackKey ? blackKeyString.Single()=='#' ? 1 : -1 : 0;
            int note_number = noteChar - 'A';

            int index = note_number * 2 - ((note_number + 2) / 2 - 1);
            index += blackKeyCorrection;
            index += 12 * (note_number < 2 ? noteLevel : noteLevel-1);
            index += relation_count;

            return base_hz * Math.Pow(2, index / 12.0);

        }
    }
}
