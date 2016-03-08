using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LayerDataReaderWriter.V5
{
    /// <summary>
    /// Represent a block element.
    /// </summary>
    public struct LayerBlockElement
    {
        /// <summary>
        /// Gets the identifier of the block.
        /// It is unique among the other elements of the same block.
        /// </summary>
        public ushort ID { get; private set; }
        /// <summary>
        /// Gets the unit position along the X axis.
        /// </summary>
        public float Tx { get; private set; }
        /// <summary>
        /// Gets the unit position along the Y axis.
        /// </summary>
        public float Ty { get; private set; }
        /// <summary>
        /// Gets the unit X pivot position.
        /// </summary>
        public float Px { get; private set; }
        /// <summary>
        /// Gets the unit Y pivot position.
        /// </summary>
        public float Py { get; private set; }
        /// <summary>
        /// Gets the rotation angle along the Z axis.
        /// </summary>
        public float Angle { get; private set; }
        /// <summary>
        /// Gets the scale along the X axis.
        /// </summary>
        public float Sx { get; private set; }
        /// <summary>
        /// Gets the scale along the Y axis.
        /// </summary>
        public float Sy { get; private set; }
        /// <summary>
        /// Gets the type of element.
        /// </summary>
        public byte Type { get; private set; }
        /// <summary>
        /// Gets the attached components.
        /// </summary>
        public Component[] Components { get; private set; }

        /// <summary>
        /// Initializes the LayerBlockElement instance.
        /// </summary>
        /// <param name="id">The unique identifier of the block element, among the block.</param>
        /// <param name="tx">The position along the X axis, in unit space (0~1).</param>
        /// <param name="ty">The position along the Y axis, in unit space (0~1).</param>
        /// <param name="px">The position of the pivot along the X axis, in unit space (0~1).</param>
        /// <param name="py">The position of the pivot along the Y axis, in unit space (0~1).</param>
        /// <param name="angle">The angle of rotation, in degrees.</param>
        /// <param name="sx">The scale along the X axis.</param>
        /// <param name="sy">The scale along the Y axis.</param>
        /// <param name="type">The type of block element.</param>
        /// <param name="components">The custom components attached to the current block element.</param>
        public LayerBlockElement(ushort id, float tx, float ty, float px, float py, float angle, float sx, float sy, byte type, Component[] components)
            : this()
        {
            ID = id;
            Tx = tx;
            Ty = ty;
            Px = px;
            Py = py;
            Angle = angle;
            Sx = sx;
            Sy = sy;
            Type = type;
            Components = components;
        }
    }

    /// <summary>
    /// Represent a component strong type. (in term of static typing)
    /// </summary>
    public enum ComponentPropertyType : byte
    {
        /// <summary>
        /// Boolean type.
        /// </summary>
        Boolean,
        /// <summary>
        /// Integer type.
        /// </summary>
        Integer,
        /// <summary>
        /// Single precision floating point type. (32 bits)
        /// </summary>
        Float,
        /// <summary>
        /// String type.
        /// </summary>
        String,
        /// <summary>
        /// Globally unique identifier type.
        /// </summary>
        Guid
    }

    /// <summary>
    /// Represent a component custom property.
    /// </summary>
    public class ComponentProperty
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public ComponentPropertyType Type { get; private set; }
        /// <summary>
        /// Gets the boolean value if typed boolean, or default value otherwise.
        /// </summary>
        public bool BooleanValue { get; private set; }
        /// <summary>
        /// Gets the integer value if typed integer, or default value otherwise.
        /// </summary>
        public int IntegerValue { get; private set; }
        /// <summary>
        /// Gets the float value if typed float, or default value otherwise.
        /// </summary>
        public float FloatValue { get; private set; }
        /// <summary>
        /// Gets the string value if typed string, or default value otherwise.
        /// </summary>
        public string StringValue { get; private set; }
        /// <summary>
        /// Gets the Guid value if typed Guid, or default value otherwise.
        /// </summary>
        public Guid GuidValue { get; private set; }

        /// <summary>
        /// Initializes the ComponentProperty instance.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The boolean value. It makes this property to be typed boolean.</param>
        public ComponentProperty(string name, bool value)
        {
            Name = name;
            Type = ComponentPropertyType.Boolean;
            BooleanValue = value;
        }

        /// <summary>
        /// Initializes the ComponentProperty instance.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The integer value. It makes this property to be typed integer.</param>
        public ComponentProperty(string name, int value)
        {
            Name = name;
            Type = ComponentPropertyType.Integer;
            IntegerValue = value;
        }

        /// <summary>
        /// Initializes the ComponentProperty instance.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The float value. It makes this property to be typed float.</param>
        public ComponentProperty(string name, float value)
        {
            Name = name;
            Type = ComponentPropertyType.Float;
            FloatValue = value;
        }

        /// <summary>
        /// Initializes the ComponentProperty instance.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The string value. It makes this property to be typed string.</param>
        public ComponentProperty(string name, string value)
        {
            Name = name;
            Type = ComponentPropertyType.String;
            StringValue = value ?? string.Empty;
        }

        /// <summary>
        /// Initializes the ComponentProperty instance.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The Guid value. It makes this property to be typed Guid.</param>
        public ComponentProperty(string name, Guid value)
        {
            Name = name;
            Type = ComponentPropertyType.Guid;
            GuidValue = value;
        }
    }

    /// <summary>
    /// Represent a custom component that can be attached to a block element.
    /// </summary>
    public class Component
    {
        /// <summary>
        /// Gets the type of component.
        /// </summary>
        public string Type { get; private set; }
        /// <summary>
        /// Gets the custom property.
        /// </summary>
        public ComponentProperty Property { get; private set; }

        /// <summary>
        /// Initializes the Component instance.
        /// </summary>
        /// <param name="type">The type of component.</param>
        public Component(string type)
        {
            Type = type;
        }

        /// <summary>
        /// Initializes the Component instance.
        /// </summary>
        /// <param name="type">The type of component.</param>
        /// <param name="property">The custom property.</param>
        public Component(string type, ComponentProperty property)
        {
            Type = type;
            Property = property;
        }
    }

    /// <summary>
    /// Represent project independent, system flags.
    /// </summary>
    public class SystemFlags
    {
        /// <summary>
        /// Gets whether the related element is enabled or not.
        /// </summary>
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Initializes the SystemFlags instance.
        /// </summary>
        /// <param name="isEnabled">The isEnabled flag.</param>
        public SystemFlags(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
    }

    /// <summary>
    /// Represent a block in a layer.
    /// </summary>
    public class LayerBlock
    {
        /// <summary>
        /// Gets the unique identifier of the block among the current layer.
        /// </summary>
        public byte Identifier { get; private set; }
        /// <summary>
        /// Gets the difficulty of the current block.
        /// </summary>
        public byte Difficulty { get; private set; }
        /// <summary>
        /// Gets the width of the block, in pixels.
        /// </summary>
        public float Width { get; private set; }
        /// <summary>
        /// Gets the height of the block, in pixels.
        /// </summary>
        public float Height { get; private set; }
        /// <summary>
        /// Gets the custom flags of the block.
        /// </summary>
        public bool[] UserFlags { get; private set; }
        /// <summary>
        /// Gets the system flags of the block.
        /// </summary>
        public SystemFlags SystemFlags { get; private set; }
        /// <summary>
        /// Gets the contained block elements.
        /// </summary>
        /// <remarks>If data has not been read from file, then accessing this property reads the data and cache it.</remarks>
        public LayerBlockElement[] Elements
        {
            get
            {
                if (elements == null)
                {
                    elements = readData();
                    readData = null;
                }
                return elements;
            }
        }

        private LayerBlockElement[] elements;
        private Func<LayerBlockElement[]> readData;

        /// <summary>
        /// Initializes the LayerBlock instance.
        /// </summary>
        /// <param name="identifier">The unique identifier of the block among the current layer.</param>
        /// <param name="difficulty">The difficulty of the current block.</param>
        /// <param name="width">The width of the block, in pixels.</param>
        /// <param name="height">The height of the block, in pixels.</param>
        /// <param name="userFlags">The custom flags of the block.</param>
        /// <param name="systemFlags">The system flags of the block.</param>
        /// <param name="elements">The contained block elements.</param>
        public LayerBlock(byte identifier, byte difficulty, float width, float height, bool[] userFlags, SystemFlags systemFlags, LayerBlockElement[] elements)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");

            Identifier = identifier;
            Difficulty = difficulty;
            Width = width;
            Height = height;
            UserFlags = userFlags;
            SystemFlags = systemFlags;

            this.elements = elements;
        }

        internal LayerBlock(byte identifier, byte difficulty, float width, float height, bool[] userFlags, SystemFlags systemFlags, Func<LayerBlockElement[]> readData)
        {
            Identifier = identifier;
            Difficulty = difficulty;
            Width = width;
            Height = height;
            UserFlags = userFlags;
            SystemFlags = systemFlags;

            this.readData = readData;
        }
    }
}
