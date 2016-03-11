using UnityEngine;
using System;

[Serializable]
public class GraphicAsset
{
    public Texture2D mainTexture;

    // textureMode will set the mainTexture wrap mode and set the texture scale according to its size
    public enum TextureMode { Stretch, Repeat }
    public TextureMode textureMode;
}

public class AssetMap : ScriptableObject
{
    public GraphicAsset[] GraphicAssets;

    public GraphicAsset GetGraphicAssetFromType(ushort type)
    {
        Debug.Assert(type < GraphicAssets.Length, "Invalid type");
        var asset = GraphicAssets[type];

        return asset;
    }
}
