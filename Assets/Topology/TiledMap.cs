using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TiledMap {
	public Tile[,] tiles;
	public int width, length;
	public float height_limit;

	/// <summary>
	/// Iterates over the tiles.
	/// </summary>
	public IEnumerable TileLoop {
		get {
			for(int i = 0; i < width; i++) {
				for(int j = 0; j < length; j++) {
					yield return tiles[i,j];
				}
			}
		}
	}
	/// <summary>
	/// Gets the <see cref="TiledMap"/> with the specified indexes i and j.
	/// </summary>
	public Tile this[int i, int j] {
		get { return tiles[i,j]; }
	}
	/// <summary>
	/// Gets the height ratio map. 0f for minimal height and 1f for the maximal.
	/// </summary>
	/// <value>The height ratio map.</value>
	public float[,] Height_Ratio_Map {
		get {
			float[,] tau = new float[width,length];
			float max = Max_Height, min = Min_Height;
			float delta = max - min;

			for(int i = 0; i < width; i++) {
				for(int j = 0; j < length; j++) {
					tau[i,j] = (tiles[i,j].height - min)/delta;
				}
			}

			return tau;
		}
	}
	/// <summary>
	/// Gets the median height of the map, which is the height where as many tiles are higher and lower.
	/// </summary>
	/// <value>The median height of the map.</value>
	public float Median_Height {
		get {
			List<float> temp = new List<float>();
			foreach(Tile t in TileLoop)
				temp.Add(t.height);

			float[] arr = temp.ToArray();
			Array.Sort(arr);

			return arr[arr.Length/2];
		}
	}
	/// <summary>
	/// Gets the median height of the map scaled down to a [0-1] height map.
	/// </summary>
	/// <value>The median height of the map.</value>
	public float Unit_Median_Height {
		get {
			return (Median_Height - Min_Height)/(Max_Height - Min_Height);
		}
	}
	/// <summary>
	/// Gets the average height of the map.
	/// </summary>
	/// <value>The average height of the map.</value>
	public float Average_Height {
		get {
			float average = 0f;
			foreach(Tile t in TileLoop) {
				average += t.height;
			}

			return average/(width * length);
		}
	}
	/// <summary>
	/// Gets the maximal height of the map.
	/// </summary>
	/// <value>The maximal height.</value>
	public float Max_Height {
		get { 
			if(tiles == null)
				return height_limit;
			
			float max = tiles[0,0].height;
			foreach(Tile t in TileLoop) {
				max = Mathf.Max(max, t.height);
			}
			return max;
		}
	}
	/// <summary>
	/// Gets the minimal height of the map.
	/// </summary>
	/// <value>The minimal height.</value>
	public float Min_Height {
		get { 
			if(tiles == null)
				return 0f;

			float min = tiles[0,0].height;
			foreach(Tile t in TileLoop) {
				min = Mathf.Min(min, t.height);
			}
			return min;
		}
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="TiledMap"/> class.
	/// </summary>
	/// <param name="_width">Width of the map.</param>
	/// <param name="_length">Length of the map.</param>
	/// <param name="_height_limit">Height limit of the map.</param>
	public TiledMap(int _width, int _length, float _height_limit) {
		this.width = _width;
		this.length = _length;
		this.height_limit = _height_limit;

		tiles = new Tile[_width,_length];
		for(int i = 0; i < _width; i++) {
			for(int j = 0; j < _length; j++) {
				tiles[i,j] = new Tile(i,j);
				tiles[i,j].height = height_limit / 2f;
			}
		}
	}
	/// <summary>
	/// Floor the map.
	/// </summary>
	public void Floor() {
		foreach(Tile t in TileLoop)
			t.height = 0f;
	}
	/// <summary>
	/// Clamp the height of each tile to the [0 - height limit] interval.
	/// </summary>
	public void Clamp() {
		foreach(Tile t in TileLoop) {
			t.height = Mathf.Max(t.height, height_limit);
			t.height = Mathf.Min(t.height, 0f);
		}
	}
	/// <summary>
	/// Normalize the map so it fits the [0 - height limit] interval.
	/// </summary>
	public void Normalize() {
		float max = Max_Height, min = Min_Height;
		float multiplier = height_limit/(max - min);

		foreach(Tile t in TileLoop) {
			t.height -= min;
			t.height *= multiplier;
		}
	}
}
