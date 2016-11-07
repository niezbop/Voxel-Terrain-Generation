using UnityEngine;
using UnityEditor;
using System.Collections;
using VOXParsing;

namespace Generation {
	[ExecuteInEditMode]
	public class GeneratorScript : MonoBehaviour {

		public enum GeneratorType {
			COASTAL_MOUNTAIN,
			CANYON
		}

		public AbstractGenerator generator;
		private int width = 64, length = 64, height_limit = 32;
		public int Width {
			get { return width; }
			set { width = value; }
		}
		public int Length {
			get { return length; }
			set { length = value; }
		}
		public int Height_Limit {
			get { return height_limit; }
			set { height_limit = value; }
		}
		private GeneratorType type;
		public GeneratorType Type {
			get { return type; }
			set { type = value; }
		}

		private bool use_seed;
		public bool Use_seed {
			get { return use_seed; }
			set {
				use_seed = value;
				SetGenerator();
			}
		}
		private int seed;
		public int Seed {
			get { return seed; }
			set {
				seed = value;
				SetGenerator();
			}
		}
		private TiledMap map;

		[Header("Preview")]
		public MeshRenderer mesh_renderer;
		public Gradient ground;
		public int color_sampling = 32;

		public void OnEnable() {
			map = new TiledMap(width, length, height_limit);
			SetGenerator();
		}

		private void SetGenerator() {
			if(use_seed)
				switch(type) {
				case GeneratorType.CANYON:
					generator = new CanyonGenerator(seed);
					break;

				case GeneratorType.COASTAL_MOUNTAIN:
					generator = new CoastalMountainGenerator(seed);
					break;

				default:
					generator = new CoastalMountainGenerator(seed);
					break;
				}
			else
				switch(type) {
				case GeneratorType.CANYON:
					generator = new CanyonGenerator();
					break;

				case GeneratorType.COASTAL_MOUNTAIN:
					generator = new CoastalMountainGenerator();
					break;

				default:
					generator = new CoastalMountainGenerator();
					break;
				}

			generator.UseMap(map);
		}

		public void InitializeMap() {
			map = new TiledMap(width, length, height_limit);
			map.Floor();
			Preview();
		}

		public void Generate() {
			if(map == null)
				map = new TiledMap(width, length, height_limit);
			map.Floor();
			generator.Run();
			Debug.Log("Average height: "+map.Average_Height+", Median Height: "+map.Median_Height);
			Preview();
		}

		public void CreateVOXObject() {
			if(!(generator is IVOXGenerator))
				return;
			
			VOXObject obj = ((IVOXGenerator)generator).GenerateVOX(ground);
			Debug.Log(obj.Count);

			VOXParser vp = new VOXParser();
			vp.CreateFile(obj, path: "Assets/Procedural.vox");
		}

		private void Preview() {
			mesh_renderer.sharedMaterial.mainTexture = generator.GetPreview(ground);
		}
	}
}
