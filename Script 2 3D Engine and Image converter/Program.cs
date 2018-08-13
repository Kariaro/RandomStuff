using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using static RandomIdeas2.ColorConverter;
using System.Runtime.InteropServices;

namespace RandomIdeas2 {
	class Program {
		public static char[] SHADE = "\u2588\u2593\u2592\u2591 ".ToCharArray();
		public static char[] SHADE2 = "\u2593\u2592".ToCharArray();

		public static ConsoleColor[] LIST_COLOR = {
			ConsoleColor.Black,
			ConsoleColor.DarkBlue,
			ConsoleColor.DarkGreen,
			ConsoleColor.DarkCyan,
			ConsoleColor.DarkRed,
			ConsoleColor.DarkMagenta,
			ConsoleColor.DarkYellow,
			ConsoleColor.Gray,
			ConsoleColor.DarkGray,

			ConsoleColor.Blue,
			ConsoleColor.Green,
			ConsoleColor.Cyan,
			ConsoleColor.Red,
			ConsoleColor.Magenta,
			ConsoleColor.Yellow,
			ConsoleColor.White
		};
		public static int[] LIST_RGB_COLOR = {
			0x000000, // BLACK
			0x000080, // DARK BLUE
			0x008000, // DARK GREEN
			0x008080, // DARK CYAN
			0x800000, // DARK RED
			0x800080, // DARK MAGENTA
			0x808000, // DARK YELLOW
			0xc0c0c0, // GRAY
			0x808080, // DARK GRAY
			0x0000ff, // BLUE
			0x00ff00, // GREEN
			0x00ffff, // CYAN
			0xff0000, // RED
			0xff00ff, // MAGENTA
			0xffff00, // YELLOW
			0xffffff, // WHITE
		};

		public static string RGBToHex(int rgb) {
			return RGBToHex((rgb & 0xff0000) / 65536, (rgb & 0xff00) / 256, rgb & 0xff);
		}
		public static string RGBToHex(int r, int g, int b) {
			return String.Format("{0:x2}{1:x2}{2:x2}", r, g, b).ToUpper();
		}

		public static void DisplayColorMap() {
			for(int i = 0; i < 2; i++) {
				for(int xy = 0; xy < 256; xy++) {
					if(xy % 16 == 0) Console.SetCursorPosition(18 * i, xy / 16);

					Console.ForegroundColor = LIST_COLOR[xy & 15];
					Console.BackgroundColor = LIST_COLOR[xy / 16];
					Console.Write(SHADE2[i]);
				}
			}
		}
		public static List<int> ExistID = new List<int>();
		public static List<int> Exist = new List<int>();
		public static void DisplayColorSheet() {
			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Black;
			for(int i = 0; i < Exist.Count; i++) {
				int rgb = Exist.ElementAt(i);
				int id = ExistID.ElementAt(i);
				int xy = id & 255;
				Console.Write(RGBToHex(rgb));

				Console.ForegroundColor = LIST_COLOR[xy & 15];
				Console.BackgroundColor = LIST_COLOR[xy / 16];
				Console.Write(SHADE2[id / 256]);

				Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.Write(" ");
			}
		}

		public static void DisplayColorSheet2() {
			int W = Console.BufferWidth / 8;
			for(int i = 0; i < Exist.Count; i++) {
				int rgb = Exist.ElementAt(i);

				int r = (rgb & 0xff0000) / 65536;
				int g = (rgb &   0xff00) /   256;
				int b = (rgb &     0xff) /     1;
				Rectangle rect = new Rectangle(56 + (i % W) * 64, (i / W) * 8, 8, 8);
				SolidBrush brush = new SolidBrush(Color.FromArgb(255, r, g, b));
				graphics.FillRectangle(brush, rect);
				brush.Dispose();
			}
		}

		public static double GetBrightness(int rgb) {
			return GetBrightness((rgb & 0xff0000) >> 16, (rgb & 0xff00) >> 8, rgb & 0xff);
		}
		public static double GetBrightness(int r, int g, int b) {
			return r * r * 0.241
				 + g * g * 0.691
				 + b * b * 0.068;
		}

		public static readonly int BLEND_COLOR = 0;
		public static readonly int PERCEIVED_COLOR = 1;
		public static void Initialize(int type) {
			if(type == BLEND_COLOR) {
				for(int i = 0; i < 2; i++) {
					for(int xy = 0; xy < 256; xy++) {
						int rgb1 = LIST_RGB_COLOR[xy & 15];
						int rgb2 = LIST_RGB_COLOR[xy / 16];

						int r1 = (rgb1 & 0xff0000) / 65536;
						int g1 = (rgb1 &   0xff00) /   256;
						int b1 = (rgb1 &     0xff) /     1;

						int r2 = (rgb2 & 0xff0000) / 65536;
						int g2 = (rgb2 &   0xff00) /   256;
						int b2 = (rgb2 &     0xff) /     1;

						if(i == 0) { // 75% Foreground  25% Background
							double r = (r1 * 3 + r2) / 4.0;
							double g = (g1 * 3 + g2) / 4.0;
							double b = (b1 * 3 + b2) / 4.0;
							r = (r > 255 ? 255 : (r < 0 ? 0 : r));
							g = (g > 255 ? 255 : (g < 0 ? 0 : g));
							b = (b > 255 ? 255 : (b < 0 ? 0 : b));

							int rgb = (int)r * 0x10000 + (int)g * 0x100 + (int)b;
							if(!Exist.Contains(rgb)) {
								Exist.Add(rgb);
								ExistID.Add(i * 256 + xy);
							}
						} else { // 50% Foreground  50% Background
							double r = (r1 + r2) / 2.0;
							double g = (g1 + g2) / 2.0;
							double b = (b1 + b2) / 2.0;
							//r = Math.Sqrt(r1 * r1 + r2 * r2);
							//g = Math.Sqrt(g1 * g1 + g2 * g2);
							//b = Math.Sqrt(b1 * b1 + b2 * b2);

							r = (r > 255 ? 255 : (r < 0 ? 0 : r));
							g = (g > 255 ? 255 : (g < 0 ? 0 : g));
							b = (b > 255 ? 255 : (b < 0 ? 0 : b));

							int rgb = (int)r * 0x10000 + (int)g * 0x100 + (int)b;
							if(!Exist.Contains(rgb)) {
								Exist.Add(rgb);
								ExistID.Add(i * 256 + xy);
							}
						}
					}
				}
			} else if(type == PERCEIVED_COLOR) {
				for(int i = 0; i < 2; i++) {
					for(int xy = 0; xy < 256; xy++) {
						int rgb1 = LIST_RGB_COLOR[xy & 15];
						int rgb2 = LIST_RGB_COLOR[xy / 16];

						CIELab lab1 = ColorConverter.RGBtoLab(rgb1);
						CIELab lab2 = ColorConverter.RGBtoLab(rgb2);

						if(i == 0) { // 75% Foreground  25% Background
							double l = (lab1.L * 3 + lab2.L) / 4.0;
							double a = (lab1.A * 3 + lab2.A) / 4.0;
							double b = (lab1.B * 3 + lab2.B) / 4.0;
							CIELab lab = new CIELab(l, a, b);

							int rgb = ColorConverter.LabtoRGB(lab);
							if(!Exist.Contains(rgb)) {
								Exist.Add(rgb);
								ExistID.Add(i * 256 + xy);
							}
						} else { // 50% Foreground  50% Background
							double l = (lab1.L + lab2.L) / 2.0;
							double a = (lab1.A + lab2.A) / 2.0;
							double b = (lab1.B + lab2.B) / 2.0;
							CIELab lab = new CIELab(l, a, b);

							int rgb = ColorConverter.LabtoRGB(lab);
							if(!Exist.Contains(rgb)) {
								Exist.Add(rgb);
								ExistID.Add(i * 256 + xy);
							}
						}
					}
				}
			}
		}
		public static readonly int AVERAGE_COLOR = 0;
		public static readonly int AVERAGE_COLOR_WITH_WEIGHT = 2;
		public static readonly int AVERAGE_LAB   = 1;
		public static readonly int MONOCHROME = 2;
		public static int ClosestColor(Color col, int type) {
			if(type == AVERAGE_COLOR) {
				int error = 196609;
				int ret   = -1;
				for(int i = 0; i < Exist.Count; i++) {
					int rgb = Exist.ElementAt(i);
					int r = (rgb & 0xff0000) / 65536;
					int g = (rgb &   0xff00) /   256;
					int b = (rgb &     0xff) /     1;

					int R = (col.R - r);
					int G = (col.G - g);
					int B = (col.B - b);

					int RGB = R*R + G*G + B*B;
					if(error > RGB) {
						error = RGB;
						ret = ExistID.ElementAt(i);
					}
				}
				return ret;
			} else if(type == AVERAGE_COLOR_WITH_WEIGHT) {
				double error = 100000000;
				int ret   = -1;
				for(int i = 0; i < Exist.Count; i++) {
					int rgb = Exist.ElementAt(i);
					int r = (rgb & 0xff0000) / 65536;
					int g = (rgb &   0xff00) /   256;
					int b = (rgb &     0xff) /     1;

					double R = (col.R - r) * 0.30;
					double G = (col.G - g) * 0.59;
					double B = (col.B - b) * 0.11;

					double er = R*R + G*G + B*B;
					if(error > er) {
						error = er;
						ret = ExistID.ElementAt(i);
					}
				}
				return ret;
			} else if(type == AVERAGE_LAB) {
				double error = 1000000000;
				int ret   = -1;
				CIELab lab_c = ColorConverter.RGBtoLab(col.ToArgb() & 0xffffff);
				for(int i = 0; i < Exist.Count; i++) {
					int rgb = Exist.ElementAt(i);
					CIELab lab = ColorConverter.RGBtoLab(rgb);

					double L = (lab_c.L - lab.L);
					double A = (lab_c.A - lab.A);
					double B = (lab_c.B - lab.B);

					double er = L*L + A*A + B*B;
					if(error > er) {
						error = er;
						ret = ExistID.ElementAt(i);
					}
				}
				return ret;
			}

			return -1;
		}

		public static int[] Image;
		public static void RenderImage(Render render) {
			Console.SetCursorPosition(0, 0);
			string ptr = "";
			double d = 5.0 / 257.0;
			for(int i = 0; i < render.pixels.Length; i++) {
				int col = render.pixels[i];
				double mono = (0.2125 * ((col & 0xff0000) / 65536)) + (0.7154 * ((col & 0xff00) / 256)) + (0.0721 * (col & 0xff));
				int id = 4 - (int)(d * mono);
				ptr += SHADE[id];
			}
			Console.Write(ptr);
			Console.ResetColor();
		}
		public static void RenderImage(int[] Image) {
			Console.Clear();
			Console.SetCursorPosition(0, 0);
			for(int i = 0; i < Image.Length; i++) {
				int id = Image[i];
				int SI = id / 256;
				int XY = id & 255;

				Console.ForegroundColor = LIST_COLOR[XY & 15];
				Console.BackgroundColor = LIST_COLOR[XY / 16];
				Console.Write(SHADE2[SI]);
			}
			Console.ResetColor();
		}

		public static void RenderImage(Bitmap image, int type) {
			Console.Clear();
			int H = image.Height;
			int W = image.Width;
			Image = new int[W * H];
			for(int y = 0; y < H; y++) {
				for(int x = 0; x < W; x++) {
					Color col = image.GetPixel(x, y);
					
					Image[x + y * W] = ClosestColor(col, type);
				}
			}
			RenderImage(Image);
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetConsoleWindow();

		static double xa = 0;
		static double ya = 0;
		static double za = 0;

		static IntPtr window;
		static Render render;
		static Graphics graphics;
		static void Main(string[] args) {
			render = new Render(80, 80);
			render.EyeOffset = 0.6f;
			render.RenderTest();

			Process process = Process.GetCurrentProcess();
			window = GetConsoleWindow();
			graphics = Graphics.FromHwnd(window);

			Initialize(PERCEIVED_COLOR);
			DisplayColorSheet();
			DisplayColorSheet2();

			Console.ReadKey(true);

			Console.SetCursorPosition(0, 0);

			Bitmap image = new Bitmap("Pixelard.png"); //"Color3.png");// 
			int W = image.Width;
			int H = image.Height;
			Console.SetWindowSize(W, H + 1);
			Console.SetBufferSize(W, H + 1);
			/*
			Thread thread = new Thread(KeyboardListener);
			thread.Start();

			while(true) {
				Thread.Sleep(100);

				render.RenderTest();
				RenderImage(render);
				for(int i = 0; i < render.pixels.Length; i++) render.pixels[i] = 5;
			}*/
			RenderImage(image, AVERAGE_COLOR);
			Console.Write("AVERAGE_COLOR");
			Console.ReadKey(true);
			/*
			int x = 0;
			int y = 0;
			while(true) {
				Thread.Sleep(10);
				
				Rect rect = new Rect();
				GetWindowRect(window, ref rect);
				Point point = GetCursorPosition();
				point.Offset(-rect.Left - 10, -rect.Top - 33);

				int nx = point.X / 8;
				int ny = point.Y / 8;
				if(nx < 0 || ny < 0 || nx >= W || ny >= H) continue;
				if(x != nx || y != ny) {
					//Console.SetCursorPosition(nx, ny);
					//Console.Write("\u2588");
					x = nx;
					y = ny;
				}

				if(Console.KeyAvailable) {
					char c = char.ToLower(Console.ReadKey(true).KeyChar);

					if(c == 'w') {
						RenderImage(Image[x + y * Console.BufferWidth]);
					}
				}
			}*/
			Console.ReadLine();
		}

		public static void KeyboardListener() {
			float speed = 0.06f;

			while(true) {
				Thread.Sleep(1);
				int ff = 0;
				int ss = 0;

				if(Console.KeyAvailable) {
					char c = char.ToLower(Console.ReadKey(true).KeyChar);
					if(c == 'w') ff++;
					if(c == 'a') ss--;
					if(c == 's') ff--;
					if(c == 'd') ss++;
				}

				Point p = MouseTest.GetCursorPosition2(window);
				int rx = p.X;
				rx %= 360;
				int ry = p.Y;
				if(ry > 90) ry = 90;
				if(ry < -90) ry = -90;
				render.Rx = rx;
				render.Ry = ry;
				float ra = render.ToRadians(rx);

				za -= ff * Math.Cos(ra) - ss * Math.Sin(ra);
				xa -= ff * Math.Sin(ra) + ss * Math.Cos(ra);
				render.Tx += (float)xa * speed;
				render.Tz += (float)za * speed;
				//render.Ty = y;

				za *= 0.5;
				xa *= 0.5;
			}
		}

		public static void RenderToScreen(int[] pixels) {
			//ConsoleColor.
		}
	}
}
