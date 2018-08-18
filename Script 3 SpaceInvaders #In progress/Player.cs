using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders {
	class Player {
		public Projectile proj = new Projectile();
		public double x;
		public int hp;
		
		private readonly int W = 0;
		public Player(int w) {
			W = w - 10;
			hp = 3;
		}

		public void Render(Render render) { // 20 B0 B1 B2 DB  DC DF
			render.Insert(6 + (int)x, 69, "\u00dc\u00dc\u00db\u00dc\u00dc");
			render.Insert(5 + (int)x, 70, "\u00db\u00db\u00db\u00db\u00db\u00db\u00db");


			render.Insert(3, 74, "You got  " + hp + " HP");

			for(int i = 0; i < 3; i++) {
				if(hp > i) {
					render.Insert(3 + 6 * i, 76, "\u00dc");
					render.Insert(1 + 6 * i, 77, "\u00db\u00db\u00db\u00db\u00db");
				} else
					render.Insert(1 + 6 * i, 77, "\u00b0\u00b0\u00b0\u00b0\u00b0");
			}
		}

		public void Shoot() {
			proj.Shoot((int)x + 8, 67, -1);
		}

		public void Hit(Projectile proj) {
			if(!proj.shoot) return;
			if(proj.x - 4 > x && proj.x - 5 < x + 7) {
				if(proj.y > 69) { // HIT
					hp--;
					proj.Stop();
				}
			}
		}

		public void Move(double dx) {
			x += dx;
			if(x <     0) x =     0;
			if(x > W - 7) x = W - 7;
		}

		public void Reset() {
			hp = 3;
			x = 0;
		}
	}
}
