using Vortice.Direct2D1;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Commons;
using SharpGen.Runtime;
using System.Numerics;
using System.Text.Json;

namespace CircularMotionVideoProcessor.Effect
{
    internal class DropFrameVideoProcessor : IVideoEffectProcessor
    {
        readonly DropFrameVideoEffect item;
        ID2D1Image? frame;
        DrawDescription drawDescription;
        ID2D1Image? input;
        public ID2D1Image Output => input ?? throw new NullReferenceException(nameof(input) + "is null");

        public DropFrameVideoProcessor(IGraphicsDevicesAndContext devices, DropFrameVideoEffect item)
        {
            this.item = item;
        }

        /// <summary>
        /// エフェクトを更新する
        /// </summary>
        /// <param name="effectDescription">エフェクトの描画に必要な各種情報</param>
        /// <returns>描画位置等の情報</returns>
        public DrawDescription Update(EffectDescription effectDescription)
        {
            var currentframe = effectDescription.ItemPosition.Frame;
            if (currentframe % (item.DropFrame + 1) == 0) {
                this.frame = input;
                drawDescription = effectDescription.DrawDescription;
            }
            if (this.frame == null || drawDescription == null)
            {
                return effectDescription.DrawDescription;
            }
            else
            {
                input = this.frame;
                return drawDescription;
            }

        }
        public void ClearInput()
        {
            input = null;
        }
        public void SetInput(ID2D1Image input)
        {
            this.input = input;
        }

        public void Dispose()
        {
        }
        public static double ConvertDgreeToRad(double degree,bool enableZeroToUp = false)
        {
            if(enableZeroToUp)degree -= 90;
            if (degree < 0) degree += 360;
            degree %= 360;
            return Math.PI * degree / 180;
        }
    }
}
