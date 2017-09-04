Utilizing the Diamond-Square algorithm, our terrain is generated via creating a number of child game object 
terrain segments to a parent in order to overcome the vertex limit of a single game object, and generates the
random height values.

We then colour the polygons based upon their height, in order to get a realistic looking landscape, this includes
limiting lower parts of the terrain to a certain height in order to get a flat looking ocean.

The Phong shader is applied to each game object and the sun will update its position in the shader each frame as to
correctly apply shading to each primitive in the landscape.

The camera controls work by moving the camera relative to the position of the mouse. When the user presses the movement
keys (w, a, s, d) the camera will move relative to the direction the camera is currently facing. The rotation keys (q, e)
will roll the camera based on the direction faced.

The PhongShader.shader and PointLight.cs have been adapted from the Graphics and Interactions Tutorial 4 solution.