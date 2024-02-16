using Regularpolygon.CustomControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YukkuriMovieMaker.Commons;
using Regularpolygon.Object;
using Windows.ApplicationModel.Contacts.DataProvider;
using Regularpolygon.Struct;
using Windows.UI.WebUI;
using System;
using Regularpolygon.Common;
using System.Reflection;
using Windows.UI.Notifications;

namespace Regularpolygon
{
    /// <summary>
    /// IncreaseDecreaseButton.xaml の相互作用ロジック
    /// IPropertyEditorControlを実装する必要がある
    /// </summary>
    public partial class RegularValueControl : UserControl, IPropertyEditorControl
    {
        public PolygonBaseInfo ReturnValue
        {
            get { return (PolygonBaseInfo)GetValue(ReturnValueProperty); }
            set
            {
                SetValue(ReturnValueProperty, value);
            }
        }
        int[] drawParam = new int[0];
        public static readonly DependencyProperty ReturnValueProperty =
            DependencyProperty.Register(nameof(ReturnValue), typeof(PolygonBaseInfo), typeof(RegularValueControl), new FrameworkPropertyMetadata((new PolygonBaseInfo() {PolygoNum = 4,DrawMode = 0,NumberOfPolygonForDraw = -1}),FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private int _sliderValue;

        public int SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = value;
                this.PolygonNumberLabel.DataContext = new { SliderValue = _sliderValue };
            }
        }
        private int[] polyList;
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        private string[] DrawMethodIntems = new string[]
        {
            "正方形として描画",
            "n芒星として描画(一筆書き)",
            "n芒星として描画(図形)"
        };

        public RegularValueControl(int polygonSliderMin,int polygonSliderMax)
        {
            InitializeComponent();

            foreach (var item in DrawMethodIntems)
            {
                this.DrawMethodCombo.Items.Add(item);
            }
            this.slider.Minimum = polygonSliderMin;
            this.slider.Maximum = polygonSliderMax;
            this.slider.Value = 4;//ReturnValue.PolygoNum;

        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;

            BeginEdit?.Invoke(this,EventArgs.Empty);
            SliderValue = (int)this.slider.Value;
            ReturnValue = ReturnValue.CopyWithOverWrite(polygonNum: SliderValue);
            EndEdit?.Invoke(this, EventArgs.Empty);

            UpdateDrawParameters();

        }

        private void DrawMethodCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null) return;
            if (comboBox.SelectedIndex == -1) return;
            if (this.drawParameterCombo is null) return;
            UpdateDrawParameters();
            int index = comboBox.SelectedIndex;

            BeginEdit?.Invoke(this, EventArgs.Empty);
            if (!drawParameterCombo.IsEnabled) index = 0;//描画不可能だったら正多角形として描画
            ReturnValue = ReturnValue.CopyWithOverWrite(drawMode:index);
            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        private void drawParameterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            var drawMode = this.DrawMethodCombo.SelectedIndex;
            if(combo is null || !combo.IsEnabled)
            {
                ReturnValue = ReturnValue.CopyWithOverWrite(drawMode: 0);
                return;
            }

            BeginEdit?.Invoke(this, EventArgs.Empty);
            if(DrawMethodCombo.SelectedIndex == 1)  ReturnValue = ReturnValue.CopyWithOverWrite(drawMode:drawMode,numberOfSkipVertex:drawParam[combo.SelectedIndex]);
            if (DrawMethodCombo.SelectedIndex == 2) ReturnValue = ReturnValue.CopyWithOverWrite(drawMode:drawMode,numberOfPolygonForDraw: drawParam[combo.SelectedIndex]);
            EndEdit?.Invoke(this, EventArgs.Empty);

        }

        private void UpdateDrawParameters()
        {
            if (this.drawParameterCombo is null) return;
            int method = this.DrawMethodCombo.SelectedIndex;
            switch (method)
            {
                case 0:
                    InitializeDrawParameterItems();
                    break;
                case 1:
                    InitializeDrawParameterItems();
                    var nums = new List<int>(Enumerable.Range(2, Math.Max((int)Math.Round(SliderValue / 2d, MidpointRounding.ToEven), 1) - 1));
                    nums = nums.Where(item => OriginalMath.GetCoefficientToLeastCommonMultiple(item, SliderValue) * item == item * SliderValue).ToList();
                    foreach (var item in Enumerable.Range(1, nums.Count))
                    {
                        drawParameterCombo.Items.Add($"タイプ{item}({nums[item - 1]})");
                    }
                    drawParam = nums.ToArray();
                    if (nums.Count > 0)
                    {
                        this.drawParameterCombo.IsEnabled = true;
                        this.drawParameterCombo.SelectedIndex = 0;
                    }
                    break;
                case 2:
                    InitializeDrawParameterItems();
                    var polygons = PolygonCaluculator.GetPolygonsForDrawPolygon(SliderValue);
                    foreach (var item in polygons)
                    {
                        drawParameterCombo.Items.Add($"正{item}角形を使って描画する");
                    }
                    drawParam = polygons;
                    if (polygons.Length > 0)
                    {
                        this.drawParameterCombo.IsEnabled = true;
                        this.drawParameterCombo.SelectedIndex = 0;
                    }
                    break;
                case 3:
                    InitializeDrawParameterItems();
                    break;
            }
        }

        private void InitializeDrawParameterItems()
        {
            this.drawParameterCombo.IsEnabled = false;
            this.drawParameterCombo.SelectedIndex = -1;
            this.drawParameterCombo.Items.Clear();
        }
    }
}
