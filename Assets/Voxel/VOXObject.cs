using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VOXObject {
	public uint size_x, size_y, size_z;
	public Voxel[][][] voxels;
	private Color[] palette;
	public static int PALETTE_LENGTH = 255;
	public Color[] Palette {
		get {
			if(palette == null || palette.Length != PALETTE_LENGTH)
				return null;
			return palette;
		}
		set {
			if(value.Length != PALETTE_LENGTH)
				throw new FormatException("Palette length must be " + PALETTE_LENGTH);
			palette = value;
		}
	}

	public VOXObject(uint _size_x, uint _size_y, uint _size_z) {		
		this.size_x = _size_x;
		this.size_y = _size_y;
		this.size_z = _size_z;

		this.voxels = new Voxel[_size_x][][];
		for(uint i = 0; i < _size_x; i++) {
			voxels[i] = new Voxel[_size_y][];
			for(uint j = 0; j < _size_y; j++) {
				voxels[i][j] = new Voxel[_size_z];
				for(uint k = 0; k < _size_z; k++) {
					voxels[i][j][k] = new Voxel((byte)i,(byte)j,(byte)k,(byte)79,true);
				}
			}
		}

		palette = null;
	}

	public void Fill() {
		for(uint i = 0; i < this.size_x; i++) {
			for(uint j = 0; j < this.size_y; j++) {
				for(uint k = 0; k < this.size_z; k++) {
					voxels[i][j][k].empty = false;
				}
			}
		}
	}

	public int Count {
		get { return CountVoxels(); }
	}

	public List<Voxel> NonEmptyVoxels {
		get { return GetNonEmptyVoxels(); }
	}

	private int CountVoxels() {

		if(voxels == null)
			return 0;
		
		int count = 0;
		for(int i = 0; i < size_x; i++) {
			for(int j = 0; j < size_y; j++) {
				for(int k = 0; k < size_z; k++) {
					if(!voxels[i][j][k].empty)
						count++;
				}
			}
		}

		return count;
	}

	private List<Voxel> GetNonEmptyVoxels() {

		if(voxels == null) {
			Debug.Log("Voxels is null");
			return null;
		}
		List<Voxel> result = new List<Voxel>();
		for(int i = 0; i < size_x; i++) {
			for(int j = 0; j < size_y; j++) {
				for(int k = 0; k < size_z; k++) {
					if(!(voxels[i][j][k].empty)) {
						result.Add(voxels[i][j][k]);
					}
				}
			}
		}

		return result;
	}
}
