# Map Magic Extensions
Custom extensions for the <a href="http://www.denispahunov.ru/MapMagic/">Map Magic</a> procedural terrain generator for the Unity 3D game engine.

## Map Generators
- **Chance** - takes a single matrix input and routes it to a randomly selected output, based on a weighted probability. (Similar to the "Split" generator for objects)

- **Pick** - randomly chooses one of the attached inputs based on weighted probability and routes it to the single output. (The complement of "Chance")
  
## Object Generators
- **Single Point** - outputs a single object location either based on an exact specified position (specified in worldspace coordinates or relative to terrain), or at a single random position chosen within a specified boundary. 

- **Regular Spaced** - outputs a series of object locations in a regular grid, based on a specified X and Z spacing. 

- **1D Scatter** - a modification of the built-in scatter generator that only places object locations along a line of fixed Z axis.

# Usage
Simply download and copy to anywhere in your project's Assets folder. The new generator types will appear under the "Custom" menu option of the MapMagic editor window.

# Examples
The following graph has a 2/3 chance of creating desert, and a 1/3 chance of creating swamp biome.
![Chance](https://raw.githubusercontent.com/alastaira/MapMagicExtensions/master/Docs/MMX_chance.png)

The following graph picks either the Constant, Noise, or Voronoi generator to create the heightmap, with equal probability.
![Pick](https://raw.githubusercontent.com/alastaira/MapMagicExtensions/master/Docs/MMX_pick.png)

The following graph places a single cube on the terrain at coordinates somewhere between (300,120) and (400,150).
![Pick](https://raw.githubusercontent.com/alastaira/MapMagicExtensions/master/Docs/MMX_singlepoint.png)
