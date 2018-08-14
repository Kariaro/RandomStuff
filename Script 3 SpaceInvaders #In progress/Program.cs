using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceInvaders {
	class Program {
		public static char[] BREAK_1 = "\u2588\u2593\u2592\u2591 ".ToCharArray();
		public static char[] BREAK_2 = "\u2666¤\u25a0\u00b7 ".ToCharArray();
		public static char[] BREAK_3 = "\u2588\u263C\u25a0\u00b7 ".ToCharArray();

		public static void Main(string[] args) {
			new Program();
		}

		public double spaceship_x = 0;
		public bool proj = false;
		public int pro_x = 0;
		public int pro_y = 0;

		public Enemy enemy = new Enemy(80);
		public Program() {
			if(Console.BufferHeight != 80 || Console.BufferWidth != 80) {
				Console.SetWindowSize(80, 80);
				Console.SetBufferSize(80, 80);
			}
			Console.CursorVisible = false;

			Thread listener = new Thread(Keylistener);
			listener.Start();

			

			Console.ForegroundColor = ConsoleColor.White;
			long last = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			int wait = 150;

			RedrawShip(0);

			int r = 0;
			while(true) {
				long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				if(left) {
					if(now - lt > 300) left = false;
					if(spaceship_x > 0) RedrawShip(-0.2);
				}
				if(right) {
					if(now - rt > 300 && right) right = false;
					if(spaceship_x < 63) RedrawShip(0.2);
				}
				if(shoot && !proj) {
					shoot = false;
					proj = true;

					Shoot();
				}

				if(proj) {
					Console.SetCursorPosition(pro_x, pro_y + 1); Console.Write(" ");
					Console.SetCursorPosition(pro_x, pro_y); Console.Write(" ");
					pro_y--;

					for(int i = 0; i < 11; i++) {
						int ac = enemy.Active[i];
						if(ac < 0) continue;

						if(pro_x >= 5 + enemy.x + i * 6 && pro_x <= 10 + enemy.x + i * 6) {
							if(pro_y <= 9 + enemy.y + ac * 5) {
								int xp = enemy.x;
								if(5 - enemy.Active[i] > r) xp = enemy.lx;
								ClearAt(5 + xp + i * 6, 5 + enemy.y + ac * 5);
								enemy.Active[i]--;
								enemy.Hit(i, 4 - ac);
								pro_y = -1;
								break;
							}
						}
					}
					if(pro_y < 0) {
						proj = false;
						pro_y = 0;
					} else {
						Console.SetCursorPosition(pro_x, pro_y + 1); Console.Write("\u2502");
						Console.SetCursorPosition(pro_x, pro_y); Console.Write("\u2502");
					}
				}

				if(now - last > wait) {
					last += wait;
					wait = 1 * enemy.count;
					if(wait < 15) wait = 15;
				} else {
					Thread.Sleep(10);
					continue;
				}

				Console.Write("\u2588\u2588\u2588\u2588\u2588\u2588");
				Console.Write("\u2588\u2588\u2588\u2588\u2588\u2588");


				if(enemy.count < 1) {
					// WIN
					enemy.Reset();
				} else {
					enemy.RenderRow(5 + enemy.x, 1 + enemy.y + (5 - r) * 5, r, r / 2);
				
					r++;
					if(r >= enemy.B) {
						enemy.Move();
						r = enemy.T;
					}
				}
			}
			Console.ReadKey(true);
		}

		private void ClearAt(int x, int y) {
			for(int i = 0; i < 5; i++) {
				Console.SetCursorPosition(x, y + i); Console.Write("      ");
			}
		}

		public void Shoot() {
			pro_x = (int)spaceship_x + 3 + 5;
			pro_y = 67;
		}

		private void RedrawShip(double delta) {
			Console.SetCursorPosition((int)spaceship_x + 6, 69); Console.Write("     ");
			Console.SetCursorPosition((int)spaceship_x + 5, 70); Console.Write("       ");
			spaceship_x += delta;
			Console.SetCursorPosition((int)spaceship_x + 6, 69);
			Console.Write("\u2584\u2584\u2588\u2584\u2584");
			Console.SetCursorPosition((int)spaceship_x + 5, 70);
			Console.Write("\u2588\u2588\u2588\u2588\u2588\u2588\u2588");
		}

		private long lt = 0, rt = 0;
		private bool right = false;
		private bool left  = false;
		private bool shoot = false;

		private long last = 0;
		private bool pause = false;
		public void Pause() {
			if(pause) { // Un Pause
				last = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				pause = false;
			} else {

				pause = true;
			}
		}

		public void Keylistener() {
			last = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			while(true) {
				Thread.Sleep(10);
				if(pause) continue;

				long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				if(Console.KeyAvailable) {
					char c = char.ToLower(Console.ReadKey(true).KeyChar);
					if(c == 'a') {
						left = true;
						right = false;
						lt = now;
					}
					if(c == 'd') {
						right = true;
						left = false;
						rt = now;
					}
					if(c == ' ') shoot = true;
					if(c == 't') System.Environment.Exit(0);
				}
			}
		}
	}
}
