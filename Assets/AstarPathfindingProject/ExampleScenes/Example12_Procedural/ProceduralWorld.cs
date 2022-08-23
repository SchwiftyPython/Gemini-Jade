using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pathfinding.Examples {
	/// <summary>Example script for generating an infinite procedural world</summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_examples_1_1_procedural_world.php")]
	public class ProceduralWorld : MonoBehaviour {
		/// <summary>
		/// The target
		/// </summary>
		public Transform target;

		/// <summary>
		/// The prefabs
		/// </summary>
		public ProceduralPrefab[] prefabs;

		/// <summary>How far away to generate tiles</summary>
		public int range = 1;

		/// <summary>
		/// The disable async load within range
		/// </summary>
		public int disableAsyncLoadWithinRange = 1;

		/// <summary>World size of tiles</summary>
		public float tileSize = 100;
		/// <summary>
		/// The sub tiles
		/// </summary>
		public int subTiles = 20;

		/// <summary>
		/// Enable static batching on generated tiles.
		/// Will improve overall FPS, but might cause FPS drops on
		/// some frames when static batching is done
		/// </summary>
		public bool staticBatching = false;

		/// <summary>
		/// The enumerator
		/// </summary>
		Queue<IEnumerator> tileGenerationQueue = new Queue<IEnumerator>();

		/// <summary>
		/// The rotation randomness enum
		/// </summary>
		public enum RotationRandomness {
			/// <summary>
			/// The all axes rotation randomness
			/// </summary>
			AllAxes,
			/// <summary>
			/// The  rotation randomness
			/// </summary>
			Y
		}

		/// <summary>
		/// The procedural prefab class
		/// </summary>
		[System.Serializable]
		public class ProceduralPrefab {
			/// <summary>Prefab to use</summary>
			public GameObject prefab;

			/// <summary>Number of objects per square world unit</summary>
			public float density = 0;

			/// <summary>
			/// Multiply by [perlin noise].
			/// Value from 0 to 1 indicating weight.
			/// </summary>
			public float perlin = 0;

			/// <summary>
			/// Perlin will be raised to this power.
			/// A higher value gives more distinct edges
			/// </summary>
			public float perlinPower = 1;

			/// <summary>Some offset to avoid identical density maps</summary>
			public Vector2 perlinOffset = Vector2.zero;

			/// <summary>
			/// Perlin noise scale.
			/// A higher value spreads out the maximums and minimums of the density.
			/// </summary>
			public float perlinScale = 1;

			/// <summary>
			/// Multiply by [random].
			/// Value from 0 to 1 indicating weight.
			/// </summary>
			public float random = 1;

			/// <summary>
			/// The all axes
			/// </summary>
			public RotationRandomness randomRotation = RotationRandomness.AllAxes;

			/// <summary>If checked, a single object will be created in the center of each tile</summary>
			public bool singleFixed = false;
		}

		/// <summary>All tiles</summary>
		Dictionary<Int2, ProceduralTile> tiles = new Dictionary<Int2, ProceduralTile>();

		// Use this for initialization
		/// <summary>
		/// Starts this instance
		/// </summary>
		void Start () {
			// Calculate the closest tiles
			// and then recalculate the graph
			Update();
			AstarPath.active.Scan();

			StartCoroutine(GenerateTiles());
		}

		// Update is called once per frame
		/// <summary>
		/// Updates this instance
		/// </summary>
		void Update () {
			// Calculate the tile the target is standing on
			Int2 p = new Int2(Mathf.RoundToInt((target.position.x - tileSize*0.5f) / tileSize), Mathf.RoundToInt((target.position.z - tileSize*0.5f) / tileSize));

			// Clamp range
			range = range < 1 ? 1 : range;

			// Remove tiles which are out of range
			bool changed = true;
			while (changed) {
				changed = false;
				foreach (KeyValuePair<Int2, ProceduralTile> pair in tiles) {
					if (Mathf.Abs(pair.Key.x-p.x) > range || Mathf.Abs(pair.Key.y-p.y) > range) {
						pair.Value.Destroy();
						tiles.Remove(pair.Key);
						changed = true;
						break;
					}
				}
			}

			// Add tiles which have come in range
			// and start calculating them
			for (int x = p.x-range; x <= p.x+range; x++) {
				for (int z = p.y-range; z <= p.y+range; z++) {
					if (!tiles.ContainsKey(new Int2(x, z))) {
						ProceduralTile tile = new ProceduralTile(this, x, z);
						var generator = tile.Generate();
						// Tick it one step forward
						generator.MoveNext();
						// Calculate the rest later
						tileGenerationQueue.Enqueue(generator);
						tiles.Add(new Int2(x, z), tile);
					}
				}
			}

			// The ones directly adjacent to the current one
			// should always be completely calculated
			// make sure they are
			for (int x = p.x-disableAsyncLoadWithinRange; x <= p.x+disableAsyncLoadWithinRange; x++) {
				for (int z = p.y-disableAsyncLoadWithinRange; z <= p.y+disableAsyncLoadWithinRange; z++) {
					tiles[new Int2(x, z)].ForceFinish();
				}
			}
		}

		/// <summary>
		/// Generates the tiles
		/// </summary>
		/// <returns>The enumerator</returns>
		IEnumerator GenerateTiles () {
			while (true) {
				if (tileGenerationQueue.Count > 0) {
					var generator = tileGenerationQueue.Dequeue();
					yield return StartCoroutine(generator);
				}
				yield return null;
			}
		}

		/// <summary>
		/// The procedural tile class
		/// </summary>
		class ProceduralTile {
			/// <summary>
			/// The 
			/// </summary>
			int x, z;
			/// <summary>
			/// The rnd
			/// </summary>
			System.Random rnd;

			/// <summary>
			/// The world
			/// </summary>
			ProceduralWorld world;

			/// <summary>
			/// Gets or sets the value of the destroyed
			/// </summary>
			public bool destroyed { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="ProceduralTile"/> class
			/// </summary>
			/// <param name="world">The world</param>
			/// <param name="x">The </param>
			/// <param name="z">The </param>
			public ProceduralTile (ProceduralWorld world, int x, int z) {
				this.x = x;
				this.z = z;
				this.world = world;
				rnd = new System.Random((x * 10007) ^ (z*36007));
			}

			/// <summary>
			/// The root
			/// </summary>
			Transform root;
			/// <summary>
			/// The ie
			/// </summary>
			IEnumerator ie;

			/// <summary>
			/// Generates this instance
			/// </summary>
			/// <returns>The enumerator</returns>
			public IEnumerator Generate () {
				ie = InternalGenerate();
				GameObject rt = new GameObject("Tile " + x + " " + z);
				root = rt.transform;
				while (ie != null && root != null && ie.MoveNext()) yield return ie.Current;
				ie = null;
			}

			/// <summary>
			/// Forces the finish
			/// </summary>
			public void ForceFinish () {
				while (ie != null && root != null && ie.MoveNext()) {}
				ie = null;
			}

			/// <summary>
			/// Randoms the inside
			/// </summary>
			/// <returns>The </returns>
			Vector3 RandomInside () {
				Vector3 v = new Vector3();

				v.x = (x + (float)rnd.NextDouble())*world.tileSize;
				v.z = (z + (float)rnd.NextDouble())*world.tileSize;
				return v;
			}

			/// <summary>
			/// Randoms the inside using the specified px
			/// </summary>
			/// <param name="px">The px</param>
			/// <param name="pz">The pz</param>
			/// <returns>The </returns>
			Vector3 RandomInside (float px, float pz) {
				Vector3 v = new Vector3();

				v.x = (px + (float)rnd.NextDouble()/world.subTiles)*world.tileSize;
				v.z = (pz + (float)rnd.NextDouble()/world.subTiles)*world.tileSize;
				return v;
			}

			/// <summary>
			/// Randoms the y rot using the specified prefab
			/// </summary>
			/// <param name="prefab">The prefab</param>
			/// <returns>The quaternion</returns>
			Quaternion RandomYRot (ProceduralPrefab prefab) {
				return prefab.randomRotation == RotationRandomness.AllAxes ? Quaternion.Euler(360*(float)rnd.NextDouble(), 360*(float)rnd.NextDouble(), 360*(float)rnd.NextDouble()) : Quaternion.Euler(0, 360 * (float)rnd.NextDouble(), 0);
			}

			/// <summary>
			/// Internals the generate
			/// </summary>
			/// <returns>The enumerator</returns>
			IEnumerator InternalGenerate () {
				Debug.Log("Generating tile " + x + ", " + z);
				int counter = 0;

				float[, ] ditherMap = new float[world.subTiles+2, world.subTiles+2];

				//List<GameObject> objs = new List<GameObject>();

				for (int i = 0; i < world.prefabs.Length; i++) {
					ProceduralPrefab pref = world.prefabs[i];

					if (pref.singleFixed) {
						Vector3 p = new Vector3((x+0.5f) * world.tileSize, 0, (z+0.5f) * world.tileSize);
						GameObject ob = GameObject.Instantiate(pref.prefab, p, Quaternion.identity) as GameObject;
						ob.transform.parent = root;
					} else {
						float subSize = world.tileSize/world.subTiles;

						for (int sx = 0; sx < world.subTiles; sx++) {
							for (int sz = 0; sz < world.subTiles; sz++) {
								ditherMap[sx+1, sz+1] = 0;
							}
						}

						for (int sx = 0; sx < world.subTiles; sx++) {
							for (int sz = 0; sz < world.subTiles; sz++) {
								float px = x + sx/(float)world.subTiles;//sx / world.tileSize;
								float pz = z + sz/(float)world.subTiles;//sz / world.tileSize;

								float perl = Mathf.Pow(Mathf.PerlinNoise((px + pref.perlinOffset.x)*pref.perlinScale, (pz + pref.perlinOffset.y)*pref.perlinScale), pref.perlinPower);

								float density = pref.density * Mathf.Lerp(1, perl, pref.perlin) * Mathf.Lerp(1, (float)rnd.NextDouble(), pref.random);
								float fcount = subSize*subSize*density + ditherMap[sx+1, sz+1];
								int count = Mathf.RoundToInt(fcount);

								// Apply dithering
								// See http://en.wikipedia.org/wiki/Floyd%E2%80%93Steinberg_dithering
								ditherMap[sx+1+1, sz+1+0] += (7f/16f) * (fcount - count);
								ditherMap[sx+1-1, sz+1+1] += (3f/16f) * (fcount - count);
								ditherMap[sx+1+0, sz+1+1] += (5f/16f) * (fcount - count);
								ditherMap[sx+1+1, sz+1+1] += (1f/16f) * (fcount - count);

								// Create a number of objects
								for (int j = 0; j < count; j++) {
									// Find a random position inside the current sub-tile
									Vector3 p = RandomInside(px, pz);
									GameObject ob = GameObject.Instantiate(pref.prefab, p, RandomYRot(pref)) as GameObject;
									ob.transform.parent = root;
									//ob.SetActive ( false );
									//objs.Add ( ob );
									counter++;
									if (counter % 2 == 0)
										yield return null;
								}
							}
						}
					}
				}

				ditherMap = null;

				yield return null;
				yield return null;

				//Batch everything for improved performance
				if (Application.HasProLicense() && world.staticBatching) {
					StaticBatchingUtility.Combine(root.gameObject);
				}
			}

			/// <summary>
			/// Destroys this instance
			/// </summary>
			public void Destroy () {
				if (root != null) {
					Debug.Log("Destroying tile " + x + ", " + z);
					GameObject.Destroy(root.gameObject);
					root = null;
				}

				// Make sure the tile generator coroutine is destroyed
				ie = null;
			}
		}
	}
}
