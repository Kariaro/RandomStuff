using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomIdeas2 {
	class ColorConverter {
		public struct CIEXYZ {
			private double x;
			private double y;
			private double z;

			public static readonly CIEXYZ D65 = new CIEXYZ(0.9505, 1.0, 1.0890);

			public double X {
				get {
					return this.x;
				}
				set {
					this.x = (value > 0.9505) ? 0.9505 : ((value < 0) ? 0 : value);
				}
			}

			public double Y {
				get {
					return this.y;
				}
				set {
					this.y = (value > 1.0) ? 1.0 : ((value < 0) ? 0 : value);
				}
			}

			public double Z {
				get {
					return this.z;
				}
				set {
					this.z = (value > 1.089) ? 1.089 : ((value < 0) ? 0 : value);
				}
			}

			public CIEXYZ(double x, double y, double z) {
				this.x = (x > 0.9505) ? 0.9505 : ((x < 0) ? 0 : x);
				this.y = (y > 1.0) ? 1.0 : ((y < 0) ? 0 : y);
				this.z = (z > 1.089) ? 1.089 : ((z < 0) ? 0 : z);
			}
		}
		public struct CIELab {
			public static readonly CIELab Empty = new CIELab();
			public double L {
				get {
					return this.l;
				}
				set {
					this.l = value;
				}
			}
			public double A {
				get {
					return this.a;
				}
				set {
					this.a = value;
				}
			}

			public double B {
				get {
					return this.b;
				}
				set {
					this.b = value;
				}
			}
			private double l;
			private double a;
			private double b;

			public CIELab(double l, double a, double b) {
				this.l = l;
				this.a = a;
				this.b = b;
			}
		}
		// RGB -> LAB & XYZ
		public static CIELab RGBtoLab(int rgb) {
			int r = (rgb & 0xff0000) / 65536;
			int g = (rgb & 0xff00) / 256;
			int b = (rgb & 0xff) / 1;
			return RGBtoLab(r, g, b);
		}
		public static CIELab RGBtoLab(int red, int green, int blue) {
			CIEXYZ xyz = RGBtoXYZ(red, green, blue);
			return XYZtoLab(xyz.X, xyz.Y, xyz.Z);
		}

		private static double Fxyz(double t) {
			return ((t > 0.008856) ? Math.Pow(t, (1.0 / 3.0)) : (7.787 * t + 16.0 / 116.0));
		}
		public static CIEXYZ RGBtoXYZ(int red, int green, int blue) { // Copied
			double rLinear = (double)red   / 255.0;
			double gLinear = (double)green / 255.0;
			double bLinear = (double)blue  / 255.0;

			double r = (rLinear > 0.04045) ? Math.Pow((rLinear + 0.055) / (1 + 0.055), 2.2) : (rLinear / 12.92);
			double g = (gLinear > 0.04045) ? Math.Pow((gLinear + 0.055) / (1 + 0.055), 2.2) : (gLinear / 12.92);
			double b = (bLinear > 0.04045) ? Math.Pow((bLinear + 0.055) / (1 + 0.055), 2.2) : (bLinear / 12.92);

			return new CIEXYZ(
				(r * 0.4124 + g * 0.3576 + b * 0.1805),
				(r * 0.2126 + g * 0.7152 + b * 0.0722),
				(r * 0.0193 + g * 0.1192 + b * 0.9505)
			);
		}

		public static CIELab XYZtoLab(double x, double y, double z) {
			CIELab lab = CIELab.Empty;

			lab.L = 116.0 *  Fxyz(y / CIEXYZ.D65.Y) - 16;
			lab.A = 500.0 * (Fxyz(x / CIEXYZ.D65.X) - Fxyz(y / CIEXYZ.D65.Y));
			lab.B = 200.0 * (Fxyz(y / CIEXYZ.D65.Y) - Fxyz(z / CIEXYZ.D65.Z));

			return lab;
		}

		// LAB & XYZ -> RGB
		public static int LabtoRGB(CIELab lab) {
			CIEXYZ xyz = LabtoXYZ(lab.L, lab.A, lab.B);
			return XYZtoRGB(xyz.X, xyz.Y, xyz.Z);
		}
		public static CIEXYZ LabtoXYZ(double l, double a, double b) {
			double delta = 6.0 / 29.0;

			double fy = (l + 16) / 116.0;
			double fx = fy + (a / 500.0);
			double fz = fy - (b / 200.0);

			return new CIEXYZ(
				(fx > delta) ? CIEXYZ.D65.X * (fx * fx * fx) : (fx - 16.0 / 116.0) * 3 * (delta * delta) * CIEXYZ.D65.X,
				(fy > delta) ? CIEXYZ.D65.Y * (fy * fy * fy) : (fy - 16.0 / 116.0) * 3 * (delta * delta) * CIEXYZ.D65.Y,
				(fz > delta) ? CIEXYZ.D65.Z * (fz * fz * fz) : (fz - 16.0 / 116.0) * 3 * (delta * delta) * CIEXYZ.D65.Z
			);
		}
		
		
		public static int XYZtoRGB(double x, double y, double z) {
			double[] Clinear = new double[3];
			Clinear[0] =  x * 3.2410 - y * 1.5374 - z * 0.4986; // red
			Clinear[1] = -x * 0.9692 + y * 1.8760 - z * 0.0416; // green
			Clinear[2] =  x * 0.0556 - y * 0.2040 + z * 1.0570; // blue

			for(int i = 0; i < 3; i++) {
				Clinear[i] = (Clinear[i] <= 0.0031308) ? 12.92 * Clinear[i] : (1 + 0.055) * Math.Pow(Clinear[i], (1.0 / 2.4)) - 0.055;
			}

			
			int r = Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", Clinear[0] * 255.0)));
			int g = Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", Clinear[1] * 255.0)));
			int b = Convert.ToInt32(Double.Parse(String.Format("{0:0.00}", Clinear[2] * 255.0)));

			r = r > 255 ? 255 : (r < 0 ? 0 : r);
			g = g > 255 ? 255 : (g < 0 ? 0 : g);
			b = b > 255 ? 255 : (b < 0 ? 0 : b);
			return r * 0x10000 + g * 0x100 + b;
		}
	}
}
