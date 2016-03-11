# Level Editor Unity Sample

This is a Unity project that serves as a sample to show how to import level editor files within Unity.

## In the level Editor

### Open the project

Once you could build and run the Level Editor, click the menu *File/Load Project...* and select file *[root]/SampleProject/LevelEditorSettings/SampleProject.settings*

You can start a level from scratch or load the sample level file by clicking the menu *File/Open...* and select the file located in the assets folder *[root]/SampleProject/Assets/Levels/SampleLevel.bytes*

### Editor interface

The *Game Board Elements* panel contains elements divided into categories, *Objects* and *Aninals*, which can be used as arbitrary tags to organize your assets. We also have the tag *All* applied to every element to have them all gather in a single panel.

The *Placeholders* foldable panel contains assets used to visually represent logic elements which do not have any graphical representation in the game. This can be ignored for now in the scope of this sample project.

The *Components* panel on the left hand side presents some simple behaviours we included in the project.

You can enhance the editor interface by implementing extensions. More documentation about that coming soon.

We included some simple extensions in the sample project. You can enable the *Grid* extension via the checkbox in the menu *Extensions/Canvas Renderers/Background/Grid*. Clicking the menu item itself opens up a property window to customize the grid appearance. Alternatively you can set a background image to the canvas via the *Background Image* canvas renderer.

Two additional foreground renderers are available.
- *Component Preview* addd a subtle overlay on the elements to visually identify which components are attached to it without selecting it.
- *Element Identifier* adds an additional overlay to know the element ID without selecting it.

Here is what the editor interface should look like now.

![Level Editor Interface](https://www.bitcraft.co.jp/pub/github/level-editor/level-editor-interface.png "Level Editor Interface") 

## In Unity 3D

Simply open the folder *'SampleProject'* as a Unity project and open the scene *Main.unity* and press play.

The Unity project file hierarchy follows the usual structure for small sample projects. The DLL file in the *plugins* folder was directly copied from the level editor build, and serves as a bridge between the level editor and Unity to parse the file format. Any update of the file format requires you to update this DLL in Unity to parse the binary files correctly.

The level sample file contains two blocks which are automatically loaded as you start the scene.
You can switch between blocks using the buttons at the top of the screen.

The first block is quite simple with just a few elements, especially in the corners to see how the editor space maps onto the Unity scene world space.

![Unity Block 0](https://www.bitcraft.co.jp/pub/github/level-editor/unity-block-0.png "Unity Block #0") 

The second block is more advanced with elements having one or several components attached to them.

![Unity Block 1](https://www.bitcraft.co.jp/pub/github/level-editor/unity-block-1.png "Unity Block #1") 

### Version
Lastly tested with Unity 5.3.3 p1