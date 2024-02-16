using Vortice.Direct2D1;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Commons;
using SharpGen.Runtime;

namespace brightnessCorrectionEx.Effect
{
    internal class BrightnessCorrectionExEffectProcessor : IVideoEffectProcessor
    {
        readonly BrightnessCorrectionExEffect item;
        readonly Vortice.Direct2D1.Effects.Posterize posterizeEffect;
        ID2D1Image? input;
        public ID2D1Image Output { get; }

        public BrightnessCorrectionExEffectProcessor(IGraphicsDevicesAndContext devices, BrightnessCorrectionExEffect item)
        {
            this.item = item;
            Output = posterizeEffect.Output;

        }

        /// <summary>
        /// エフェクトを更新する
        /// </summary>
        /// <param name="effectDescription">エフェクトの描画に必要な各種情報</param>
        /// <returns>描画位置等の情報</returns>
        public DrawDescription Update(EffectDescription effectDescription)
        {
            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            return effectDescription.DrawDescription;
        }
        public void ClearInput()
        {
            posterizeEffect.SetInput(0, null, new(0));
        }
        public void SetInput(ID2D1Image input)
        {
            RawBool rawBool = new(0);
            posterizeEffect.SetInput(0, input, rawBool);
        }

        public void Dispose()
        {
            posterizeEffect.SetInput(0, null, new());
            Output.Dispose();
            posterizeEffect.Dispose();
        }
    }
}
