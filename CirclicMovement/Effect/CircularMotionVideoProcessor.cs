using Vortice.Direct2D1;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Commons;
using SharpGen.Runtime;
using System.Numerics;

namespace CircularMotionVideoProcessor.Effect
{
    internal class CircularMotionVideoProcessor : IVideoEffectProcessor
    {
        readonly CircularMotionVideoEffect item;
        ID2D1Image? input;
        bool isFirst = true;
        double posterize;
        public ID2D1Image Output => input ?? throw new NullReferenceException(nameof(input) + "is null");

        public CircularMotionVideoProcessor(IGraphicsDevicesAndContext devices, CircularMotionVideoEffect item)
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

            var frame = effectDescription.ItemPosition.Frame;
            var length = effectDescription.ItemDuration.Frame;
            var fps = effectDescription.FPS;

            var centerX = effectDescription.DrawDescription.Draw.X;
            var centerY = effectDescription.DrawDescription.Draw.Y;

            var speed = item.Speed;
            var angle = speed * frame / fps;
            angle += item.BaseAngle;
            var radian = ConvertDgreeToRad(angle,true);
            var r = item.r.GetValue(frame, length, fps);
            var aspect = item.aspect.GetValue(frame, length, fps);

            var x_aspect = 1 - aspect/100;
            var y_aspect = 1 + aspect/100;
            x_aspect = x_aspect > 1 ? 1 : x_aspect;
            y_aspect = y_aspect > 1 ? 1 : y_aspect;

            var base_x = Math.Round(Math.Cos(radian),10,MidpointRounding.AwayFromZero) * r * x_aspect;
            var base_y = Math.Round(Math.Sin(radian),10,MidpointRounding.AwayFromZero) * r * y_aspect;
            
            var movementRadian = ConvertDgreeToRad(item.movementAngle.GetValue(frame, length, fps));
            var x = base_x * Math.Cos(movementRadian) - base_y * Math.Sin(movementRadian) + centerX;
            var y = base_x * Math.Sin(movementRadian) + base_y * Math.Cos(movementRadian) + centerY;

            bool autoAdjustment = item.AutAdjustmentAngle;

            

            if (autoAdjustment)
            {
                var rotation = new Vector3(effectDescription.DrawDescription.Rotation.X, effectDescription.DrawDescription.Rotation.Y, effectDescription.DrawDescription.Rotation.Z + (float)angle - 90);
                return new DrawDescription(
                        effectDescription.DrawDescription,
                        draw: new System.Numerics.Vector3((float)x, (float)y, effectDescription.DrawDescription.Draw.Z), rotation: rotation);
            }
            else
            {
                return new DrawDescription(
                        effectDescription.DrawDescription,
                        draw: new System.Numerics.Vector3((float)x, (float)y, effectDescription.DrawDescription.Draw.Z));
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
