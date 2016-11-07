using UnityEngine;
using System.Collections;

public class Tile {
	public int x, y;
	public float height;

	public Tile(int i, int j) {
		this.x = i;
		this.y = j;
		this.height = 0f;
	}
}
