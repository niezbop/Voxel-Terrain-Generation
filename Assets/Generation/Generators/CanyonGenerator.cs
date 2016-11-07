using UnityEngine;
using System.Collections;
using Simplex;
using System;

namespace Generation {
	public class CanyonGenerator : AbstractGenerator {
		private System.Random rand;
		private int seed;

		public CanyonGenerator() {
			seed = (int) DateTime.Now.GetHashCode() & 0x0000FFFF;
			Noise.Seed = seed;
			rand = new System.Random(seed);
		}

		public CanyonGenerator(int _seed) {
			seed = _seed;
			Noise.Seed = seed;
			rand = new System.Random(seed);
		}

		public override void Run() {
			HeightOperations.AddNoise(map,0.1f,0.004f);
			HeightOperations.AddNoise(map,0.2f,0.01f);
			HeightOperations.AddNoise(map,0.015f,0.05f);
			HeightOperations.AddNoise(map,0.005f,0.1f);
			map.Normalize();
			HeightOperations.SimpleFlattenTopAndBottom(map);
		}

		public override Texture2D GetPreview(Gradient colors) {
			Texture2D tex = new Texture2D(map.width, map.length);
			float[,] tau = map.Height_Ratio_Map;

			for(int i = 0; i < map.width; i++) {
				for(int j = 0; j < map.length; j++) {
					tex.SetPixel(i,j, colors.Evaluate(tau[i,j]));
				}
			}

			tex.Apply();
			tex.wrapMode = TextureWrapMode.Clamp;
			tex.filterMode = FilterMode.Point;
			tex.name = "Procedural texture";

			return tex;
		}
	}
}
	