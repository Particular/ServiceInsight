namespace ServiceInsight.Framework.Settings
{
    using System.ComponentModel;
    using System.Windows;

    public class WindowStateSetting
    {
        [DefaultValue(double.NaN)]
        public double Left { get; set; }

        [DefaultValue(double.NaN)]
        public double Top { get; set; }

        [DefaultValue(450)]
        public double Width { get; set; }

        [DefaultValue(400)]
        public double Height { get; set; }

        [DefaultValue(typeof(WindowState), "Normal")]
        public WindowState WindowState { get; set; }
    }
}