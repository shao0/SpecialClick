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
    /// 主窗口类 - 实现特殊点击效果功能
    /// 继承自 Window 类，处理鼠标点击事件并创建动画效果
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 主窗口构造函数
        /// 初始化窗口组件并设置加载事件
        /// </summary>
        /// <remarks>
        /// 伪代码:
        /// 1. 调用 InitializeComponent() 初始化界面组件
        /// 2. 绑定 Loaded 事件到 OnLoaded 方法
        /// </remarks>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        // 定义私有成员变量
        // _Timer: 用于定时器功能（当前已禁用）
        // _Random: 用于生成随机数
        // _Api: Win32 API 实例
        // mh: 鼠标钩子实例
        private DispatcherTimer _Timer;
        private Random _Random;
        private Win32Api _Api;
        private MouseHook mh;
        
        /// <summary>
        /// 窗口加载完成时的事件处理方法
        /// 初始化各种组件和钩子
        /// </summary>
        /// <param name="sender">事件发送对象</param>
        /// <param name="e">路由事件参数</param>
        /// <remarks>
        /// 伪代码:
        /// 1. 创建 Win32Api 实例
        /// 2. 创建 Random 实例
        /// 3. 初始化鼠标钩子
        /// 4. 设置鼠标钩子事件处理程序
        /// 5. 设置窗口关闭时的清理操作
        /// 6. 调用 HideAltTab() 隐藏窗口从 Alt+Tab 列表
        /// </remarks>
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

        /// <summary>
        /// 鼠标按键抬起事件处理方法
        /// 在鼠标位置创建多个特殊效果元素
        /// </summary>
        /// <param name="sender">事件发送对象</param>
        /// <param name="e">鼠标事件参数</param>
        /// <remarks>
        /// 伪代码:
        /// 1. 获取鼠标当前位置
        /// 2. 循环 8 次，每次在鼠标位置附近创建一个特殊效果元素
        /// 3. 调用 CreateSpecial 方法创建效果
        /// </remarks>
        private void MhOnMouseUpEvent(object sender, MouseEventArgs e)
        {
            var p = e.Location;
            for (int i = 0; i < 8; i++)
            {
                CreateSpecial(p.X, p.Y);
            }
        }

        /// <summary>
        /// 定时器触发事件处理方法（当前已禁用）
        /// 用于处理鼠标持续按下时的效果
        /// </summary>
        /// <param name="sender">事件发送对象</param>
        /// <param name="e">事件参数</param>
        /// <remarks>
        /// 伪代码:
        /// 1. 检查鼠标左键是否被按下
        /// 2. 获取当前鼠标位置
        /// 3. 调用 CreateSpecial 方法创建效果
        /// </remarks>
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

        /// <summary>
        /// 创建特殊效果元素
        /// 在指定位置创建带动画的矩形元素
        /// </summary>
        /// <param name="x">X 坐标位置</param>
        /// <param name="y">Y 坐标位置</param>
        /// <remarks>
        /// 伪代码:
        /// 1. 创建随机偏移向量
        /// 2. 计算实际显示位置
        /// 3. 创建矩形元素并设置样式
        /// 4. 设置变换（平移）效果
        /// 5. 添加到网格容器
        /// 6. 创建大小动画效果
        /// 7. 创建透明度动画效果
        /// 8. 设置动画完成时移除元素
        /// 9. 开始播放动画
        /// </remarks>
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
        
        /// <summary>
        /// 隐藏窗口从 Alt+Tab 切换列表中
        /// 通过设置窗口扩展样式实现
        /// </summary>
        /// <remarks>
        /// 伪代码:
        /// 1. 获取窗口句柄
        /// 2. 获取当前扩展窗口样式
        /// 3. 添加 WS_EX_TOOLWINDOW 样式
        /// 4. 应用新的窗口样式
        /// </remarks>
        private void HideAltTab()
        {
            var windowInterop = new WindowInteropHelper(this);
            int exStyle = (int)GetWindowLong(windowInterop.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);
            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(windowInterop.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        /// <summary>
        /// 窗口状态改变事件处理方法
        /// 强制窗口保持最大化状态
        /// </summary>
        /// <param name="sender">事件发送对象</param>
        /// <param name="e">事件参数</param>
        /// <remarks>
        /// 伪代码:
        /// 1. 检查当前窗口状态是否为最大化
        /// 2. 如果不是最大化，则设置为最大化状态
        /// 3. 激活窗口以确保其获得焦点
        /// </remarks>
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
