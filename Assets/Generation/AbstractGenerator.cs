using UnityEngine;
using System.Collections;
using System;

namespace Generation {
	public abstract class AbstractGenerator {
		protected TiledMap map;

		public void UseMap(TiledMap _map) {
			this.map = _map;
		}

		public TiledMap GetMap() {
			map.Normalize();
			return map;
		}

		public abstract void Run();
		public abstract Texture2D GetPreview(Gradient colors);
	}
}
