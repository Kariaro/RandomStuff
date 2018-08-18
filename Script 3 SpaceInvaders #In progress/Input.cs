using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInvaders {
	class Input {
		public Input() {

		}

		public int GetState(int key) {
			return GetKeyState(key);
		}

		[DllImport("user32.dll")]
		private static extern short GetKeyState(int nVirtKey);
	}
}
