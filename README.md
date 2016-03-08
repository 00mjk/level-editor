# Getting started

The Level Editor has been build with WPF, so it runs only on Windows.

To build the Level Editor, the simplest way is to execute the *Build.bat* script in the root folder.
This has been tested on very few machines, so it may fail on yours according to your configuration.

You can open the solution in [Visual Studio 2015 Community Edition](https://www.visualstudio.com/products/visual-studio-community-vs), available for free, and build it.

Then you can either run it from Visual Studio, or execute the *bin/Release/LevelEditor.exe* file.

# Level Editor

Once you could build and run the Level Editor, click the menu *File/Load Project...* and select file *[root]/SampleData/ProjectFiles/SampleProject.settings*.

From now on, the project is loaded so you should see components on the right of the window, and sprites on the left of the window.

Then load a level file by clicking the menu *File/Open...* and select any file in folder *[root]/SampleData/LevelFiles/*.

In the folder *[root]/Documentation* you can find another documentation explaining a bit more how to use the Level Editor.

# Warning

The Level Editor still does not have undo/redo feature, and we have no idea when we will have time to implement it.

It has an auto-backup feature to avoid too much lose in case of crash.
