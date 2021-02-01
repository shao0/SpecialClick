﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace SpecialClick
{                        
    class MouseHook
    {
        private Point point;
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
        private int hHook;
        //private static int hMouseHook = 0;
       //private const int WM_MOUSEMOVE = 0x200;//鼠标移动
        private const int WM_LBUTTONDOWN = 0x201;//鼠标左键按下
        private const int WM_RBUTTONDOWN = 0x204;//鼠标右键按下
        private const int WM_MBUTTONDOWN = 0x207;//鼠标中键按下
        private const int WM_LBUTTONUP = 0x202;//鼠标左键抬起
        private const int WM_RBUTTONUP = 0x205;//鼠标右键抬起
        private const int WM_MBUTTONUP = 0x208;//鼠标中键抬起
       // private const int WM_LBUTTONDBLCLK = 0x203;
       // private const int WM_RBUTTONDBLCLK = 0x206;
       // private const int WM_MBUTTONDBLCLK = 0x209;

        public const int WH_MOUSE_LL = 14;
        public Win32Api.HookProc hProc;
        public MouseHook()
        {
            this.Point = new Point();
        }
        public int SetHook()
        {
            hProc = MouseHookProc;
            hHook = Win32Api.SetWindowsHookEx(WH_MOUSE_LL, hProc, IntPtr.Zero, 0);
            return hHook;
        }
        public void UnHook()
        {
            Win32Api.UnhookWindowsHookEx(hHook);
        }
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


        public delegate void MouseMoveHandler(object sender, MouseEventArgs e);
        public event MouseMoveHandler MouseMoveEvent;

        public delegate void MouseClickHandler(object sender, MouseEventArgs e);
        public event MouseClickHandler MouseClickEvent;

        public delegate void MouseDownHandler(object sender, MouseEventArgs e);
        public event MouseDownHandler MouseDownEvent;

        public delegate void MouseUpHandler(object sender, MouseEventArgs e);
        public event MouseUpHandler MouseUpEvent;
        
        
    }
}