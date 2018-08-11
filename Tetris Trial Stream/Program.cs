using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleGameTesting {
	class Program {
		static void Main(string[] args) {
			Program program = new Program();
		}

		public string[] BlocksStr = {
			"........####....", // Stick
			"..#...#...#...#.",
			"........####....",
			"..#...#...#...#.",

			".....##..##.....", // Block
			".....##..##.....",
			".....##..##.....",
			".....##..##.....",

			".........###...#", // J Shape
			"......#...#..##.",
			".....#...###....",
			"......##..#...#.",

			".........###.#..", // L shape
			".....##...#...#.",
			".......#.###....",
			"......#...#...##",

			"..........##.##.", // Right snake
			"......#...##...#",
			"..........##.##.",
			"......#...##...#",

			".........##...##", // Left snake
			".......#..##..#.",
			".........##...##",
			".......#..##..#.",

			".........###..#.", // Pyramid
			"......#..##...#.",
			"......#..###....",
			"......#...##..#.",
		};
		public static Random random = new Random();

		public static bool DEBUG_SHAPES = false;

		public static int SCALE = 2;
		public static int HEIGHT = 30;
		public static int WIDTH = 20 / SCALE;

		public int[] map = new int[WIDTH * HEIGHT];
		public bool hit = false;

		public static int NextID = 0;
		
		public int[][] Block_Data = new int[7][];
		public static ConsoleColor[] color = {
			ConsoleColor.Red,
			ConsoleColor.Blue,
			ConsoleColor.Green,
			ConsoleColor.Gray,
			ConsoleColor.DarkGray,
			ConsoleColor.White,
			ConsoleColor.Yellow
		};

		public Program() {
			// Init for all the shapes
			for(int shape = 0; shape < 7; shape++) {
				Block_Data[shape] = new int[32]; // first
				if(DEBUG_SHAPES) Console.WriteLine("Shape Index: " + shape);
				for(int rot = 0; rot < 4; rot++) {
					string split = BlocksStr[shape * 4 + rot];
					if(DEBUG_SHAPES) Console.Write("Rot[" + rot + "]: str=\"" + split + "\"\n  { ");

					//int low = 0;

					for(int cut = 0; cut < 4; cut++) {
						string s = split.Substring(cut * 4, 4);
						int start = s.IndexOf('#');

						if(start < 0) {
							if(DEBUG_SHAPES) {
								Console.Write("0, 0");
								if(cut != 3) Console.Write(", ");
							}
							continue;
						}
						//if(start < low) low = start;
						Block_Data[shape][rot * 8 + cut * 2 + 0] = start;
						Block_Data[shape][rot * 8 + cut * 2 + 1] = s.LastIndexOf('#') - start + 1;

						if(DEBUG_SHAPES) {
							Console.Write(start + ", " + (s.LastIndexOf('#') - start + 1));
							if(cut != 3) Console.Write(", ");
						}
					}

					//Block_Data[shape][(rot + 1) * 9 - 1] = low;
					if(DEBUG_SHAPES) Console.WriteLine(" }");
				}
				if(DEBUG_SHAPES) Console.WriteLine("");
			}

			for(int i = 0; i < SCALE; i++) {
				BLOCK_ADD += '\u2588';
				SPACE_ADD += ' ';
			}

			Console.CursorVisible = false;

			long last = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

			// Start the KeyListener
			var KeyThread = new Thread(KeyListenerThread);
			KeyThread.Start();

			UpdateScreen();

			int lx = 0;
			int ly = 0;
			int lr = 0;
			int li = 0;
			Next();

			SpawnBlock(Next(), 0, 0, 0);
			DrawShapeSc(block_id, block_rot, block_x, block_y);


			while(true) {
				Thread.Sleep(10);

				long cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				if(cur - last > 500) {
					last += 500;

					if(CollideFall(block_y + 1)) {
						hit = true;
					} else block_y++;
				}

				if(hit) {
					AddShape(block_id, block_rot, block_x, block_y);
					CheckRow();
					UpdateScreen();

					SpawnBlock(Next(), 0, 0, 0);
					hit = false;

					DrawShapeSc(block_id, block_rot, block_x, block_y);
					lx = block_x; ly = block_y; lr = block_rot;
					continue;
				}

				if(block_x != lx || block_y != ly || block_rot != lr) {
					if(block_y > HEIGHT) {
						AddShape(block_id, block_rot, block_x, block_y);
						CheckRow();
						UpdateScreen();
						
						SpawnBlock(li, 0, block_x, 0);

						lx = block_x; ly = block_y; lr = block_rot;
						continue;
					}

					DrawShapeScClear(block_id, lr, lx, ly);
					DrawShapeSc(block_id, block_rot, block_x, block_y);
					lx = block_x; ly = block_y; lr = block_rot;
				}
			}
		}

		public int Next() {
			int Next = NextID;
			NextID = (int)(random.NextDouble() * 7);
			return Next;
		}
		public bool CollideSide(int x) {
			int[] Data = Block_Data[block_id];

			for(int i = 0; i < 4; i++) {
				int s = Data[block_rot * 8 + i * 2 + 0];
				int l = Data[block_rot * 8 + i * 2 + 1];
				if(l < 1) continue;
				if(x + s < 0) return true;
				if(x + s + l > WIDTH) return true;

				for(int j = x + s; j < x + s + l; j++) {
					int v = map[j + (block_y + i) * WIDTH];

					if(v > 0) return true;
				}
			}
			return false;
		}
		public bool CollideRotate(int rot) {
			rot = rot & 3;
			int[] Data = Block_Data[block_id];

			for(int i = 0; i < 4; i++) {
				int s = Data[rot * 8 + i * 2 + 0];
				int l = Data[rot * 8 + i * 2 + 1];
				if(l < 1) continue;
				if(block_y + i >= HEIGHT) return true;
				if(block_x + s < 0) return true;
				if(block_x + s + l > WIDTH) return true;


				for(int j = block_x + s; j < block_x + s + l; j++) {
					int v = map[j + (block_y + i) * WIDTH];

					if(v > 0) return true;
				}
			}
			return false;
		}

		public void CheckRow() {
			for(int i = 0; i < HEIGHT; i++) {
				bool sweep = true;
				for(int j = 0; j < WIDTH; j++) {

					int val = map[j + i * WIDTH];

					if(val < 1) {
						sweep = false;
						break;
					}
				}

				if(sweep) {
					// 0 # # # # vvvvvvv
					// 1 ####### # # # #
					for(int v = i; v > 0; v--) {
						for(int k = 0; k < WIDTH; k++)
							map[k + v * WIDTH] = map[k + (v - 1) * WIDTH];
					}
				}
			}
		}

		public bool CollideFall(int y) {
			int[] Data = Block_Data[block_id];

			for(int i = 0; i < 4; i++) {
				int s = Data[block_rot * 8 + i * 2 + 0];
				int l = Data[block_rot * 8 + i * 2 + 1];
				if(l < 1) continue;
				if(y + i >= HEIGHT) return true;
				
				for(int j = block_x + s; j < block_x + s + l; j++) {
					int v = map[j + (y + i) * WIDTH];

					if(v > 0) return true;
				}
			}

			return false;
		}
		public void MoveBlock(int x, int y) {
			int lx = block_x;
			int ly = block_y;

			int[] Data = Block_Data[block_id];

			for(int i = 0; i < 4; i++) {
				int s = Data[block_rot * 8 + i * 2 + 0];
				int l = Data[block_rot * 8 + i * 2 + 1];
				if(l < 1) continue;

				for(int j = x + s; j < x + s + l; j++) {
					int content = map[j + (y + i) * WIDTH];

				}
			}
		}

		public string[] XD = { "", " ", "  ", "   ", "    " };
		public void DrawShapeScClear(int id, int rot, int x, int y) {
			int[] Data = Block_Data[id];
			
			Console.ForegroundColor = color[id];
			for(int i = 0; i < 4; i++) {
				int s = Data[rot * 8 + i * 2 + 0];
				int l = Data[rot * 8 + i * 2 + 1];
				if(l < 1) continue;

				Console.SetCursorPosition(x * SCALE + s * SCALE, y * SCALE + i * SCALE + 0);
				Console.Write(XD[l].Replace(" ", SPACE_ADD));
				Console.SetCursorPosition(x * SCALE + s * SCALE, y * SCALE + i * SCALE + 1);
				Console.Write(XD[l].Replace(" ", SPACE_ADD));
			}
		}

		public void DrawShapeSc(int id, int rot, int x, int y) {
			int[] Data = Block_Data[id];

			Console.ForegroundColor = color[id];
			for(int i = 0; i < 4; i++) {
				int s = Data[rot * 8 + i * 2 + 0];
				int l = Data[rot * 8 + i * 2 + 1];
				if(l < 1) continue;

				Console.SetCursorPosition(x * SCALE + s * SCALE, y * SCALE + i * SCALE + 0);
				Console.Write(XD[l].Replace(" ", BLOCK_ADD));
				Console.SetCursorPosition(x * SCALE + s * SCALE, y * SCALE + i * SCALE + 1);
				Console.Write(XD[l].Replace(" ", BLOCK_ADD));
			}
		}

		public string BLOCK_ADD = "";
		public string SPACE_ADD = "";
		public void UpdateScreen() {
			int last = 0;
			string prt = "";
			for(int jv = 0; jv < HEIGHT * SCALE; jv++) {
				int j = jv / SCALE;
				last = map[j * WIDTH];
				Console.SetCursorPosition(0, jv);
				for(int i = 0; i < WIDTH; i++) {
					int val = map[i + j * WIDTH];

					/*if(last != val || i == WIDTH - 1) {
						if(i == WIDTH - 1) {
							if(val > 0) {
								prt += val + "" + val;
							} else {
								prt += SPACE_ADD;
							}
						}
						if(val > 0) Console.ForegroundColor = color[val - 1];
						Console.Write(prt);
						prt = "";
						last = val;

						if(i == WIDTH - 1) continue;
					}

					if(val > 0) {
						//prt += BLOCK_ADD;// '\u2588';
						prt += val + "" + val;
					} else {
						prt += SPACE_ADD;// ' ';
					}*/

					if(val > 0) {
						Console.ForegroundColor = color[val - 1];
						Console.Write(BLOCK_ADD);
					} else Console.Write(SPACE_ADD);
				}
			}

			Console.Write(prt);
			Console.ResetColor();
		}

		public int block_x = 0, block_y = 0;
		public int block_rot = 0;
		public int block_id  = 0;

		// X and Y should have a default value
		public void SpawnBlock(int id, int rot, int x, int y) {
			block_id  = id;
			block_rot = rot;
			block_x = x;
			block_y = y;

			if(CollideFall(block_y)) {
				// DIED
				ResetMap();
				UpdateScreen();
			}
		}

		public void ResetMap() {
			for(int i = 0; i < map.Length; i++) {
				map[i] = 0;
			}
		}


		public void AddShape(int id, int rot, int x, int y) {
			AddShape(id, rot, x, y, id + 1);
		}
		public void AddShape(int id, int rot, int x, int y, int c) {
			int[] Data = Block_Data[id];

			for(int i = 0; i < 4; i++) {
				int s = Data[rot * 8 + i * 2 + 0];
				int l = Data[rot * 8 + i * 2 + 1];
				if(l < 1) continue;

				for(int j = x + s; j < x + s + l; j++) map[j + (y + i) * WIDTH] = c;
			}
		}

		public void KeyListenerThread() {
			while(true) {
				if(Console.KeyAvailable) {
					char c = Char.ToLower(Console.ReadKey(true).KeyChar);

					if(c == 't') { // Terminate
						System.Environment.Exit(1);
						return;
					}
					if(c == 'a') { // left
						if(!CollideSide(block_x - 1)) block_x--;
					}
					if(c == 'd') { // right
						if(!CollideSide(block_x + 1)) block_x++;
					}
					if(c == 's') { // down
						if(CollideFall(block_y + 1)) {
							hit = true;
						} else block_y++;
					}
					if(c == 'r') { // rotate
						if(!CollideRotate(block_rot + 1)) {
							block_rot++;
							if(block_rot > 3) block_rot = 0;
						}
					}
				}
			}
		}

	}
}
