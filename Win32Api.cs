using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpecialClick
{
    /// <summary>
    /// Win32 API 操作类
    /// 提供与 Windows API 交互的方法，主要用于设置和管理鼠标钩子
    /// </summary>
    /// <remarks>
    /// 伪代码:
    /// 1. 定义 POINT 结构体用于表示坐标点
    /// 2. 定义 MouseHookStruct 结构体用于存储鼠标钩子信息
    /// 3. 定义 HookProc 委托用于钩子回调处理
    /// 4. 提供导入的 Win32 函数用于设置、卸载和调用钩子
    /// </remarks>
    public class Win32Api
    {
        /// <summary>
        /// 表示坐标点的结构体
        /// 用于存储 x 和 y 坐标值
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            /// <summary>
            /// X 坐标
            /// </summary>
            public int x;

            /// <summary>
            /// Y 坐标
            /// </summary>
            public int y;
        }

        /// <summary>
        /// 鼠标钩子结构体
        /// 用于存储鼠标钩子相关信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            /// <summary>
            /// 鼠标位置点
            /// </summary>
            public POINT pt;

            /// <summary>
            /// 窗口句柄
            /// </summary>
            public int hwnd;

            /// <summary>
            /// 命中测试代码
            /// </summary>
            public int wHitTestCode;

            /// <summary>
            /// 额外信息
            /// </summary>
            public int dwExtraInfo;
        }

        /// <summary>
        /// 钩子过程委托
        /// 定义钩子回调函数的签名
        /// </summary>
        /// <param name="nCode">钩子代码，用于确定如何处理消息</param>
        /// <param name="wParam">消息的 wParam 参数</param>
        /// <param name="lParam">消息的 lParam 参数</param>
        /// <returns>钩子处理结果</returns>
        /// <remarks>
        /// 伪代码:
        /// 1. 接收系统传递的钩子代码和消息参数
        /// 2. 根据代码值决定如何处理消息
        /// 3. 返回处理结果给系统
        /// </remarks>
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);


        /// <summary>
        /// 安装钩子
        /// 设置一个系统范围的钩子，用于拦截和处理鼠标消息
        /// </summary>
        /// <param name="idHook">钩子类型</param>
        /// <param name="lpfn">钩子过程委托</param>
        /// <param name="hInstance">应用程序实例句柄</param>
        /// <param name="threadId">线程ID，0表示全局钩子</param>
        /// <returns>成功时返回钩子句柄，失败时返回0</returns>
        /// <remarks>
        /// 伪代码:
        /// 1. 调用 Windows API 的 SetWindowsHookEx 函数
        /// 2. 传入钩子类型、回调函数、实例句柄和线程ID
        /// 3. 返回系统创建的钩子句柄
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        /// <summary>
        /// 卸载钩子
        /// 移除之前设置的系统钩子
        /// </summary>
        /// <param name="idHook">要移除的钩子句柄</param>
        /// <returns>成功时返回 true，失败时返回 false</returns>
        /// <remarks>
        /// 伪代码:
        /// 1. 调用 Windows API 的 UnhookWindowsHookEx 函数
        /// 2. 传入要移除的钩子句柄
        /// 3. 返回操作是否成功
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// 调用下一个钩子
        /// 将消息传递给钩子链中的下一个钩子
        /// </summary>
        /// <param name="idHook">当前钩子句柄</param>
        /// <param name="nCode">钩子代码</param>
        /// <param name="wParam">消息的 wParam 参数</param>
        /// <param name="lParam">消息的 lParam 参数</param>
        /// <returns>下一个钩子的处理结果</returns>
        /// <remarks>
        /// 伪代码:
        /// 1. 调用 Windows API 的 CallNextHookEx 函数
        /// 2. 传入当前钩子句柄、钩子代码和消息参数
        /// 3. 返回下一个钩子的处理结果
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
    }
}