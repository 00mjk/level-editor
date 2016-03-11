# Level Editor
## Purpose

This level editor has been developed for creating all the levels of the game [Jelly Arcade] available on [Android] and [iOS].

## Getting started

The Level Editor has been build with WPF, so it runs only on Windows.

To build the Level Editor, the simplest way is to execute the *Build.bat* script in the root folder.

You can also open the solution in [Visual Studio 2015 Community Edition], available for free, and build it.

Then you can either run it from Visual Studio, or execute *bin/Release/LevelEditor.exe* 

The folder *[root]/Documentation* contains documentation explaining a bit more how to use the Level Editor.

## Samples
### Editor project files

We included sample project files in the folder *SampleProject/LevelEditorSettings*

Open the file *SampleProject.settings* it in the editor via the menu *File/Load Project*. 

You should see some components being loaded in the left-hand side panel *Components*,  as well as a few *Game Board Elements* on the right-hand side panel.

Then you can load the sample level file *SampleLevel.bytes* located in the folder *SampleProject/Assets/Levels* via the menu *File/Open*

Once loaded, the first block of the level should be displayed automatically. The level file contains two blocks, you can browse them using the drop-down selector above the editor workspace.

![Level Editor Sample](https://www.bitcraft.co.jp/pub/github/level-editor/level-editor-sample.png "Level Editor Sample") 

### Unity 3D project

This folder also contains a Unity project showing how to import level files within [Unity 3D].

Please refer to the readme file in this folder for more information.

## Future work

The editor does not have undo/redo feature. 

For now we implemented an auto-backup feature to avoid too much lose in case of crash.

[Jelly Arcade]: <http://jellyarcade.com/>
[Android]: <https://play.google.com/store/apps/details?id=jp.co.bitcraft.jellyarcade>
[iOS]: <https://itunes.apple.com/us/app/jelly-arcade/id1035083184>
[Visual Studio 2015 Community Edition]: <https://www.visualstudio.com/products/visual-studio-community-vs>
[Unity 3D]: <https://unity3d.com/>