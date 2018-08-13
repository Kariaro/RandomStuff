using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RandomIdeas2 {
	class MouseTest {
		[StructLayout(LayoutKind.Sequential)]
		public struct POINT {
			public int X;
			public int Y;

			public static implicit operator Point(POINT point) {
				return new Point(point.X, point.Y);
			}
		}

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out POINT lpPoint);

		public struct Rect {
			public int Left { get; set; }
			public int Top { get; set; }
			public int Right { get; set; }
			public int Bottom { get; set; }
		}

		[DllImport("user32.dll")]
		public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

		public static Point Empty = new Point(-1, -1);
		public static Point GetCursorPosition(IntPtr window) {
			Rect rect = new Rect();
			GetWindowRect(window, ref rect);
			POINT point;
			GetCursorPos(out point);
			
			int x = (point.X - rect.Left - 10) / 8;
			int y = (point.Y - rect.Top  - 33) / 8;
			if(x < 0 || y < 0 || x >= Console.BufferWidth || y >= Console.BufferHeight) return Empty;

			return new Point(x, y);
		}

		public static Point GetCursorPosition2(IntPtr window) {
			Rect rect = new Rect();
			GetWindowRect(window, ref rect);
			POINT point;
			GetCursorPos(out point);
			
			int x = point.X - rect.Left - 10;
			int y = point.Y - rect.Top  - 33;
			return new Point(x, y);
		}
	}
}
