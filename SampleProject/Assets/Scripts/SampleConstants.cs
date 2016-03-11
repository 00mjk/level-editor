
public static class SampleConstants
{
    public const int LEVEL_EDITOR_FORMAT_VERSION = 9;
    
    // reference block size to perform a coherent scale between unity and the level editor
    public const float LEVEL_EDITOR_BLOCK_WIDTH = 1000.0f;
    public const float LEVEL_EDITOR_BLOCK_HEIGHT = 440.0f;
    public const float LEVEL_EDITOR_SNAP_SIZE = 20.0f;
    public const float LEVEL_EDITOR_HORIZONTAL_CELL_COUNT = LEVEL_EDITOR_BLOCK_WIDTH / LEVEL_EDITOR_SNAP_SIZE;
    public const float LEVEL_EDITOR_VERTICAL_CELL_COUNT = LEVEL_EDITOR_BLOCK_HEIGHT / LEVEL_EDITOR_SNAP_SIZE;

    // layers definitions in the level editor, which only purpose here is to show how to read the data back in Unity
    public const string LAYER_BACKGROUND = "Background";
    public const string LAYER_ANIMALS = "Animals";
}
