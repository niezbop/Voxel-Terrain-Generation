using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VOXParsing;
using System;
using System.Linq;
using Simplex;

namespace Generation {
	public class CoastalMountainGenerator : AbstractGenerator, IVOXGenerator {
		private System.Random rand;
		private int seed;
		private const float SEA_LEVEl = 0.25f;
		private const int COLOR_SAMPLING = 128;

		public CoastalMountainGenerator() {
			seed = (int) DateTime.Now.GetHashCode() & 0x0000FFFF;
			Noise.Seed = seed;
			rand = new System.Random(seed);
		}

		public CoastalMountainGenerator(int _seed) {
			seed = _seed;
			Noise.Seed = seed;
			rand = new System.Random(seed);
		}

		public override void Run() {
			Debug.Log("Seed is: "+seed);
			HeightOperations.AddSlope(map,1f,rand);
			map.Normalize();
			HeightOperations.AddNoise(map,0.1f,0.01f);
			HeightOperations.AddNoise(map,0.015f,0.05f);
			HeightOperations.AddNoise(map,0.005f,0.1f);
			map.Normalize();
			HeightOperations.ApplyPowerLevel(map, SEA_LEVEl, 2f);
			map.Normalize();
			HeightOperations.Smooth(map, steps: 20, max_height: SEA_LEVEl);
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

		public VOXObject GenerateVOX(Gradient colors) {
			map.Normalize();

			Color palette_filler = new Color(128f, 128f, 128f), 
			sea_color = new Color(35f, 100f, 189f), 
			ground_color = new Color(73f, 36f, 14f);

			byte ground_index = (byte)(VOXObject.PALETTE_LENGTH - 1), sea_index = (byte)(VOXObject.PALETTE_LENGTH - 2);
			VOXObject vox_obj = new VOXObject((uint)map.width, (uint)map.length, (uint)map.height_limit);
			float[,] tau = map.Height_Ratio_Map;

			Color[] color_array = new Color[VOXObject.PALETTE_LENGTH];

			int SAMPLING = Math.Min((int)sea_index, COLOR_SAMPLING);
			for(int i = 0; i < SAMPLING; i++) {
				float ratio = (float)i/(float)SAMPLING;
				color_array[i] = colors.Evaluate(ratio) * 255;
			}

			uint I_sea_level = (uint)(SEA_LEVEl * vox_obj.size_z);
			Debug.Log("Sea level@"+I_sea_level);

			if(COLOR_SAMPLING < 254)
				for(int i = COLOR_SAMPLING; i < 254; i++) {
					color_array[i] = palette_filler;
				}

			color_array[ground_index] = ground_color;
			color_array[sea_index] = sea_color;

			int height_tenth = (int)(map.height_limit/10f), height_eighth = (int)(map.height_limit/8f);

			for(int i = 0; i < map.width; i++) {
				for(int j = 0; j < map.length; j++) {
					int k = 0;
					byte col = (byte)(tau[i,j] * (SAMPLING - 1));
					while(k < map[i,j].height && k < map.height_limit - 1) {
						vox_obj.voxels[i][j][k].empty = false;
						vox_obj.voxels[i][j][k].color = (byte)(ground_index + 1);
						k++;
					}
					vox_obj.voxels[i][j][k].empty = false;
					vox_obj.voxels[i][j][k].color = col;
					k++;
//					while(k <= map[i,j].height && k < map.height_limit - 1) {
//						vox_obj.voxels[i][j][k].empty = false;
//						vox_obj.voxels[i][j][k].color = col;
//						k++;
//					}
					while(k <= I_sea_level && k < map.height_limit - 1) {
						vox_obj.voxels[i][j][k].empty = false;
						vox_obj.voxels[i][j][k].color = (byte)(sea_index + 1);
						k++;
					}
				}
			}

			vox_obj.Palette = color_array;

			return vox_obj;
		}
	}
}
