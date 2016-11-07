using UnityEngine;
using System.Collections;

public struct Voxel {
	public bool empty;
	public byte pos_x, pos_y, pos_z, color;

	public Voxel(byte x, byte y, byte z, byte col = 0, bool b = true) {
		this.pos_x = x;
		this.pos_y = y;
		this.pos_z = z;
		this.color = col;
		this.empty = b;
	}

	public override string ToString () {
		return empty? "Empty voxel" : "Voxel at "+pos_x+", "+pos_y+", "+pos_z+" with color index "+color;
	}
}