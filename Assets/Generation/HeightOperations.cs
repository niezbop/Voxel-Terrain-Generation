using UnityEngine;
using System.Collections;
using Simplex;

namespace Generation {
	public static class HeightOperations {

		/// <summary>
		/// Adds a random slope to the map.
		/// </summary>
		/// <param name="map">The map.</param>
		/// <param name="intensity">Intensity of the slope (1 meaning that the diagonal length of the map is equal to the maximum increase of the map, <1 will have a lesser influence, and >1 will have a greater).</param>
		/// <param name="random">The random engine to use.</param>
		public static void AddSlope(TiledMap map, float intensity, System.Random random) {
			float a = random.Next(-100, 100)/100f;
			float b = random.Next(-100, 100)/100f;

			float increase = Mathf.Sqrt((float)(map.width + map.length)) * intensity;

//			Debug.Log("a, b : "+ a + ", " + b);

			for(int i = 0; i < map.width; i++) {
				for(int j = 0; j < map.length; j++) {
					map.tiles[i,j].height += (a*i + b*j) * increase;
				}
			}
		}
			
		public static void AddCone(TiledMap map, float intensity, System.Random random) {
			
		}

		public static void AddNoise(TiledMap map, float intensity, float scale) {
			float[,] noise = Noise.Calc2D(map.width, map.length, scale);

			for(int i = 0; i < map.width; i++) {
				for(int j = 0; j < map.length; j++) {
					map.tiles[i,j].height += noise[i,j] * intensity;
				}
			}
		}

		public static void SimpleFlattenTop(TiledMap map) {
			ApplyPowerTop(map, 2f);
		}

		public static void SimpleFlattenBottom(TiledMap map) {
			ApplyPowerBottom(map, 2f);
		}

		public static void SimplePeakTop(TiledMap map) {
			ApplyPowerTop(map, 0.5f);
		}

		public static void SimplePeakBottom(TiledMap map) {
			ApplyPowerBottom(map, 0.5f);
		}

		public static void SimpleFlattenMiddle(TiledMap map) {
			ApplyPowerMedian(map, 2f);
		}

		public static void SimpleFlattenTopAndBottom(TiledMap map) {
			ApplyPowerTopAndBottom(map, 2f);
		}

		//FIXME Power methods do not work as planned with powers withing ]-1,1[ range

		public static void ApplyPowerBottom(TiledMap map, float intensity) {
			float factor = Mathf.Pow(map.height_limit, 1f - intensity);

			for(int i = 0; i < map.width; i++) {
				for(int j = 0; j < map.length; j++) {
					map[i,j].height = factor * Mathf.Pow(map[i,j].height, intensity);
				}
			}
		}

		public static void ApplyPowerTop(TiledMap map, float intensity) {
			float factor = Mathf.Pow(map.height_limit, 1f - intensity);

			for(int i = 0; i < map.width; i++) {
				for(int j = 0; j < map.length; j++) {
					map[i,j].height = map.height_limit - (factor * Mathf.Pow(map[i,j].height, intensity));
				}
			}
		}

		public static void ApplyPowerMedian(TiledMap map, float intensity) {
			float median = map.Median_Height;
			float top_factor = Mathf.Pow(map.height_limit - median, 1f - intensity), bottom_factor = Mathf.Pow(median, 1f - intensity);

			float temp;
			for(int i = 0; i < map.width; i++) {
				for(int j = 0; j < map.length; j++) {
					temp = map[i,j].height - median;
					if(temp >= 0) {
						map[i,j].height = (top_factor * Mathf.Pow(temp, intensity)) + median;
					} else {
						map[i,j].height = median - (bottom_factor * Mathf.Pow(temp, intensity));
					}
				}
			}
		}

		public static void ApplyPowerLevel(TiledMap map, float ratio, float intensity) {
			float median = ratio * map.height_limit;
			float top_factor = Mathf.Pow(map.height_limit - median, 1f - intensity), bottom_factor = Mathf.Pow(median, 1f - intensity);

			float temp;
			for(int i = 0; i < map.width; i++) {
				for(int j = 0; j < map.length; j++) {
					temp = map[i,j].height - median;
					if(temp >= 0) {
						map[i,j].height = (top_factor * Mathf.Pow(temp, intensity)) + median;
					} else {
						map[i,j].height = median - (bottom_factor * Mathf.Pow(temp, intensity));
					}
				}
			}
		}

		public static void ApplyPowerTopAndBottom(TiledMap map, float intensity) {
			float median = map.Median_Height;
			float top_factor = Mathf.Pow(map.height_limit - median, 1f - intensity), bottom_factor = Mathf.Pow(median, 1f - intensity);

			float temp;
			for(int i = 0; i < map.width; i++) {
				for(int j = 0; j < map.length; j++) {
					temp = map[i,j].height;
					if(temp >= median) {
						map[i,j].height = map.height_limit - (top_factor * Mathf.Pow(map.height_limit - temp, intensity));
					} else {
						map[i,j].height = bottom_factor * Mathf.Pow(temp, intensity);
					}
				}
			}
		}
	
		public static void Smooth(TiledMap map, int steps = 1, float max_height = 1f) {
			int count;
			float sum;
			float max = max_height * map.height_limit;
			for(int k = 0; k < steps; k++) {
				for(int i = 0; i < map.width; i++) {
					for(int j = 0; j < map.length; j++) {
						sum = map[i,j].height;
						if(sum <= max) {
							count = 1;
							if(i - 1 >= 0) {
								sum += map[i - 1, j].height;
								count++;
							}
							if(i + 1 < map.width) {
								sum += map[i +1, j].height;
								count++;
							}
							if(j - 1 >= 0) {
								sum += map[i,j - 1].height;
								count++;
							}
							if(j + 1 < map.length) {
								sum += map[i,j + 1].height;
								count++;
							}

							map[i,j].height = sum/count;
						}
					}
				}
			}
		}
	}
}
