using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerDataReaderWriter.V9;
using LevelEditor.Extensibility.ReadOnlyViewModels;
using System.Windows.Input;
using Bitcraft.UI.Core;

namespace LevelEditor.ViewModels
{
    public class ComponentInstancePropertyViewModel : RootedViewModel, IReadOnlyComponentInstancePropertyViewModel
    {
        public string Name { get; private set; }

        private int typeIndex;
        public int TypeIndex
        {
            get { return typeIndex; }
            set
            {
                if (SetValue(ref typeIndex, value))
                    Type = (ComponentPropertyType)TypeIndex;
            }
        }

        private ComponentPropertyType type;
        public ComponentPropertyType Type
        {
            get { return type; }
            set
            {
                if (SetDataValue(ref type, value))
                    TypeIndex = (int)Type;
            }
        }

        public string Description { get; private set; }

        private bool booleanValue;
        public bool BooleanValue
        {
            get { return booleanValue; }
            set { SetDataValue(ref booleanValue, value); }
        }

        private int integerValue;
        public int IntegerValue
        {
            get { return integerValue; }
            set { SetDataValue(ref integerValue, value); }
        }

        private float floatValue;
        public float FloatValue
        {
            get { return floatValue; }
            set { SetDataValue(ref floatValue, value); }
        }

        private string stringValue;
        public string StringValue
        {
            get { return stringValue; }
            set { SetDataValue(ref stringValue, value); }
        }

        private Guid guidValue;
        public Guid GuidValue
        {
            get { return guidValue; }
            set { SetDataValue(ref guidValue, value); }
        }

        // the IsNew property is a trick to give focus to the property value only the first (when it is created)
        private bool isNew;
        public bool IsNew
        {
            get
            {
                var result = isNew;
                isNew = false;
                return result;
            }
        }

        public ICommand RemoveCommand { get; private set; }

        public bool IsPreset { get; private set; }

        public ComponentInstancePropertyViewModel(RootViewModel root, bool isPreset, ComponentProperty property, string description, bool isNew, Action remove)
            : base(root)
        {
            if (property != null)
            {
                Name = property.Name;
                Type = property.Type;
                Description = description;

                BooleanValue = property.BooleanValue;
                IntegerValue = property.IntegerValue;
                FloatValue = property.FloatValue;
                StringValue = property.StringValue;
                GuidValue = property.GuidValue;
            }

            this.isNew = isNew;
            IsPreset = isPreset;

            RemoveCommand = new AnonymousCommand(remove);
        }

        public ComponentProperty ProduceValue()
        {
            switch (Type)
            {
                case ComponentPropertyType.Boolean: return new ComponentProperty(Name, BooleanValue);
                case ComponentPropertyType.Integer: return new ComponentProperty(Name, IntegerValue);
                case ComponentPropertyType.Float: return new ComponentProperty(Name, FloatValue);
                case ComponentPropertyType.String: return new ComponentProperty(Name, StringValue);
                case ComponentPropertyType.Guid: return new ComponentProperty(Name, GuidValue);
                default:
                    throw new InvalidOperationException(string.Format("Unknown property type '{0}'.", Type));
            }
        }
    }
}
