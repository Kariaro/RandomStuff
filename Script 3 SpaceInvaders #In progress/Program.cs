using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpaceInvaders {
	class Program {
		public static char[] BREAK_4 = " \u00b0\u00b1\u00b2\u00db".ToCharArray();

		public static char[] BREAK_1 = " \u2591\u2592\u2593\u2588".ToCharArray();
		public static char[] BREAK_2 = "\u2666¤\u25a0\u00b7 ".ToCharArray();
		public static char[] BREAK_3 = "\u2588\u263C\u25a0\u00b7 ".ToCharArray();

		public static void Main(string[] args) {
			new Program();
			// 20 B0 B1 B2 DB  DC DF
		}

		public Player player;
		public Enemy enemy;
		public Input input;
		public Wall wall;
		public Program() {
			Render render = new Render(80, 80);
			player = new Player(80);
			enemy  = new Enemy(80);
			input  = new Input();
			wall   = new Wall(80, 10, 4);
			Reset();

			Thread listener = new Thread(Keylistener);
			listener.Start();

			long last = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			long shot = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			int wait = 150;

			player.Render(render);

			int r = 0;
			while(true) {
				long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

				player.proj.Redraw(render);
				enemy .proj.Redraw(render);

				wall.Hit(enemy .proj);
				wall.Hit(player.proj);
				enemy.Hit(player.proj);
				player.Hit(enemy.proj);

				wall  .Render(render);
				player.Render(render);
				enemy .Render(render, r);

				// Uppdate
				render.UpdateScreen();


				if(now - shot > 2000) {
					shot += 2000;
					enemy.Shoot(player);
				}

				if(now - last > wait) {
					last += wait;
					wait = 3 * enemy.count;
					if(wait < 15) wait = 15;

					r++;
					if(r >= enemy.B) {
						enemy.Move();
						r = enemy.T;
					}
				} else {
					Thread.Sleep(10);
					continue;
				}

				if(enemy.count < 1) {
					// WIN
					Reset();
				}

			}
		}

		public void Reset() {
			player.Reset();
			enemy.Reset();
			wall.Reset();
		}

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
				if(now - last > 50) {
					last += 50;

					if((input.GetState('O') & 0x8000) > 0) {
						enemy.Shoot(player);
					}

					if((input.GetState(' ') & 0x8000) > 0) {
						player.Shoot();
					}

					if((input.GetState('D') & 0x8000) > 0) {
						player.Move(1);
					}

					if((input.GetState('A') & 0x8000) > 0) {
						player.Move(-1);
					}
				}

				if(Console.KeyAvailable) {
					char c = char.ToLower(Console.ReadKey(true).KeyChar);
					if(c == 't') System.Environment.Exit(0);
				}
			}
		}
	}
}
