﻿using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Taction {

	internal static class WinApi {

		/// <summary>
		/// Leave focus on the current app.
		/// </summary>
		public static void CancelActivation(Window window) {

			var handle = GetHandle(window);
			var style = GetWindowLong(handle, (int)GetWindowLongPtr.GWL_EXSTYLE);
			style |= (int)WindowStyles.WS_EX_NOACTIVATE;
			SetWindowLong(handle, (int)GetWindowLongPtr.GWL_EXSTYLE, style);
		}

		/// <summary>
		/// Make window click-through.
		/// </summary>
		/// <param name="window"></param>
		/// <param name="isTransparent"></param>
		public static void SetWindowExTransparent(Window window, bool isTransparent) {

			var handle = GetHandle(window);
			var style = GetWindowLong(handle, (int)GetWindowLongPtr.GWL_EXSTYLE);
			if (isTransparent)
				style |= (int)WindowStyles.WS_EX_TRANSPARENT;
			else
				style &= ~(int)WindowStyles.WS_EX_TRANSPARENT;

			SetWindowLong(handle, (int)GetWindowLongPtr.GWL_EXSTYLE, style);
		}

		public static IntPtr GetHandle(Window window) {

			var handle = new WindowInteropHelper(window).EnsureHandle();
			return handle;
		}

		/**
		 * Pinvoke definitions
		 */

		public enum HookType : int {
			WH_JOURNALRECORD = 0,
			WH_JOURNALPLAYBACK = 1,
			WH_KEYBOARD = 2,
			WH_GETMESSAGE = 3,
			WH_CALLWNDPROC = 4,
			WH_CBT = 5,
			WH_SYSMSGFILTER = 6,
			WH_MOUSE = 7,
			WH_HARDWARE = 8,
			WH_DEBUG = 9,
			WH_SHELL = 10,
			WH_FOREGROUNDIDLE = 11,
			WH_CALLWNDPROCRET = 12,
			WH_KEYBOARD_LL = 13,
			WH_MOUSE_LL = 14
		}

		public enum WM : uint {
			WM_LBUTTONDOWN = 0x0201,
			WM_LBUTTONUP = 0x0202,
			WM_MOUSEMOVE = 0x0200,
			WM_MOUSEWHEEL = 0x020A,
			WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205,
			WM_ENTERSIZEMOVE = 0x0231,
			WM_EXITSIZEMOVE = 0x0232,
			WM_POINTERUPDATE = 0x0245,
		}

		public enum WindowStyles : uint {
			WS_EX_TRANSPARENT = 0x00000020,
			WS_EX_NOACTIVATE = 0x08000000,
		}

		public enum GetWindowLongPtr : int {
			GWL_EXSTYLE = -20
		}

		public enum FR : int {
			FR_PRIVATE = 16
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public int x;
			public int y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MSLLHOOKSTRUCT {
			public POINT pt;
			public int mouseData; // be careful, this must be ints, not uints (was wrong before I changed it...). regards, cmew.
			public int flags;
			public int time;
			public UIntPtr dwExtraInfo;
		}

		public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

		[DllImport("User32.dll")]
		public static extern bool SetCursorPos(int X, int Y);

		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetModuleHandle(string moduleName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll")]
		public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		[DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		// For ReleaseCapture (user32)
		[DllImport("User32.dll")]
		private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref int lParam);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int RemoveFontResourceEx(string lpszFilename, int fl, IntPtr pdv);
	}
}
