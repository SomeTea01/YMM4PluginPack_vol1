using Vortice.Direct2D1;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Commons;
using SharpGen.Runtime;

namespace ShikitumeEx.Effect
{
    internal class ShikitumeExVideoProcessor : IVideoEffectProcessor
    {
        readonly ShikitumeExVideoEffect item;
        readonly Vortice.Direct2D1.Effects.Posterize posterizeEffect;
        ID2D1Image? input;
        bool isFirst = true;
        double posterize;
        public ID2D1Image Output { get; }

        public ShikitumeExVideoProcessor(IGraphicsDevicesAndContext devices, ShikitumeExVideoEffect item)
        {
            this.item = item;

            posterizeEffect = new Vortice.Direct2D1.Effects.Posterize(devices.DeviceContext);
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

            var posterize = item.GrayLevel.GetValue(frame, length, fps);
            if (isFirst || this.posterize != posterize)
            {
                posterizeEffect.RedValueCount = (int)posterize;
                posterizeEffect.GreenValueCount = (int)posterize;
                posterizeEffect.BlueValueCount = (int)posterize;
            }
            isFirst = false;
            this.posterize = posterize;

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
