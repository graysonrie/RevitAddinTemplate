using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RealRevitPlugin.WpfWindow.ApplicationLogic.Controls
{
    /// <summary>
    /// Interaction logic for ChromeButton.xaml
    /// </summary>
    public partial class ChromeButton : UserControl
    {
        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register("ButtonType", typeof(ChromeButtonType), typeof(ChromeButton),
                new PropertyMetadata(ChromeButtonType.Minimize, OnButtonTypeChanged));

        public ChromeButtonType ButtonType
        {
            get => (ChromeButtonType)GetValue(ButtonTypeProperty);
            set => SetValue(ButtonTypeProperty, value);
        }

        public static readonly DependencyProperty IconGeometryProperty =
            DependencyProperty.Register("IconGeometry", typeof(Geometry), typeof(ChromeButton),
                new PropertyMetadata(null));

        public Geometry IconGeometry
        {
            get => (Geometry)GetValue(IconGeometryProperty);
            private set => SetValue(IconGeometryProperty, value);
        }

        public ChromeButton()
        {
            InitializeComponent();
            UpdateButtonIcon();
        }

        private static void OnButtonTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChromeButton button)
            {
                button.UpdateButtonIcon();
            }
        }

        private void UpdateButtonIcon()
        {
            IconGeometry = ButtonType switch
            {
                ChromeButtonType.Minimize => Geometry.Parse("M 0,8 L 16,8"),
                ChromeButtonType.Maximize => Geometry.Parse("M 0,0 L 16,0 L 16,16 L 0,16 Z"),
                ChromeButtonType.Close => Geometry.Parse("M 0,0 L 16,16 M 0,16 L 16,0"),
                _ => null
            };

            if (ButtonType == ChromeButtonType.Close)
            {
                PART_Button.Style = FindResource("ChromeButtonStyle") as Style;
                PART_Button.Style = new Style(typeof(Button), PART_Button.Style);
                PART_Button.Style.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent));
                PART_Button.Style.Triggers.Add(new Trigger
                {
                    Property = Button.IsMouseOverProperty,
                    Value = true,
                    Setters = {
                        new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(232, 17, 35))),
                        new Setter(Button.ForegroundProperty, new SolidColorBrush(Colors.White))
                    }
                });
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null) return;

            switch (ButtonType)
            {
                case ChromeButtonType.Minimize:
                    window.WindowState = WindowState.Minimized;
                    break;
                case ChromeButtonType.Maximize:
                    window.WindowState = window.WindowState == WindowState.Maximized
                        ? WindowState.Normal
                        : WindowState.Maximized;
                    break;
                case ChromeButtonType.Close:
                    window.Close();
                    break;
            }
        }
    }

    public enum ChromeButtonType
    {
        Minimize,
        Maximize,
        Close
    }
}
