using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace SpecialClick
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private DispatcherTimer _Timer;
        private Random _Random;
        private Win32Api _Api;
        private MouseHook mh;
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _Api = new Win32Api();
            _Random = new Random();
            //_Timer = new DispatcherTimer(DispatcherPriority.Render);
            //_Timer.Interval = TimeSpan.FromSeconds(0.1);
            //_Timer.Tick += TimerOnTick;
            //_Timer.Start();

            mh = new MouseHook();
            mh.SetHook();
            mh.MouseUpEvent += MhOnMouseUpEvent;
            //mh.MouseDownEvent += MhOnMouseUpEvent;
            Closed += (o, args) =>
            {
                mh.UnHook();
            };
        }

        private void MhOnMouseUpEvent(object sender, MouseEventArgs e)
        {
            var p = e.Location;
            for (int i = 0; i < 8; i++)
            {
                CreateSpecial(p.X, p.Y);
            }
        }

        private void TimerOnTick(object sender,
            EventArgs e)
        {
            //if (Mouse.LeftButton != MouseButtonState.Pressed) return;
            //POINT mouseStart = new POINT();
            //GetCursorPos(out mouseStart);
            //var mousePosition = new Point(mouseStart.X, mouseStart.Y);
            //var mousePosition = Mouse.GetPosition(_Grid);
            //CreateSpecial(mousePosition);
        }

        private void CreateSpecial(double x, double y)
        {
            Vector mouseVector = new Vector(x, y);
            Vector offsetVector = new Vector(_Random.Next(-25, 25), _Random.Next(-25, 25));
            Vector centerVector = new Vector(_Grid.ActualWidth / 2, _Grid.ActualHeight / 2);
            var resultVector = mouseVector + offsetVector - centerVector;
            Rectangle rectangle = new Rectangle();
            rectangle.StrokeThickness = _Random.Next(1, 4);
            rectangle.Stroke = Brushes.LightSkyBlue;
            rectangle.SetBinding(HeightProperty, new Binding("Width") { Source = rectangle });
            rectangle.IsHitTestVisible = false;
            TransformGroup transformGroup = new TransformGroup();
            var translate = new TranslateTransform();
            translate.X = resultVector.X;
            translate.Y = resultVector.Y;
            //RotateTransform rotate = new RotateTransform();
            //rotate.Angle = 15;
            //transformGroup.Children.Add (rotate);
            transformGroup.Children.Add(translate);
            rectangle.RenderTransform = transformGroup;
            _Grid.Children.Add(rectangle);
            var sizeAnimation = new DoubleAnimation
            {
                Duration = TimeSpan.FromMilliseconds(_Random.Next(100, 500)),
                From = 0,
                To = _Random.Next(40, 61)
            };
            Storyboard.SetTargetProperty(sizeAnimation, new PropertyPath("Width"));
            var opacityAnimation = new DoubleAnimation { From = 1, To = 0, Duration = sizeAnimation.Duration };
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            var storyBoard = new Storyboard();

            storyBoard.Children.Add(sizeAnimation);
            storyBoard.Children.Add(opacityAnimation);
            storyBoard.Completed += (o, args) => { _Grid.Children.Remove(rectangle); };
            rectangle.BeginStoryboard(storyBoard);
        }

        public struct POINT
        {
            public int X;
            public int Y;
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        private void OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }

        private void Window_OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized) return;
            WindowState = WindowState.Maximized;
            Activate();
        }
    }
}
