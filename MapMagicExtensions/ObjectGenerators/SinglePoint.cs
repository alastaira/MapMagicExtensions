using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MapMagic
{
    // Refer to ScatterGenerator in ObjectGenerators.cs 
    [System.Serializable]
    [GeneratorMenu(menu = "Custom", name = "Single Point", disengageable = true)]
    public class SinglePointGenerator : Generator {
        public Output output = new Output("Output", InoutType.Objects);
        public override IEnumerable<Output> Outputs() { yield return output; }

        public float xMin, xMax, zMin, zMax;
        public int seed = 12345;

        public enum CoordinateSpace { Terrain, World }
		public CoordinateSpace coordinateSpace = CoordinateSpace.World;
        
        public override void Generate(Chunk chunk, Biome biome = null) {
           SpatialHash spatialHash = chunk.defaultSpatialHash;
            if (!enabled) { output.SetObject(chunk, spatialHash); return; }
            if (chunk.stop) return;

            // Note - it's important that we *don't* change the seed based on the chunk coordinates, or else
            // we risk generating two different points.
            InstanceRandom rnd = new InstanceRandom(MapMagic.instance.seed + seed/* + chunk.coord.x*1000 + chunk.coord.z */);
            Vector2 candidate = new Vector2(xMin + rnd.Random() * (xMax - xMin), zMin + rnd.Random() * (zMax - zMin));
            
            // MM works with coordinates specified in "terrain space". If the user specifies coordinates in
            // absolute world position, need to scale these based on resolution and size of terrain before
            // adding to the spatialhash
            if(coordinateSpace == CoordinateSpace.World){
                float scaleFactor = (float)MapMagic.instance.resolution / (float)MapMagic.instance.terrainSize;
                candidate *= scaleFactor;
            }
            
            // If the spatial hash for this chunk does not contain the candidate point, simply return the default
            // spatial hash
            if (
                spatialHash.offset.x + spatialHash.size <= candidate.x /* candidate point lies too far to the right */
                || spatialHash.offset.x > candidate.x /* candidate point lies too far to the left */
                || spatialHash.offset.y + spatialHash.size <= candidate.y /* candidate point lies too far forward */
                || spatialHash.offset.y > candidate.y /* candidate point lies too far backward */
                ) {
                output.SetObject(chunk, spatialHash);
                return;
            }
            // If the candidate lies within this chunk's bounds, add it to the hash
            else {
                spatialHash.Add(candidate, 0, 0, 1); 
            }
            
            if (chunk.stop) return;
            output.SetObject(chunk, spatialHash);
        }

        public override void OnGUI() {
            //inouts
            layout.Par(20); 
            output.DrawIcon(layout);
            layout.Par(5);
            //params
            layout.fieldSize = 0.62f;
			layout.Field(ref coordinateSpace, "Space");
            layout.Field(ref seed, "Seed");
            layout.Field(ref xMin, "X Min");
            layout.Field(ref xMax, "X Max");
            layout.Field(ref zMin, "Z Min");
            layout.Field(ref zMax, "Z Max");
        }
    }
}