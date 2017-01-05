using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MapMagic
{
    // Refer to ScatterGenerator in ObjectGenerators.cs 
    [System.Serializable]
	[GeneratorMenu (menu="Custom", name ="1D Scatter", disengageable = true)]
	public class OneDScatterGenerator : Generator
	{
		public Input probability = new Input("Probability", InoutType.Map, write:false);
		public Output output = new Output("Output", InoutType.Objects);
		public override IEnumerable<Input> Inputs() { yield return probability; }
		public override IEnumerable<Output> Outputs() { yield return output; }

		public int seed = 12345;
        public float zValue = 0f;
		public float count = 10;
		public float uniformity = 0.1f; //aka candidatesNum/100

		public override void Generate (Chunk chunk, Biome biome = null)
		{
			Matrix probMatrix = (Matrix)probability.GetObject(chunk);
			SpatialHash spatialHash = chunk.defaultSpatialHash;
			if (!enabled) { output.SetObject(chunk, spatialHash); return; }
			if (chunk.stop) return; 
			
            // If the bounds of this chunk don't contain the specified zValue then return the
            // default spatialHash
            if(spatialHash.offset.y > zValue || spatialHash.offset.y + spatialHash.size <= zValue) {
                output.SetObject(chunk, spatialHash);
                return;
            }
            
			InstanceRandom rnd = new InstanceRandom(MapMagic.instance.seed + seed + chunk.coord.x*1000 + chunk.coord.z);

			int candidatesNum = (int)(uniformity*100);
			
			for (int i=0; i<count; i++)
			{
				Vector2 bestCandidate = Vector3.zero;
				float bestDist = 0;
				
				for (int c=0; c<candidatesNum; c++)
				{
					Vector2 candidate = new Vector2((spatialHash.offset.x+1) + (rnd.Random()*(spatialHash.size-2.01f)), zValue);
				
					//checking if candidate available here according to probability map
					if (probMatrix!=null && probMatrix[candidate] < rnd.Random()) continue;

					//checking if candidate is the furthest one
					float dist = spatialHash.MinDist(candidate);
					if (dist>bestDist) { bestDist=dist; bestCandidate = candidate; }
				}
              
				if (bestDist>0.001f) 
                    spatialHash.Add(bestCandidate, 0, 0, 1); //adding only if some suitable candidate found
			}

			if (chunk.stop) return;
			output.SetObject(chunk, spatialHash);
		}


		public override void OnGUI ()
		{
			//inouts
			layout.Par(20); probability.DrawIcon(layout); output.DrawIcon(layout);
			layout.Par(5);

			//params
			layout.Field(ref seed, "Seed");
			layout.Field(ref count, "Count");
            layout.Field(ref zValue, "Z Value");
			layout.Field(ref uniformity, "Uniformity", max:1);
		}
	}
}