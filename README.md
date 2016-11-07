# Voxel Terrain Generation

This repository shows the way I've setup Unity3D to be able to create terrain generators and export them to VOX files.

__Disclaimer :__ the code presented here is not optimized *at all*! This is just the first take on this subject I have been doing and requires several cycles of cleaning up.

![Rendered output](http://i.imgur.com/Or2ns8X.png)

# How does it work

Add the `GeneratorScript` behaviour on any game object in Unity, plug it in any kind of `mesh renderer` and it's good to go.

You can create your own generators by inheriting the `AbstractGenerator` class and modifying a bit the `GeneratorScript` to select and preview them.

# Credits

* The simplex noise I am using is the [C# implementation](https://github.com/WardBenjamin/SimplexNoise) of Benjamin Ward.
* VOX files are specified by [@ephtracy](https://twitter.com/ephtracy) for his program [MagicaVoxel](http://ephtracy.github.io/index.html?page=mv_main).

# I'm working on

- [ ] Fixing the various bugs/issues in my code
- [ ] More generators
- [ ] Improving VOX files reading in the VOX parser
