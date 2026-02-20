Pixel dispaly drawing application. Used to expedite the programming of a grid of individually programmable LEDs using the arduino fastLED library format. 
Draw desired image on the 41x24 pixel grid and hit save and it will output arduino code to set the individual pixels of LEDs arranged as I have for my current arduino project.
Next frame and last frame buttons used for the purpose of animation. Last frame sets the current frame and next frame will output any changes made from the last frame. This is used to reduce memory consumption in arduino code
and prevent every pixel from being set any time a new frame of animation is created, only the pixels that change between two frames of animation will be set in the resulting arduino code.

This project is currently extremely personalised for the needs of MY arduino project, but I have plans to generalize it for the purposes of anyone trying to quickly program LEDs using the fastLED library.

Planned features: setting and changing the grid size, setting the number of data wires used, complete arduino code generation(currently it only outputs the code for setting a frame, I would like it to do ALL of the work),
fill bucket tool, frame history, layers.
