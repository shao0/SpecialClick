using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace SpecialClick
{
    /// <summary>
    /// 鼠标钩子类 - 用于捕获系统级鼠标事件
    /// 通过 Windows API 设置全局鼠标钩子，监听鼠标操作
    /// </summary>
    /// <remarks>
    /// 伪代码:
    /// 1. 定义鼠标事件常量（按下、抬起等）
    /// 2. 设置全局鼠标钩子
    /// 3. 处理捕获的鼠标消息
    /// 4. 触发相应的事件给订阅者
    /// </remarks>
    class MouseHook
    {
        /// <summary>
        /// 存储当前鼠标位置的私有变量
        /// </summary>
        private Point point;

        /// <summary>
        /// 鼠标位置属性 - 当位置改变时触发 MouseMoveEvent 事件
        /// </summary>
        private Point Point
        {
            get { return point; }
            set
            {
                if (point != value)
                {
                    point = value;
                    if (MouseMoveEvent != null)
                    {
                        var e = new MouseEventArgs(MouseButtons.None, 0, (int)point.X, (int)point.Y, 0);
                        MouseMoveEvent(this, e);
                    }
                }
            }
        }

        /// <summary>
        /// 鼠标钩子句柄
        /// </summary>
        private int hHook;

        //private static int hMouseHook = 0;
        //private const int WM_MOUSEMOVE = 0x200;//鼠标移动

        /// <summary>
        /// 鼠标左键按下消息常量
        /// </summary>
        private const int WM_LBUTTONDOWN = 0x201; //鼠标左键按下

        /// <summary>
        /// 鼠标右键按下消息常量
        /// </summary>
        private const int WM_RBUTTONDOWN = 0x204; //鼠标右键按下

        /// <summary>
        /// 鼠标中键按下消息常量
        /// </summary>
        private const int WM_MBUTTONDOWN = 0x207; //鼠标中键按下

        /// <summary>
        /// 鼠标左键抬起消息常量
        /// </summary>
        private const int WM_LBUTTONUP = 0x202; //鼠标左键抬起

        /// <summary>
        /// 鼠标右键抬起消息常量
        /// </summary>
        private const int WM_RBUTTONUP = 0x205; //鼠标右键抬起

        /// <summary>
        /// 鼠标中键抬起消息常量
        /// </summary>
        private const int WM_MBUTTONUP = 0x208; //鼠标中键抬起

        // private const int WM_LBUTTONDBLCLK = 0x203;
        // private const int WM_RBUTTONDBLCLK = 0x206;
        // private const int WM_MBUTTONDBLCLK = 0x209;

        /// <summary>
        /// 全局鼠标钩子类型常量
        /// </summary>
        public const int WH_MOUSE_LL = 14;

        /// <summary>
        /// 鼠标钩子处理程序委托
        /// </summary>
        public Win32Api.HookProc hProc;

        /// <summary>
        /// MouseHook 类的构造函数
        /// 初始化鼠标位置
        /// </summary>
        /// <remarks>
        /// 伪代码:
        /// 1. 创建一个新的 Point 对象作为初始鼠标位置
        /// </remarks>
        public MouseHook()
        {
            this.Point = new Point();
        }

        /// <summary>
        /// 设置鼠标钩子
        /// 安装全局鼠标钩子以捕获鼠标事件
        /// </summary>
        /// <returns>钩子句柄，如果设置失败则返回 0</returns>
        /// <remarks>
        /// 伪代码:
        /// 1. 将 MouseHookProc 方法赋值给 hProc 委托
        /// 2. 调用 Win32Api.SetWindowsHookEx 设置低级鼠标钩子
        /// 3. 返回钩子句柄
        /// </remarks>
        public int SetHook()
        {
            hProc = MouseHookProc;
            hHook = Win32Api.SetWindowsHookEx(WH_MOUSE_LL, hProc, IntPtr.Zero, 0);
            return hHook;
        }

        /// <summary>
        /// 卸载鼠标钩子
        /// 移除之前设置的全局鼠标钩子
        /// </summary>
        /// <remarks>
        /// 伪代码:
        /// 1. 调用 Win32Api.UnhookWindowsHookEx 函数卸载钩子
        /// </remarks>
        public void UnHook()
        {
            Win32Api.UnhookWindowsHookEx(hHook);
        }

        /// <summary>
        /// 鼠标钩子处理程序
        /// 处理捕获到的鼠标消息并触发相应的事件
        /// </summary>
        /// <param name="nCode">钩子代码，用于确定如何处理消息</param>
        /// <param name="wParam">消息的 wParam 参数</param>
        /// <param name="lParam">消息的 lParam 参数</param>
        /// <returns>调用下一个钩子的返回值</returns>
        /// <remarks>
        /// 伪代码:
        /// 1. 将 lParam 转换为 MouseHookStruct 结构体
        /// 2. 检查 nCode 是否小于 0，如果是则直接调用下一个钩子
        /// 3. 根据 wParam 的值判断鼠标操作类型（左键、右键、中键按下或抬起）
        /// 4. 创建相应的 MouseButtons 枚举值和点击次数
        /// 5. 触发对应的鼠标事件（MouseDownEvent 或 MouseUpEvent）
        /// 6. 更新当前鼠标位置
        /// 7. 调用下一个钩子处理程序
        /// </remarks>
        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Win32Api.MouseHookStruct MyMouseHookStruct = (Win32Api.MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32Api.MouseHookStruct));
            if (nCode < 0)
            {
                return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else
            {
                MouseButtons button = MouseButtons.None;
                int clickCount = 0;
                switch ((Int32)wParam)
                {
                    case WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        MouseDownEvent?.Invoke(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                    case WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        MouseDownEvent?.Invoke(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                    case WM_MBUTTONDOWN:
                        button = MouseButtons.Middle;
                        clickCount = 1;
                        MouseDownEvent?.Invoke(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                    case WM_LBUTTONUP:
                        button = MouseButtons.Left;
                        clickCount = 1;
                        MouseUpEvent?.Invoke(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                    case WM_RBUTTONUP:
                        button = MouseButtons.Right;
                        clickCount = 1;
                        MouseUpEvent?.Invoke(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                    case WM_MBUTTONUP:
                        button = MouseButtons.Middle;
                        clickCount = 1;
                        MouseUpEvent?.Invoke(this, new MouseEventArgs(button, clickCount, (int)point.X, (int)point.Y, 0));
                        break;
                }

                this.Point = new Point(MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y);
                return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        /// <summary>
        /// 鼠标移动事件委托定义
        /// </summary>
        /// <param name="sender">事件发送对象</param>
        /// <param name="e">鼠标事件参数</param>
        public delegate void MouseMoveHandler(object sender, MouseEventArgs e);

        /// <summary>
        /// 鼠标移动事件
        /// 当鼠标位置改变时触发
        /// </summary>
        public event MouseMoveHandler MouseMoveEvent;

        /// <summary>
        /// 鼠标点击事件委托定义
        /// </summary>
        /// <param name="sender">事件发送对象</param>
        /// <param name="e">鼠标事件参数</param>
        public delegate void MouseClickHandler(object sender, MouseEventArgs e);

        /// <summary>
        /// 鼠标点击事件
        /// 当鼠标点击时触发
        /// </summary>
        public event MouseClickHandler MouseClickEvent;

        /// <summary>
        /// 鼠标按下事件委托定义
        /// </summary>
        /// <param name="sender">事件发送对象</param>
        /// <param name="e">鼠标事件参数</param>
        public delegate void MouseDownHandler(object sender, MouseEventArgs e);

        /// <summary>
        /// 鼠标按下事件
        /// 当鼠标按键按下时触发
        /// </summary>
        public event MouseDownHandler MouseDownEvent;

        /// <summary>
        /// 鼠标抬起事件委托定义
        /// </summary>
        /// <param name="sender">事件发送对象</param>
        /// <param name="e">鼠标事件参数</param>
        public delegate void MouseUpHandler(object sender, MouseEventArgs e);

        /// <summary>
        /// 鼠标抬起事件
        /// 当鼠标按键抬起时触发
        /// </summary>
        public event MouseUpHandler MouseUpEvent;
        
        
    }
}
