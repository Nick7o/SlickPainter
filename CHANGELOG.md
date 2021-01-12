# Slick Painter - Changelog

### [Alpha 0.3.0]
 - added painting interpolation
   - SlickPainter's Paint() method overloads use normalized canvas position instead of BrushRects now
   - interpolation density can be controlled by the interpolation settings
 - moved project to the Unity 2020.1.16f1
 - moved input to a seperate class
 - moved initialization of SlickPainter services to the Awake() method
 - changed default canvas size to 512x512 pixels
 - fixed some minor bugs
 - code maintenance & clean up

### [Alpha 0.2.0]
**Note:** Main feature of this release is undo/redo support. It also includes some minor fixes and new utility functions.
 - added undo/redo support
 - fixed TextureUtilities.CompareSizeAndFormat() method which caused small memory leak
 - added new API functions
 - removed redundant code and comments

### [Alpha 0.1.0]
 - first version