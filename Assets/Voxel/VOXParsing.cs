using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;

namespace VOXParsing {
	public class VOXParser {
		private const string HEADER_ID = "VOX ";
		private const uint VERSION = 150;
		private const string MAIN_ID = "MAIN";
		private const string SIZE_ID = "SIZE";
		private const string XYZI_ID = "XYZI";
		private const string PALETTE_ID = "RGBA";

		private readonly byte[] empty4 = new byte[4] { 0, 0, 0, 0 };

		public VOXParser() {

		}

		public VOXObject FromFile(string path) {
			BinaryReader br;

			uint r_version, r_main_size, r_size_x, r_size_y, r_size_z, r_xyzi_size, r_voxel_count;
			VOXObject result = null;

			try {
				br = new BinaryReader(System.IO.File.Open(path,FileMode.Open));
			} catch(IOException e) {
				Debug.Log(e.Message + "\nCannot read file");
				return result;
			}
			byte[] temp;
			try {
				temp = br.ReadBytes(4);
				if(temp != System.Text.Encoding.ASCII.GetBytes(HEADER_ID)) {
					throw new FormatException("File is not .vox");
				}
				temp = br.ReadBytes(4);
//				Debug.Log("Version is " + BitConverter.ToUInt32(temp,0));
				r_version = BitConverter.ToUInt32(temp,0);
				temp = br.ReadBytes(4);		//"MAIN"
				temp = br.ReadBytes(4);
				temp = br.ReadBytes(4);
				r_main_size = BitConverter.ToUInt32(temp,0);
				temp = br.ReadBytes(4);		//"SIZE"
				temp = br.ReadBytes(4);
				temp = br.ReadBytes(4);

				temp = br.ReadBytes(4);
				r_size_x = BitConverter.ToUInt32(temp,0);
				temp = br.ReadBytes(4);
				r_size_y = BitConverter.ToUInt32(temp,0);
				temp = br.ReadBytes(4);
				r_size_z = BitConverter.ToUInt32(temp,0);
				result = new VOXObject(r_size_x,r_size_y,r_size_z);
//				Debug.Log("Sizes: "+ r_size_x + ", " + r_size_y + ", " + r_size_z);

				temp = br.ReadBytes(4);		//"XYZI"

				temp = br.ReadBytes(4);
				r_xyzi_size = BitConverter.ToUInt32(temp,0);
				temp = br.ReadBytes(4);
				temp = br.ReadBytes(4);
				r_voxel_count = BitConverter.ToUInt32(temp,0);

				Voxel parsed;
				for(uint i = 0; i < r_voxel_count; i++) {
					byte pos_x, pos_y, pos_z, color;
					pos_x = br.ReadByte();
					pos_y = br.ReadByte();
					pos_z = br.ReadByte();
					color = br.ReadByte();
					parsed = new Voxel(pos_x, pos_y, pos_z, color, false);
//					Debug.Log(parsed);
					result.voxels[pos_x][pos_y][pos_z].empty = false;
					result.voxels[pos_x][pos_y][pos_z].color = color;
				}

				//TODO read palette/materials
			} catch(IOException e) {
				Debug.Log(e.Message + "\nError reading file");
			}

			br.Close();
			
			return result;
		}

		public VOXObject FromFile(DefaultAsset asset) {
			return FromFile(AssetDatabase.GetAssetPath(asset));
		}

		public void CreateFile(VOXObject obj, string path = "Assets/NewModel.vox") {
			BinaryWriter bin_writer;

			try {
				bin_writer = new BinaryWriter(System.IO.File.Create(path));
			} catch(IOException e) {
				Debug.Log(e.Message + "\nCannot create file");
				return;
			}
			//MAIN
			VOXChunk main_chunk = new VOXChunk(MAIN_ID);
			//SIZE
			VOXChunk size_chunk = new VOXChunk(SIZE_ID);
			byte[] size_x_byte = BitConverter.GetBytes(obj.size_x);
			byte[] size_y_byte = BitConverter.GetBytes(obj.size_y);
			byte[] size_z_byte = BitConverter.GetBytes(obj.size_z);
			byte[] sizes = new byte[12];

			for(int i = 0; i < 4; i++) {
				sizes[i + 0] = size_x_byte[i];
				sizes[i + 4] = size_y_byte[i];
				sizes[i + 8] = size_z_byte[i];
			}
			size_chunk.content = sizes;
			main_chunk.children.Add(size_chunk);

			//XYZI
			VOXChunk xyzi_chunk = new VOXChunk(XYZI_ID);
			List<Voxel> voxels = obj.NonEmptyVoxels;
			uint nb_voxels = (uint)voxels.Count;
			byte[] voxels_byte = new byte[4 * (nb_voxels + 1)];
			BitConverter.GetBytes(nb_voxels).CopyTo(voxels_byte, 0);
			for(int i = 0; i < nb_voxels; i++) {
				voxels[i].ToBytes().CopyTo(voxels_byte, 4 * (i+1));
			}
			xyzi_chunk.content = voxels_byte;
			main_chunk.children.Add(xyzi_chunk);

			Color[] palette = obj.Palette;
			if(palette != null) {
				VOXChunk palette_chunk = new VOXChunk(PALETTE_ID);
				byte[] colors = new byte[VOXObject.PALETTE_LENGTH * 4];
				byte[] byte_color;
				for(int i = 0; i < VOXObject.PALETTE_LENGTH; i++) {
					byte_color = palette[i].ToBytes();
					colors[4 * i + 0] = byte_color[0];
					colors[4 * i + 1] = byte_color[1];
					colors[4 * i + 2] = byte_color[2];
					colors[4 * i + 3] = byte_color[3];
				}
				palette_chunk.content = colors;
				main_chunk.children.Add(palette_chunk);
			}

			try {
				bin_writer.Write(System.Text.Encoding.ASCII.GetBytes(HEADER_ID));
				bin_writer.Write(VERSION);
				bin_writer.WriteVOXChunk(main_chunk);
			} catch(IOException e) {
				Debug.Log(e.Message + "\nError writing to file");
			}

			/* DEPLETED
			 * try {
				bin_writer.Write(System.Text.Encoding.ASCII.GetBytes(header_id));		//"VOX "
				bin_writer.Write(version);			//version

				bin_writer.Write(System.Text.Encoding.ASCII.GetBytes(main_id));			//"MAIN"
				bin_writer.Write((uint)0);			//Content size of main
				bin_writer.Write((uint)
					4 +			//SIZE header
					4 +			//SIZE content size
					4 +			//SIZE children size
					12 + 		//SIZE content
					4 + 		//XYZI header
					4 + 		//XYZI content size
					4 + 		//XYZI children size
					4*(nb_voxels+1)
				);

				bin_writer.Write(System.Text.Encoding.ASCII.GetBytes(size_id));			//"SIZE"
				bin_writer.Write((uint)12);
				bin_writer.Write(empty4); 			//Children size of SIZE
				//SIZE content: 3 x uint representing dimension length of the model
				bin_writer.Write((uint)obj.size_x);
				bin_writer.Write((uint)obj.size_y);
				bin_writer.Write((uint)obj.size_z);

				bin_writer.Write(System.Text.Encoding.ASCII.GetBytes(xyzi_id));			//"XYZI"
				bin_writer.Write(4*(nb_voxels+1));	//Content size of XYZI : (Nb de voxel*4) + 4
				bin_writer.Write((uint)0);			//Children size of XYZI
				bin_writer.Write(nb_voxels);

				for(int i = 0; i < nb_voxels; i++) {
					bin_writer.Write(voxels[i].pos_x);
					bin_writer.Write(voxels[i].pos_y);
					bin_writer.Write(voxels[i].pos_z);
					bin_writer.Write(voxels[i].color);
				}

			} catch(IOException e) {
				Debug.Log(e.Message + "\nError writing to file");
			}*/

			bin_writer.Close();
		}
	}

	/// <summary>
	/// VOX chunk class, used to parse VOXObject in VOXParsing.
	/// </summary>
	public class VOXChunk {
		private byte[] id;
		public byte[] ID {
			get { return id; }
			set { 
				if(value.Length != 4)
					throw new FormatException("Unexpected id length");
				id = value;
			}
		}
		public void SetID(string _id) {
			if(_id.Length > 4)
				throw new FormatException("ID cannot be longer than 4 characters");
			string final_id = _id;
			while(final_id.Length < 4)
				final_id += " ";

			id = System.Text.Encoding.ASCII.GetBytes(final_id.Substring(0,4));
		}
		public string GetID() {
			return System.Text.Encoding.ASCII.GetString(id);
		}
		public byte[] content;
		public List<VOXChunk> children = new List<VOXChunk>();
		public IEnumerable Childrens {
			get {
				for(int i = 0; i < children.Count; i++)
					yield return children[i];
			}
		}

		public uint Content_size {
			get {
				return (uint)content.Length;
			}
		}

		public uint Children_size {
			get {
				uint count = 0;

				for(int i = 0; i < children.Count; i++) {
					count += children[i].Total_size;
				}

				return count;
			}
		}

		public uint Total_size {
			get {
				return Content_size + Children_size + 12;		// 4 bytes for the header, 4 bytes for the content size field, 4 bytes for the children size field
			}
		}
		public VOXChunk(byte[] _id) {
			if(_id.Length != 4)
				throw new FormatException("Unexpected id length");
			this.id = _id;
			this.content = new byte[0];
		}
		public VOXChunk(string _id) {
			this.id = new byte[4];
			this.SetID(_id);
			this.content = new byte[0];
		}
		public VOXChunk(byte[] _id, byte[] _content) {
			if(_id.Length != 4)
				throw new FormatException("Unexpected id length");
			this.id = _id;
			this.content = _content;
		}
		public VOXChunk(string _id, byte[] _content) {
			this.id = new byte[4];
			this.SetID(_id);
			this.content = _content;
		}
	}

	/// <summary>
	/// VOX format utilities.
	/// </summary>
	public static class VOXFormatUtilities {
		public static byte[] ToBytes(this Voxel voxel) {
			if(!voxel.empty)
				return new byte[4] { 
					voxel.pos_x, 
					voxel.pos_y, 
					voxel.pos_z, 
					voxel.color 
				};
			else
				return null;
		}

		public static byte[] ToBytes(this Color color) {
			return new byte[4] {
				Convert.ToByte(color.r),
				Convert.ToByte(color.g),
				Convert.ToByte(color.b),
				Convert.ToByte(color.a)
			};
		}

		public static void WriteVOXChunk(this BinaryWriter bin_writer, VOXChunk chunk) {
			bin_writer.Write(chunk.ID);
			bin_writer.Write(chunk.Content_size);
			bin_writer.Write(chunk.Children_size);
			bin_writer.Write(chunk.content);
			foreach(VOXChunk child in chunk.Childrens)
				bin_writer.WriteVOXChunk(child);
		}
	}
}
