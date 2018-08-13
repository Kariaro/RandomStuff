using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace RandomIdeas2 {
	class Render {
		public int[] pixels;
		private int[] zbuff;
		public int HEIGHT;
		public int WIDTH;

		private readonly float[] b = new float[65536];

		public Render(int w, int h) {
			WIDTH = w;
			HEIGHT = h;
			pixels = new int[w * h];
			zbuff = new int[w * h];
			for(int i = 0; i < 65536; i++) {
				b[i] = (float)Math.Sin((double)i * 3.141592653589793D * 2.0D / 65536.0D);
			}
		}

		public void RenderTest() {
			random = new Random(1);
			clearZbuff();
			float[] N_U2 = { 0, 0, 100, 0, 0, 0, 100, 0, 100, 100, 0, 0, };
			float[] N_U = { 0, 0, 1, 0, 0, 0, 1, 0, 1, 1, 0, 0, };
			float[] N_D = { 0, 1, 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, };
			float[] N_L = { 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 1, 0, };
			float[] N_R = { 1, 0, 0, 1, 1, 0, 1, 0, 1, 1, 1, 1, };
			float[] N_F = { 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, };
			float[] N_B = { 1, 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, };

			//TriangleFan2(10, -10, 0, N_U2, 0x0100ff);
			//TriangleFan2(0, 0, 1, N_D, 0x00FF00);
			TriangleFan(0, 0, 1, N_L, 0x00FF00);
			TriangleFan(0, 0, 1, N_R, 0x00FF00);
			TriangleFan(0, 0, 1, N_F, 0x00FF00);

		}

		public void clearZbuff() {
			for(int i = 0; i < WIDTH * HEIGHT; i++) {
				zbuff[i] = 0xfffffff;
			}
		}

		public float sinf(float f) {
			return b[(int)(f * 10430.378) & '\uffff'];
		}
		public float cosf(float f) {
			return b[(int)(f * 10430.378 + 16384) & '\uffff'];
		}
		
		private Random random = new Random();
		public float EyeOffset = 0;
		public float Tx = 0;
		public float Ty = 0;
		public float Tz = 0;

		public float Rx = 0;
		public float Ry = 0;
		public float Rz = 0; // NEVER USED
		private void TriangleFan(float x, float y, float z, float[] ve, int color) {
			x += Tx; y += Ty + EyeOffset; z += Tz;
			int A = WIDTH / 2, B = HEIGHT / 2;
			float fov = 0.8f;

			float[] ver = Matrix(x, y, z, ve);
			for(int Triangle = 0; Triangle < (ver.Length / 3 - 2); Triangle++) {
				if((Triangle & 1) == 0) color = random.Next() & 0xffffff;

				int Id = Triangle * 3;
				float z1 = ver[Id + 2] * fov,
					  z2 = ver[Id + 5] * fov,
					  z3 = ver[Id + 8] * fov;

				if(z1 < 0.01 || z2 < 0.01 || z3 < 0.01) continue;
				float x1 = A + (ver[Id]) * (WIDTH / z1);
				float y1 = B + (ver[Id + 1]) * (HEIGHT / z1);
				float x2 = A + (ver[Id + 3]) * (WIDTH  / z2);
				float y2 = B + (ver[Id + 4]) * (HEIGHT / z2);
				float x3 = A + (ver[Id + 6]) * (WIDTH  / z3);
				float y3 = B + (ver[Id + 7]) * (HEIGHT / z3);
				x1 = (int)x1; y1 = (int)y1;
				x2 = (int)x2; y2 = (int)y2;
				x3 = (int)x3; y3 = (int)y3;

				if(((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1)) * ((Triangle & 1) * 2 - 1) < 0) continue;
				int y_m = y1 > y2 ? (y2 > y3 ? 1 : (y1 > y3 ? 2 : 0)) : (y1 > y3 ? 0 : (y2 > y3 ? 2 : 1));

				if(y_m == 0) {
					float x_s = (x2 - x3) / (y2 - y3);
					float x_sta = x_s * (y1 - y2) + x2;

					float z_s = (z2 - z3) / (y2 - y3);
					float z_sta = z_s * (y1 - y2) + z2;
					if(x_sta > x1) {
						if(y2 < y3) renderTriangle(y1, y2, y3, x1, x_sta, x3, x2, z1, z_sta, z3, z2, color);
						else renderTriangle(y1, y3, y2, x1, x_sta, x2, x3, z1, z_sta, z2, z3, color);
					} else {
						if(y2 < y3) renderTriangle(y1, y2, y3, x_sta, x1, x3, x2, z_sta, z1, z3, z2, color);
						else renderTriangle(y1, y3, y2, x_sta, x1, x2, x3, z_sta, z1, z2, z3, color);
					}
				} else if(y_m == 1) {
					float x_s = (x1 - x3) / (y1 - y3);
					float x_sta = x_s * (y2 - y1) + x1;

					float z_s = (z1 - z3) / (y1 - y3);
					float z_sta = z_s * (y2 - y1) + z1;
					if(x_sta > x2) {
						if(y1 < y3) renderTriangle(y2, y1, y3, x2, x_sta, x3, x1, z2, z_sta, z3, z1, color);
						else renderTriangle(y2, y3, y1, x2, x_sta, x1, x3, z2, z_sta, z1, z3, color);
					} else {
						if(y1 < y3) renderTriangle(y2, y1, y3, x_sta, x2, x3, x1, z_sta, z2, z3, z1, color);
						else renderTriangle(y2, y3, y1, x_sta, x2, x1, x3, z_sta, z2, z1, z3, color);
					}
				} else {
					float x_s = (x1 - x2) / (y1 - y2);
					float x_sta = x_s * (y3 - y1) + x1;

					float z_s = (z1 - z2) / (y1 - y2);
					float z_sta = z_s * (y3 - y1) + z1;
					if(x_sta > x3) {
						if(y1 < y2) renderTriangle(y3, y1, y2, x3, x_sta, x2, x1, z3, z_sta, z2, z1, color);
						else renderTriangle(y3, y2, y1, x3, x_sta, x1, x2, z3, z_sta, z1, z2, color);
					} else {
						if(y1 < y2) renderTriangle(y3, y1, y2, x_sta, x3, x2, x1, z_sta, z3, z2, z1, color);
						else renderTriangle(y3, y2, y1, x_sta, x3, x1, x2, z_sta, z3, z1, z2, color);
					}
				}
			}
		}

		public void renderTriangle(float y_mid, float y_max, float y_min, float x_sta, float x_end, float x_bot, float x_top, float z_sta, float z_end, float z_bot, float z_top, int col) {
			int H = HEIGHT;
			int W = WIDTH;
			float y_d0 = y_min - y_mid;

			y_mid = (int)y_mid;
			y_max = (int)y_max;
			y_min = (int)y_min;
			x_sta = (int)x_sta;
			x_end = (int)x_end;
			x_bot = (int)x_bot;
			x_top = (int)x_top;

			//col = (i++ & 1) == 0 ? 0xaaaaaa : 0xdddddd;
			if(y_d0 > 0) {
				float x13_s0 = (x_bot - x_sta) / y_d0;
				float x23_s0 = (x_bot - x_end) / y_d0;

				float z13_s0 = (z_bot - z_sta) / y_d0;
				float z23_s0 = (z_bot - z_end) / y_d0;
				float ys = 0;
				if(y_min > H) y_d0 = H - y_mid;
				if(y_mid < 0) ys = -y_mid;

				for(float yy = ys; yy < y_d0; yy++) {
					int yb = (int)(yy + y_mid) * W;
					if(yb >= W * H) return;

					int xe = (int)(yy * x23_s0 + x_end);
					int xs = (int)(yy * x13_s0 + x_sta);
					if(xe - xs == 0) continue;
					int oxs = xs;

					float zs = yy * z13_s0 + z_sta;
					float ze = yy * z23_s0 + z_end;
					float xes = (ze - zs) / (xe - xs);

					if(xs < 0) xs = 0;
					if(xe >= W) xe = W - 1;
					for(int xx = xs + yb; xx <= xe + yb; xx++) {
						float test = xes * (xx - yb - oxs) + zs;
						if(zbuff[xx] <= test * 0xfff) continue;
						zbuff[xx] = (int)(test * 0xfff);
						pixels[xx] = col;//(zbuff[xx] / 0xff) * 0x010101;//col;
					}
				}
			}
			//col = (i++ & 2) == 0 ? 0xffffff : 0x999999;
			float y_d1 = y_mid - y_max;
			if(y_d1 > 0) {
				//System.out.println(y_mid + ", " + y_max + ", " + y_d1 + ", " + y_d0 + ", " + x_sta + ", " + x_end + ", " + x_top + ", " + x_bot);
				float x13_s1 = (x_sta - x_top) / y_d1;
				float x23_s1 = (x_end - x_top) / y_d1;

				float z13_s1 = (z_sta - z_top) / y_d1;
				float z23_s1 = (z_end - z_top) / y_d1;
				float ys = 0;
				if(y_mid > H) y_d1 = H - y_max;
				if(y_max < 0) ys = -y_max;

				for(float yy = ys; yy < y_d1; yy++) {
					int yb = (int)(yy + y_max) * W;
					if(yb > W * H) return;
					int xe = (int)(yy * x23_s1 + x_top);
					int xs = (int)(yy * x13_s1 + x_top);
					if(xe - xs == 0) continue;
					int oxs = xs;

					float zs = yy * z13_s1 + z_top;
					float ze = yy * z23_s1 + z_top;
					float xes = (ze - zs) / (xe - xs);

					if(xs < 0) xs = 0;
					if(xe >= W) xe = W - 1;
					for(int xx = xs + yb; xx <= xe + yb; xx++) {
						float test = xes * (xx - yb - oxs) + zs;
						if(zbuff[xx] <= test * 0xfff) continue;
						zbuff[xx] = (int)(test * 0xfff);
						pixels[xx] = col;
					}
				}
			}
		}

		private float ATD = 180f / 3.141592653589793f;
		public float ToRadians(float angdeg) { return angdeg / ATD; }
		private float[] Matrix(float px, float py, float pz, float[] ver) {
			float[] ret = new float[ver.Length];
			
			float A = ToRadians(Rx);
			float B = ToRadians(Ry);
			//float C = ToRadians(Rz);
			float cosf_A = cosf(A);
			float sinf_A = sinf(A);
			float cosf_B = cosf(B);
			float sinf_B = sinf(B);
			//float cosf_C = MathUtils.cosf(C);
			//float sinf_C = MathUtils.sinf(C);
			for(int i = 0; i < ver.Length / 3; i++) {
				float x = ver[i * 3 + 0] + px;
				float y = ver[i * 3 + 1] + py;
				float z = ver[i * 3 + 2] + pz;
				float nx_A = x * cosf_A - z * sinf_A;
				//float ny_A = y;
				float nz_A = x * sinf_A + z * cosf_A;

				//float nx_B = nx_A;
				float ny_B = y * cosf_B - nz_A * sinf_B;
				float nz_B = y * sinf_B + nz_A * cosf_B;

				ret[i * 3 + 0] = nx_A;
				ret[i * 3 + 1] = ny_B;
				ret[i * 3 + 2] = nz_B;
			}
			return ret;
		}
	}
}
