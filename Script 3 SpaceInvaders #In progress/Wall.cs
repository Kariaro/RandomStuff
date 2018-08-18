using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders {
	class Wall {
		private readonly int CL = 0;
		private readonly int H = 5;
		private readonly int L = 0;
		private readonly int C = 0;
		private int[] offset;
		private int[] Walls;

		public static char[] BREAK = "\u00b0\u00b1\u00b2\u00db".ToCharArray();

		public Wall(int width, int length, int count) {
			if(count < 2) count = 2;
			L = length;
			C = count;

			int distance = width - length - 20;
			double xs = distance / (count - 1.0);

			offset = new int[count];
			for(int i = 0; i < count; i++) {
				offset[i] = (int)(xs * i + 0.5);
			}

			CL = C * L;
			Walls = new int[5 * CL];
		}

		public void Render(Render render) {
			for(int y = 0; y < H; y++) {
				for(int o = 0; o < offset.Length; o++) {
					for(int x = 0; x < L; x++) {
						int id = Walls[y * CL + x + o * L];
						if(id < 0) continue;
						render.SetAt(10 + offset[o] + x, 55 + y, BREAK[id]);
					}
				}
			}
		}

		public bool Hit(Projectile p) {
			int index = InsideBounds(p.x, p.y, p.delta);
			if(index < 0) return false;

			Walls[index]--;
			if(index % 10 > 0) Walls[index -  1]--;
			if(index % 10 < 9) Walls[index +  1]--;
			if(index / CL > 0) Walls[index - CL]--;
			if(index / CL<H-1) Walls[index + CL]--;

			p.Stop();
			return true;
		}

		private int InsideBounds(int x, int y, int delta) {
			if(x < 10) return -1;
			x -= 10;
			y += delta;
			
			for(int i = 0; i < offset.Length; i++) {
				int o = offset[i];
				if(x >= o && x < o + L) {
					if(y > 54 && y < 60) {
						int xc = i * L + (x - o);
						int id = Walls[xc + (y - 55) * CL];
						if(id < 0) continue;
						return xc + (y - 55) * CL;
					}
				}
			}

			return -1;
		}

		public void Reset() {
			for(int i = 0; i < Walls.Length; i++) Walls[i] = 1;
		}
	}
}
