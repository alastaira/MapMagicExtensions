using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MapMagic
{

    // Refer to CombineGenerator in ObjectGenerators.cs
	[System.Serializable]
	[GeneratorMenu (menu="Custom", name ="Pick", disengageable = true)]
	public class PickMapGenerator : Generator
	{
        // Each output needs to have weight property associated with it, so we'll use
        // an array of Layers rather than a simple array of Outputs.
        public class Layer
		{
			public Input input = new Input(InoutType.Map);
			public float weight = 1f;
		}
		public Layer[] layers = new Layer[] { new Layer(), new Layer() };
    
        // Only a single Input
        public Output output = new Output("Output", InoutType.Map);
        
		public override IEnumerable<Input> Inputs() { for (int i=0; i<layers.Length; i++) yield return layers[i].input; }
		public override IEnumerable<Output> Outputs() { yield return output; }
        
		public int inputsNum = 2;
        public int seed = 12345;
        public int guiSelected;
        
		public override void Generate (Chunk chunk, Biome biome = null)
		{	
			//return on stop/disable
			if (chunk.stop || !enabled) return;

            InstanceRandom rnd = new InstanceRandom(MapMagic.instance.seed + seed/* + chunk.coord.x*1000 + chunk.coord.z*/);

            // Calculate the total weight of all layers
            float sumOfWeights = 0f;
            for (int i=0; i<layers.Length; i++){
                sumOfWeights += layers[i].weight;
            }
            
            // Pick a random value less than the total weight
            float rouletteSelection = rnd.Random(0, sumOfWeights);

            // Loop through the layers, keeping a running sum of weights
            // The layer that contains the chosen rouletteWeight gets sent the input
            // All other layers get sent the default matrix
            float prevRunningWeight = 0f;
            float nextRunningWeight = 0f;               
            for (int i=0; i<layers.Length; i++){
                nextRunningWeight = prevRunningWeight + layers[i].weight;
                if(prevRunningWeight < rouletteSelection && nextRunningWeight > rouletteSelection) {
                    output.SetObject(chunk, (Matrix)layers[i].input.GetObject(chunk));
                }
                prevRunningWeight = nextRunningWeight;
            }
		}

        // Defines how the row element for each individual output layer is drawn
        public void OnLayerGUI (Layer layer, Layout layout, int num, bool selected) 
		{
			layout.margin += 10; layout.rightMargin +=5;
			layout.Par(20);
            layer.input.DrawIcon(layout);
            layout.Label("Weight:", layout.Inset(0.4f));
            layout.Field(ref layer.weight, rect:layout.Inset(0.5f));
			layout.margin -= 10; layout.rightMargin -=5;
		}
        
        
		public override void OnGUI ()
		{
			//input
            output.DrawIcon(layout, "Output");
			layout.Par(16);
			//params
            layout.Field(ref seed, "Seed");
            layout.Par(16);
			//output layers
			layout.Label("Layers:", layout.Inset(0.4f));
			layout.DrawArrayAdd(ref layers, ref guiSelected, layout.Inset(0.15f), def:new Layer());
			layout.DrawArrayRemove(ref layers, ref guiSelected, layout.Inset(0.15f));
			layout.DrawArrayUp(ref layers, ref guiSelected, layout.Inset(0.15f));
			layout.DrawArrayDown(ref layers, ref guiSelected, layout.Inset(0.15f));
			layout.margin = 10;
			layout.Par(5);
			layout.DrawLayered(layers, ref guiSelected, min:0, max:layers.Length, reverseOrder:true, onLayerGUI:OnLayerGUI);
		}
	}
}