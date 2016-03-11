using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using LayerDataReaderWriter;
using LayerDataReaderWriter.V9;

public class LevelLoader : MonoBehaviour
{
    public TextAsset levelToLoad;
    public AssetMap assetMap;

    private int currentBlockIndex = 0;
    private List<GameObject> blocks;

    protected void Start ()
    {
        blocks = new List<GameObject>();
        LoadLevelBlocks();
    }

    // OnClick callback of the canvas buttons to switch blocks
    public void DisplayBlock(int index)
    {
        Debug.Assert(index >= 0 && index < blocks.Count, "Invalid block index");

        if (index == currentBlockIndex)
            return;

        ResetComponents(currentBlockIndex);
        blocks[currentBlockIndex].SetActive(false);

        currentBlockIndex = index;

        blocks[currentBlockIndex].SetActive(true);
    }

    // find all component in hierarchy of the given block, and call their Reset method
    private void ResetComponents(int blockIndex)
    {
        ComponentBase[] components = blocks[blockIndex].GetComponentsInChildren<ComponentBase>();
        foreach (var comp in components)
            comp.Reset();
    }

    // function which loads all the blocks in the scene
    private void LoadLevelBlocks()
    {
        var stream = new MemoryStream();
        stream.Write(levelToLoad.bytes, 0, levelToLoad.bytes.Length);
        stream.Seek(0, SeekOrigin.Begin);

        // read block headers
        var file = ReaderWriterManager.Read(stream, Encoding.UTF8, SampleConstants.LEVEL_EDITOR_FORMAT_VERSION) as LayerFile;
        Debug.Assert(file != null);

        // load the block one by one
        foreach (var block in file.Blocks)
        {
            // discard blocks which have been disabled in the editor
            if (block.SystemFlags.IsEnabled == false)
                continue;

            // create the block root
            GameObject newBlock = new GameObject();
            newBlock.name = "Block#" + block.Identifier;

            // populate the block with the elements created in the level editor
            var blockInstance = newBlock.AddComponent<BlockInstance>();
            blockInstance.Populate(block);

            // assign textures
            AssignTextures(blockInstance);

            // only keep the current block active
            newBlock.SetActive(blocks.Count == currentBlockIndex);
            blocks.Add(newBlock);
        }
    }

    private void AssignTextures(BlockInstance bi)
    {
        foreach (Transform child in bi.transform)
        {
            var meshRenderer = child.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                continue;

            var metadata = bi.MetaData[child.gameObject];
            var  graphicAsset = assetMap.GetGraphicAssetFromType(metadata.Type);
        
            // set the game object layer, sorting layer can also be set
            child.gameObject.layer = LayerMask.NameToLayer(metadata.LayerName);

            // set the texture if defined
            if (graphicAsset.mainTexture != null)
            {
                // we choose a Unity built-in shader for this sample
                meshRenderer.material.shader = Shader.Find("Sprites/Default");
                meshRenderer.material.mainTexture = graphicAsset.mainTexture;

                if (graphicAsset.textureMode == GraphicAsset.TextureMode.Repeat)
                {
                    graphicAsset.mainTexture.wrapMode = TextureWrapMode.Repeat;
                    Debug.Assert(graphicAsset.mainTexture.wrapMode == TextureWrapMode.Repeat, "Wrong wrap mode for texture " + graphicAsset.mainTexture.name);
                }
            }
        }
    }
}
