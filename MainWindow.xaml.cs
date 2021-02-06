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
using System.Windows.Interop;
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
            HideAltTab();
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
            var time = TimeSpan.FromMilliseconds(_Random.Next(100, 500));
            var w = _Random.Next(40, 61);
            DoubleAnimationUsingKeyFrames sizeDoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
            sizeDoubleAnimationUsingKeyFrames.KeyFrames = new DoubleKeyFrameCollection();
            sizeDoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame
            {
                Value = 0,
                KeyTime = TimeSpan.FromMilliseconds(0),
            });
            sizeDoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame
            {
                Value = w,
                KeyTime = time,
            });
            Storyboard.SetTargetProperty(sizeDoubleAnimationUsingKeyFrames, new PropertyPath("Width"));
            DoubleAnimationUsingKeyFrames opacityDoubleAnimationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
            opacityDoubleAnimationUsingKeyFrames.KeyFrames = new DoubleKeyFrameCollection();
            opacityDoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame
            {
                Value = 1,
                KeyTime = TimeSpan.FromMilliseconds(0),
            });
            opacityDoubleAnimationUsingKeyFrames.KeyFrames.Add(new LinearDoubleKeyFrame
            {
                Value = 0,
                KeyTime = time,
            });
            Storyboard.SetTargetProperty(opacityDoubleAnimationUsingKeyFrames, new PropertyPath("Opacity"));
            var storyBoard = new Storyboard();

            storyBoard.Children.Add(sizeDoubleAnimationUsingKeyFrames);
            storyBoard.Children.Add(opacityDoubleAnimationUsingKeyFrames);
            storyBoard.Completed += (o, args) => { _Grid.Children.Remove(rectangle); };
            rectangle.BeginStoryboard(storyBoard);
        }
        private void HideAltTab()
        {
            var windowInterop = new WindowInteropHelper(this);
            int exStyle = (int)GetWindowLong(windowInterop.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(windowInterop.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void Window_OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized) return;
            WindowState = WindowState.Maximized;
            Activate();
        }
        #region Window styles
        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);
        #endregion
    }



}
