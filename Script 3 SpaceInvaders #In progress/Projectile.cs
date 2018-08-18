using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders {
	class Projectile {
		public bool shoot = false;
		public int delta = 0;
		public int x = 0;
		public int y = 0;

		public Projectile() {
		}

		public void Redraw(Render render) {
			if(!shoot) return;

			if(y < 1 || y > 78) {
				Stop();
				return;
			}

			render.SetAt(x, y + delta, '\u00b3');
			render.SetAt(x, y        , '\u00b3');

			Move();
		}

		public void Move() {
			if(!shoot) return;
			y += delta;

			if(y < 1 || y > 78) Stop();
		}

		public void Shoot(int x, int y, int d) {
			if(y < 1 || y > 78) return;
			if(shoot) return;
			shoot = true;

			this.x = x;
			this.y = y;
			delta = d;
		}

		public void Stop() {
			if(!shoot) return;
			shoot = false;
			y = -1;
			x = -1;
		}
	}
}
