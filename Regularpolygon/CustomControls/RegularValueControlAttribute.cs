using Regularpolygon.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Views.Converters;

namespace Regularpolygon
{
    internal class RegularValueControlAttribute : PropertyEditorAttribute2
    {
        int polyNumberMin = 0;
        int polyNumberMax = 0;
        public RegularValueControlAttribute(int polygonNumber_min,int polygonNumber_max) {
            polyNumberMin = polygonNumber_min;
            polyNumberMax = polygonNumber_max;
        }

        /// <summary>
        /// コントロールを作成する
        /// ここで返すコントロールはIPropertyEditorControlを実装している必要がある
        /// </summary>
        /// <returns></returns>
        public override FrameworkElement Create()
        {
            return new RegularValueControl(polyNumberMin, polyNumberMax);
        }

        /// <summary>
        /// コントロールにバインディングを設定する（複数編集対応版）
        /// </summary>
        /// <param name="control">Create()で作成したコントロール</param>
        /// <param name="itemProperties">編集対象のプロパティ</param>
        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            ((RegularValueControl)control).SetBinding(RegularValueControl.ReturnValueProperty, ItemPropertiesBinding.Create(itemProperties));
        }

        /// <summary>
        /// バインディングを解除する
        /// </summary>
        /// <param name="control"></param>
        public override void ClearBindings(FrameworkElement control)
        {
            BindingOperations.ClearBinding(control, RegularValueControl.ReturnValueProperty);
        }

    }

}
