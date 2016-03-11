using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LayerData = LayerDataReaderWriter.V9;
using System.Reflection;

public class BlockElementMetadata
{
    public ushort Type;
    public string LayerName;
}

// A block is the whole level you can see on screen,
// and which is composed of several block elements.
public class BlockInstance : MonoBehaviour
{
    public int Identifier
    { get; private set; }

    public int Difficulty
    { get; private set; }

    // block meta data
    public Dictionary<GameObject, BlockElementMetadata> MetaData
    { get; private set; }

    // sample flag defined in the level editor project which
    // can be customized to hold any property your game needs
    public bool SampleFlag
    { get; private set; }
    
    // function to parse the layer data set via the level editor
    private string GetLayerNameFromLayerDataValue(byte layer)
    {
        switch (layer)
        {
            case 0: return SampleConstants.LAYER_BACKGROUND;
            case 1: return SampleConstants.LAYER_ANIMALS;
            default:
                Debug.Assert(false, "Unknown layer. Invalid Level editor data.");
                break;
        }

        return null;
    }

    // populate the block with elements created in the level editor
    public void Populate(LayerData.LayerBlock block)
    {
        // check block size
        Debug.Assert(block.Width == SampleConstants.LEVEL_EDITOR_BLOCK_WIDTH, "Block width must be " + SampleConstants.LEVEL_EDITOR_BLOCK_WIDTH);
        Debug.Assert(block.Height == SampleConstants.LEVEL_EDITOR_BLOCK_HEIGHT, "Block height must be " + SampleConstants.LEVEL_EDITOR_BLOCK_HEIGHT);

        // set basic properties
        Identifier = block.Identifier;
        Difficulty = block.Difficulty;

        // retrieve and set the dummy sample flag
        SampleFlag = block.UserFlags[0];

        // sanity check
        Debug.Assert(block.Elements != null, "Error: Failed reading block #" + Identifier + " data");
        Debug.Assert(block.Elements.Length > 0, "Error: Block #" + Identifier + " has no element");

        // allocate metadata structure
        MetaData = new Dictionary<GameObject, BlockElementMetadata>();

        var camera = Camera.main;
        float scalingFactor = SampleUtils.ComputeScaleFactor();

        var elementIndex = 0;
        foreach (LayerData.LayerBlockElement element in block.Elements)
        {
            // create the game object
            var go = new GameObject();
            go.name = "Element#" + block.Identifier.ToString("00") + "#" + elementIndex.ToString("00");

            // store element metadata
            MetaData[go] = new BlockElementMetadata
            {
                Type = element.Type,
                LayerName = GetLayerNameFromLayerDataValue(element.Layer),
            };

            // add the unit quad base mesh
            // (you will probably want to instantiate you own custom prefab mesh instead)
            var meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = SampleUtils.BuildQuadMesh(1.0f, 1.0f);

            // add a mesh renderer
            go.AddComponent<MeshRenderer>();

            // move mesh pivot point to match the editor pivot point
            var px = element.Px * 2.0f - 1.0f;
            var py = element.Py * 2.0f - 1.0f;
            SampleUtils.MoveQuadPivotPoint(go, new Vector2(px, py), true);

            // set the new element as a child
            go.transform.SetParent(this.transform);

            // Now we compute the transform of the element to match its transform in the editor
            //
            // Note: the editor stores coordinates as [0,1]x[0,1] as pixels in a texture
            //       (0,0) is the bottom-left corner
            //       We keep this coordinate system for local transforms
                        
            // we compute the size the entire block takes in "camera pixel space"
            float blockSizeRatio = block.Height / block.Width;
            float blockHeightCameraSpace = camera.pixelWidth * blockSizeRatio;

            // we compute the final position on the y-axis in camera pixel space
            float elementScaledPosY = element.Ty * blockHeightCameraSpace;

            // compute the element world space position, knowing that screenspace is defined in pixels.
            // The bottom-left of the screen is (0,0); the right-top is (pixelWidth,pixelHeight)
            Vector3 worldPosition = camera.ScreenToWorldPoint(
                new Vector3(element.Tx * camera.pixelWidth, elementScaledPosY, camera.nearClipPlane));

            // set the transform
            go.transform.localPosition = new Vector3(worldPosition.x, worldPosition.y, 0.0f);
            go.transform.localScale = new Vector3(element.Sx * scalingFactor, element.Sy * scalingFactor, 1.0f);
            go.transform.Rotate(Vector3.forward, -element.Angle);

            // add the collider type defined in the level editor
            switch (element.ColliderType)
            {
                case LayerData.ColliderType.None:
                    // do not add any collider
                    break;
                case LayerData.ColliderType.Square:
                    go.AddComponent<BoxCollider2D>();
                    break;
                case LayerData.ColliderType.Circle:
                    go.AddComponent<CircleCollider2D>();
                    break;
                default:
                    throw new Exception("Unknown collider type value: " + (int)element.ColliderType);
            }

            // attach the component behaviours
            foreach (var component in element.Components)
            {
                var type = Type.GetType(component.Type, false, true);

                if (type != null)
                {
                    if (typeof(ComponentBase).IsAssignableFrom(type))
                    {
                        var comp = go.AddComponent(type) as ComponentBase;
                        go.name += "_" + comp.NameSuffix;
                        SetComponentProperties(comp, component);

                        comp.Initialize();
                    }
                    else
                        throw new Exception(string.Format("Unknown .NET type '{0}'", component.Type));
                }
            }

            elementIndex += 1;
        }
    }
    
    private Type GetComponentPropertyType(LayerData.ComponentPropertyType type)
    {
        switch (type)
        {
            case LayerData.ComponentPropertyType.Boolean: return typeof(bool);
            case LayerData.ComponentPropertyType.Integer: return typeof(int);
            case LayerData.ComponentPropertyType.Float: return typeof(float);
            case LayerData.ComponentPropertyType.String: return typeof(string);
            case LayerData.ComponentPropertyType.Guid: return typeof(Guid);
        }
        return null;
    }

    /// Sets the values of the component properties to the properties or fields of the given instance.
    /// Note: Properties with a public getter and public fields are accepted.
    /// 
    /// <param name="instance">The instance of any .NET runtime object that will receive the values of the component properties.</param>
    /// <param name="component">The component that contains properties to transfer its values to the instance.</param>
    private void SetComponentProperties(object instance, LayerData.Component component)
    {
        if (instance == null ||
            component == null ||
            component.Properties == null ||
            component.Properties.Length == 0)
            return;

        var type = instance.GetType();

        // public members
        var bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        foreach (var componentProperty in component.Properties)
        {
            var value = componentProperty.GetUntypedValue();

            var property = type.GetProperty(
                componentProperty.Name,
                bindingFlags);

            if (property != null)
            {
                if (property.GetGetMethod(false) == null) // skip properties with no public getter
                    continue;

                var setMethod = property.GetSetMethod(true);
                if (setMethod == null) // skip properties with no setter (because won't be able to set value)
                    continue;

                if (property.PropertyType.IsEnum)
                {
                    if (CanSetEnum(componentProperty, property.PropertyType, ref value) == false) // skip properties of invalid enum type
                        continue;
                }

                try
                {
                    setMethod.Invoke(instance, new[] { value });
                }
                catch { }
            }
            else
            {
                // fallback to the field

                var field = type.GetField(componentProperty.Name, bindingFlags);
                if (field == null) // skip private fields (or if name can't be found)
                    continue;

                if (field.FieldType.IsEnum)
                {
                    if (CanSetEnum(componentProperty, field.FieldType, ref value) == false) // skip fields of invalid enum type
                        continue;
                }

                try
                {
                    field.SetValue(instance, value);
                }
                catch { }
            }
        }
    }

    private bool CanSetEnum(LayerData.ComponentProperty componentProperty, Type memberType, ref object value)
    {
        if (memberType.IsEnum)
        {
            if (componentProperty.Type == LayerData.ComponentPropertyType.String)
            {
                try
                {
                    value = Enum.Parse(memberType, componentProperty.StringValue, true);
                    return true;
                }
                catch { }
            }
            else if (componentProperty.Type == LayerData.ComponentPropertyType.Integer)
            {
                if (Enum.IsDefined(memberType, value))
                    return true;
            }
        }

        return false;
    }
}
