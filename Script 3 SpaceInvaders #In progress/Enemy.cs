using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders {
	class Enemy {
		private string[] SHAPE = {
			"\u0020\u2584\u0020\u2584\u0020",
			"\u2588\u2580\u2588\u2580\u2588",
			"\u2584\u2580\u0020\u2580\u2584",
			
			"\u0020\u2584\u2584\u2584\u0020",
			"\u2580\u2584\u2588\u2584\u2580",
			"\u2580\u2584\u0020\u2584\u2580",

			"\u0020\u0020\u2584\u0020\u0020",
			"\u2584\u2580\u2588\u2580\u2584",
			"\u2584\u2580\u0020\u2580\u2584",
		};

		private int WI = 0;
		public Enemy(int w) {
			WI = w - 10;
		}

		public int lx = 0;
		public int x = 0; // 80
		public int y = 0; // x

		public int L = 0;
		public int R = 11;
		public int T = 0;
		public int B = 5;

		public int count = 55;
		private bool[] ALIVE = {
			true, true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true, true,
		};
		public int[] Active = { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 };

		public void RenderRow(int x, int y, int r, int t) {
			string s1 = " ", s2 = " ", s3 = " ", s4 = " ";
			x += L * 6;
			for(int i = L; i < R; i++) {
				if(ALIVE[i + r * 11]) {
					s2 += SHAPE[t * 3 + 0] + " ";
					s3 += SHAPE[t * 3 + 1] + " ";
					s4 += SHAPE[t * 3 + 2] + " ";
				} else {
					s2 += "      ";
					s3 += "      ";
					s4 += "      ";
				}
				s1 += "      ";
			}

			Console.SetCursorPosition(x - 1, y - 1);
			Console.Write(s1);
			Console.SetCursorPosition(x - 1, y + 0);
			Console.Write(s2);
			Console.SetCursorPosition(x - 1, y + 1);
			Console.Write(s3);
			Console.SetCursorPosition(x - 1, y + 2);
			Console.Write(s4);
		}

		public void Reset() {

		}
		private bool dir = true;
		public void Move() {
			if(dir) { // RIGHT
				lx = x;
				x++;
				if(x > WI - R * 6 + 1) {
					dir = false;
					y++;
					x--;
				}
			} else {
				lx = x;
				x--;
				if(x < -(L * 6)) {
					dir = true;
					y++;
					x++;
				}
			}
		}

		public void Hit(int x, int y) {
			ALIVE[x + y * 11] = false;
			CalculateBounds();
		}

		private void CalculateBounds() {
			if(!(ALIVE[L]||ALIVE[L+11]||ALIVE[L+22]||ALIVE[L+33]||ALIVE[L+44])) L++;
			if(!(ALIVE[R-1]||ALIVE[R+10]||ALIVE[R+21]||ALIVE[R+32]||ALIVE[R+43])) R--;

			bool NT = true;
			for(int i = 0; i < 11; i++) if(ALIVE[i+T*11]) NT = false;
			if(NT) T++;

			bool NB = true;
			for(int i = 0; i < 11; i++) if(ALIVE[i+(B-1)*11]) NB = false;
			if(NB) B--;

			if(B <= L || R <= L) {
				// WON..
			}
			count = 0;
			for(int i = 0; i < ALIVE.Length; i++) if(ALIVE[i]) count++;
		}
	}
}
