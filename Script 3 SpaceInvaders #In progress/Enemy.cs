using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders {
	class Enemy {
		private string[] SHAPE = { // 20 B0 B1 B2 DB  DC DF
			/*"\u0020\u2584\u0020\u2584\u0020",
			"\u2588\u2580\u2588\u2580\u2588",
			"\u2584\u2580\u0020\u2580\u2584",
			
			"\u0020\u2584\u2584\u2584\u0020",
			"\u2580\u2584\u2588\u2584\u2580",
			"\u2580\u2584\u0020\u2584\u2580",

			"\u0020\u0020\u2584\u0020\u0020",
			"\u2584\u2580\u2588\u2580\u2584",
			"\u2584\u2580\u0020\u2580\u2584",*/

			"\u0020\u00dc\u0020\u00dc\u0020",
			"\u00db\u00df\u00db\u00df\u00db",
			"\u00dc\u00df\u0020\u00df\u00dc",
			
			"\u0020\u00dc\u00dc\u00dc\u0020",
			"\u00df\u00dc\u00db\u00dc\u00df",
			"\u00df\u00dc\u0020\u00dc\u00df",

			"\u0020\u0020\u00dc\u0020\u0020",
			"\u00dc\u00df\u00db\u00df\u00dc",
			"\u00dc\u00df\u0020\u00df\u00dc",
		};

		private readonly int W = 0;
		public Enemy(int w) {
			W = w - 10;
		}

		public int count = 55;
		public int lx = 0;
		public int ly = 0;
		public int x = 0; // 80
		public int y = 0; // x

		public int L = 0;
		public int R = 11;
		public int T = 0;
		public int B = 5;

		private bool[] ALIVE = {
			true, true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true, true,
		};
		public int[] Active = { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 };
		public Projectile proj = new Projectile();

		public void Render(Render render, int i) {
			int x = this.x;
			int y = this.y;
			
			for(int r = T; r < B; r++) {
				int t = r / 2;
				if(r == i) {
					x = lx;
					y = ly;
				}
				for(int j = L; j < R; j++) {
					if(!ALIVE[j + r * 11]) continue;

					render.Insert(5 + x + j * 6, 1 + y + (4 - r) * 5, SHAPE[t * 3 + 0]);
					render.Insert(5 + x + j * 6, 2 + y + (4 - r) * 5, SHAPE[t * 3 + 1]);
					render.Insert(5 + x + j * 6, 3 + y + (4 - r) * 5, SHAPE[t * 3 + 2]);
				}
			}

			render.Insert(0, 0, L + ", " + R + ", " + T + ", " + B);
		}

		private bool dir = true;
		public void Move() {
			if(dir) { // RIGHT
				lx = x;
				x++;
				ly = y;
				if(x > W - R * 6 + 1) {
					dir = false;
					y++;
					x--;
				}
			} else {
				lx = x;
				x--;
				ly = y;
				if(x < -(L * 6)) {
					dir = true;
					y++;
					x++;
				}
			}
		}

		public int xp = 0;
		public void Shoot(Player player) {
			if(R - L == 0) return;
			xp = ((int)player.x - x + 3) / 6;
			if(xp < 0 || xp > 10) return;

			proj.Shoot(5 + x + xp * 6 + 2, y + Active[xp] * 5 + 4, 1);
		}

		public void Hit(Projectile proj) {
			if(!proj.shoot) return;

			int xc = proj.x - x - 5;
			int xi = xc / 6;

			int yc = proj.y - y - 1;
			if(xi < 0 || xi > 10 || yc < 0) return;

			int id = Active[xi];
			if(id < 0) return;

			if(yc > id * 5 && yc < id * 5 + 5) {
				ALIVE[xi + (4 - Active[xi]) * 11] = false;
				Active[xi]--;

				proj.Stop();

				CalculateBounds();
			}
		}
		public void Hit(int x, int y) {
			ALIVE[x + y * 11] = false;
			CalculateBounds();
		}

		private void CalculateBounds() {
			for(int i = 0; i < 5; i++) { // Left
				if(ALIVE[L + 11 * i]) break;
				if(i == 4) L++;
			}
			for(int i = 0; i < 5; i++) { // Right
				if(ALIVE[R + 11 * i - 1]) break;
				if(i == 4) R--;
			}

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

		public void Reset() {
			x = y = xp = lx = 0;
			dir = true;

			count = ALIVE.Length;

			for(int i = 0; i < Active.Length; i++) Active[i] = 4;
			for(int i = 0; i < ALIVE.Length; i++) ALIVE[i] = true;

			R = 11;
			B =  5;
			L =  0;
			T =  0;
		}
	}
}
