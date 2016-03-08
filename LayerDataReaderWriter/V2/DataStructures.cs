using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LayerDataReaderWriter.V2
{
    /// <summary>
    /// Type of layer block element.
    /// </summary>
    /// <remarks>This enum is specific to the showcase application 1.</remarks>
    public enum LayerBlockElementType : byte
    {
        // boost category range=[0,19]

        /// <summary>
        /// Boost 1.
        /// </summary>
        BoostLv01 = 0,
        /// <summary>
        /// Boost 2.
        /// </summary>
        BoostLv02 = 1,
        /// <summary>
        /// Boost 3.
        /// </summary>
        BoostLv03 = 2,

        // gems/coins range=[20, 39]

        /// <summary>
        /// Red gem.
        /// </summary>
        GemRed = 20,
        /// <summary>
        /// Blue gem.
        /// </summary>
        GemBlue = 21,
        /// <summary>
        /// Purple gem.
        /// </summary>
        GemPurple = 22,

        // Obstacles range=[40, 99]

        /// <summary>
        /// Static obstacle.
        /// </summary>
        ObstacleStatic = 40,
        /// <summary>
        /// Breakable obstacle.
        /// </summary>
        ObstacleBreakable = 41,

        // Power Ups range=[100, 149]

        /// <summary>
        /// Bullet-Time Power Up.
        /// </summary>
        PowerUpBulletTime = 100,
        /// <summary>
        /// Shield Power Up.
        /// </summary>
        PowerUpShield = 101,

        // Logic elements range=[200,229]

        /// <summary>
        /// Triggerer.
        /// </summary>
        Triggerer = 200,
        /// <summary>
        /// Triggerer for debug.
        /// </summary>
        TriggererDebug = 201,
        /// <summary>
        /// Waypoint 1.
        /// </summary>
        Waypoint01 = 202,
        /// <summary>
        /// Waypoint 2.
        /// </summary>
        Waypoint02 = 203,
        /// <summary>
        /// Waypoint 3.
        /// </summary>
        Waypoint03 = 204,
    }

    /// <summary>
    /// Represent a block element.
    /// </summary>
    public struct LayerBlockElement
    {
        /// <summary>
        /// Gets the position along the X axis, in unit space (0~1).
        /// </summary>
        public float Tx { get; private set; }
        /// <summary>
        /// Gets the position along the Y axis, in unit space (0~1).
        /// </summary>
        public float Ty { get; private set; }
        /// <summary>
        /// Gets the angle of rotation, in degrees.
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
        /// Gets the type of block element.
        /// </summary>
        public LayerBlockElementType Type { get; private set; }
        /// <summary>
        /// Gets the custom components attached to the current block element.
        /// </summary>
        public Component[] Components { get; private set; }

        /// <summary>
        /// Initializes the LayerBlockElement instance.
        /// </summary>
        /// <param name="tx">The position along the X axis, in unit space (0~1).</param>
        /// <param name="ty">The position along the Y axis, in unit space (0~1).</param>
        /// <param name="angle">The angle of rotation, in degrees.</param>
        /// <param name="sx">The scale along the X axis.</param>
        /// <param name="sy">The scale along the Y axis.</param>
        /// <param name="type">The type of block element.</param>
        /// <param name="components">The custom components attached to the current block element.</param>
        public LayerBlockElement(float tx, float ty, float angle, float sx, float sy, LayerBlockElementType type, Component[] components)
            : this()
        {
            Tx = tx;
            Ty = ty;
            Angle = angle;
            Sx = sx;
            Sy = sy;
            Type = type;
            Components = components;
        }
    }

    /// <summary>
    /// Flags available for each layer block.
    /// </summary>
    /// <remarks>This enum is specific to the showcase application 1.</remarks>
    public enum LayerBlockFlags
    {
        /// <summary>
        /// Tells whether the block is a block where take off is possible or not.
        /// </summary>
        TakeOffBlock,
        /// <summary>
        /// Tells whether the block is enabled or disabled.
        /// </summary>
        EnabledBlock,
    }

    /*
    public enum ComponentPropertyType
    {
        Boolean,
        Float,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ComponentPropertyValue
    {
        [FieldOffset(0)]
        public float FloatValue;
        [FieldOffset(0)]
        public bool BooleanValue;
    }

    public class ComponentProperty
    {
        public string Name { get; private set; }
        public ComponentPropertyType Type { get; private set; }
        public ComponentPropertyValue Value { get; private set; }

        public ComponentProperty(string name, float value)
        {
            Name = name;
            Type = ComponentPropertyType.Float;
            Value = new ComponentPropertyValue { FloatValue = value };
        }

        public ComponentProperty(string name, bool value)
        {
            Name = name;
            Type = ComponentPropertyType.Boolean;
            Value = new ComponentPropertyValue { BooleanValue = value };
        }
    }
    */

    /// <summary>
    /// Represent a custom component that can be attached to a block element.
    /// </summary>
    public class Component
    {
        /// <summary>
        /// Gets the type of component.
        /// </summary>
        public string Type { get; private set; }
        //public ComponentProperty[] Properties { get; private set; }

        /// <summary>
        /// Initializes the Component instance.
        /// </summary>
        /// <param name="type">The type of component.</param>
        public Component(string type/*, ComponentProperty[] properties*/)
        {
            Type = type;
            //Properties = properties;
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
        /// Gets whether the TakeOff flag is set or not.
        /// </summary>
        public bool IsTakeOffBlock { get; private set; }
        /// <summary>
        /// Gets whether the Enabled flag is set or not.
        /// </summary>
        public bool IsEnabled { get; private set; }
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
        /// <param name="isTakeOffBlock">Tells whether the block is a block where take off is possible or not.</param>
        /// <param name="isEnabled">Tells whether the block enabled or disabled.</param>
        /// <param name="elements">The contained block elements.</param>
        public LayerBlock(byte identifier, byte difficulty, float width, float height, bool isTakeOffBlock, bool isEnabled, LayerBlockElement[] elements)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");

            Identifier = identifier;
            Difficulty = difficulty;
            Width = width;
            Height = height;
            IsTakeOffBlock = isTakeOffBlock;
            IsEnabled = isEnabled;

            this.elements = elements;
        }

        internal LayerBlock(byte identifier, byte difficulty, float width, float height, bool isTakeOffBlock, bool isEnabled, Func<LayerBlockElement[]> readData)
        {
            Identifier = identifier;
            Difficulty = difficulty;
            Width = width;
            Height = height;
            IsTakeOffBlock = isTakeOffBlock;
            IsEnabled = isEnabled;

            this.readData = readData;
        }
    }
}
