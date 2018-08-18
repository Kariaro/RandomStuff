using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders {
	class Render {
		private static SafeFileHandle Handle => Kernel32.CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
		private static Kernel32.SmallRect WriteRegion;
		private Kernel32.CharInfo[] buffer;
		private Kernel32.Coord BufferCoord;
		private Kernel32.Coord BufferSize;
		
		public int W = 0;
		public int H = 0;
		public Render(int w, int h) {
			W = w;
			H = h;

			InitBufferSize(W, H);
			InitConsole();
		}


		public void Insert(int x, int y, string s) {
			for(int i = 0; i < s.Length; i++) {
				int index = i + x + y * W;
				buffer[index].Char.UnicodeChar = s[i];
			}
		}

		public void SetAt(int x, int y, char c) {
			buffer[x + y * W].Char.UnicodeChar = c;
		}

		public void UpdateScreen() {
			Kernel32.WriteConsoleOutput(Handle, buffer, BufferSize, BufferCoord, ref WriteRegion);

			for(int i = 0; i < buffer.Length; i++) buffer[i].Char.UnicodeChar = ' ';
		}

		private void InitConsole() {
			Console.CursorVisible = false;
			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Black;
		}

		private void InitBufferSize(int w, int h) {
			Console.WindowLeft = 0;
			Console.WindowTop  = 0;
			if(Console.WindowWidth != w || Console.WindowHeight != h) {
				Console.SetWindowSize(w, h);

				Console.WindowWidth  = w;
				Console.WindowHeight = h;

				Console.SetBufferSize(w, h);
			}

			WriteRegion = new Kernel32.SmallRect() { Left = 0, Top = 0, Right = (short)w, Bottom = (short)h };
			BufferCoord = new Kernel32.Coord(0, 0);
			BufferSize  = new Kernel32.Coord((short)w, (short)h);

			buffer = new Kernel32.CharInfo[w * h];
			for(int i = 0; i < w * h; i++) {
				buffer[i].Char.UnicodeChar = ' ';
				buffer[i].Attributes = 0x0f;
			}
		}
	}

	// Source https://stackoverflow.com/a/2754674/7937949
	internal static class Kernel32 {
		[DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern SafeFileHandle CreateFile(
			string fileName,
			[MarshalAs(UnmanagedType.U4)] uint fileAccess,
			[MarshalAs(UnmanagedType.U4)] uint fileShare,
			IntPtr securityAttributes,
			[MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
			[MarshalAs(UnmanagedType.U4)] int flags,
			IntPtr template);

		[DllImport("Kernel32.dll", SetLastError = true)]
		public static extern bool WriteConsoleOutput(
			SafeFileHandle hConsoleOutput,
			CharInfo[] lpBuffer,
			Coord dwBufferSize,
			Coord dwBufferCoord,
			ref SmallRect lpWriteRegion);

		[StructLayout(LayoutKind.Sequential)]
		public struct Coord {
			public short X;
			public short Y;

			public Coord(short x, short y) {
				X = x;
				Y = y;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct CharUnion {
			[FieldOffset(0)] public char UnicodeChar;
			[FieldOffset(0)] public byte AsciiChar;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct CharInfo {
			[FieldOffset(0)] public CharUnion Char;
			[FieldOffset(2)] public short Attributes;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct SmallRect {
			public short Left;
			public short Top;
			public short Right;
			public short Bottom;
		}
	}
}
