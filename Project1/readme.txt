Utilizing the Diamond-Square algorithm, our terrain is generated via creating a number of child game object 
terrain segments to a parent in order to overcome the vertex limit of a single game object, and generates the
random height values.

We then colour the polygons based upon their height, in order to get a realistic looking landscape, this includes
limiting lower parts of the terrain to a certain height in order to get a flat ocean.