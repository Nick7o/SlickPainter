# Slick Painter
**Version: 0.1.0**

Slick Painter is a tool for runtime drawing in Unity, written in C#.

#### Main features:
 - drawing and erasing using brushes
   - circular brush with hardness property
   - stamp brush
 - support for alpha and non-alpha backgrounds
 - bilinear image scaling
 - easily extendable brushes, blend modes and image scaling implementations

### Roadmap:
 - undo/redo
 - support for layers (mainly for different blend modes, moving, scaling etc.)
 - mouse path drawing interpolation (drawing brushes in places where mouse wasn't in provided frame, especially useful for low framerates)
 - text support

# How to install
Just put **SlickPainter** folder in your assets and you should be good to go.
Demo scene shows main features of the system and how you can use the API.

# Examples
Demo Scene
![Demo Scene](https://github.com/Naspey/LogoMaker/blob/main/img/example-3.png "Demo")

Made only using circular brush:
![Example 2](https://github.com/Naspey/LogoMaker/blob/main/img/example-2.png "Example 2")
![Example 1](https://github.com/Naspey/LogoMaker/blob/main/img/example-1.png "Example 1")
