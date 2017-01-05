using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MapMagic
{
    // Refer to ScatterGenerator in ObjectGenerators.cs 
    [System.Serializable]
    [GeneratorMenu(menu = "Custom", name = "Regular Spaced", disengageable = true)]
    public class RegularSpacedGenerator : Generator {
        public Output output = new Output("Output", InoutType.Objects);
        public override IEnumerable<Output> Outputs() { yield return output; }

        public float xOffset = 0f;
        public float zOffset = 0f;
        public float xSpacing = 0f;
        public float zSpacing = 0f;

        public override void Generate(Chunk chunk, Biome biome = null) {
            SpatialHash spatialHash = chunk.defaultSpatialHash;
            if (!enabled) { output.SetObject(chunk, spatialHash); return; }
            if (chunk.stop) return;

            // Note that spacing is specified in terms of the resolution of the terrain.
            // The *world spacing* will be equal to the spacing specified here / the terrain size * terrain resolution
            for (float z = zOffset; z < spatialHash.size; z += zSpacing) {
                for (float x = xOffset; x < spatialHash.size; x += xSpacing) {
                    Vector2 candidate = new Vector2((spatialHash.offset.x + x), (spatialHash.offset.y + z));
                    spatialHash.Add(candidate, 0, 0, 1); //adding only if some suitable candidate found
                    if (xSpacing == 0) break;
                }
                if (zSpacing == 0) break;
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
            layout.Field(ref xOffset, "X offset");
            layout.Field(ref zOffset, "Z offset");
            layout.Field(ref xSpacing, "X spacing");
            layout.Field(ref zSpacing, "Z spacing");
        }
    }
}