using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Bitcraft.UI.Core;
using LayerDataReaderWriter.V9;
using LevelEditor.Extensibility.ReadOnlyViewModels;
using System.Collections.ObjectModel;
using LevelEditor.Core.Settings.V1;
using System.Xml.Linq;
using System.Windows;

namespace LevelEditor.ViewModels
{
    public class PresetPropertyValue
    {
        public PresetProperty Parent { get; private set; }
        public string DisplayText { get; private set; }
        public object Value { get; private set; }
        public ICommand ClickCommand { get; private set; }

        public PresetPropertyValue(string displayText, object value, ICommand command)
        {
            DisplayText = displayText;
            Value = value;
            ClickCommand = command;
        }

        public void SetParent(PresetProperty parent)
        {
            Parent = parent;
        }
    }

    public class PresetProperty
    {
        public string DisplayText { get; private set; }
        public ComponentPropertyType Type { get; private set; }
        public string Description { get; private set; }
        public ICommand ClickCommand { get; private set; }
        public PresetPropertyValue[] Values { get; private set; }

        public PresetProperty(string displayText, ComponentPropertyType type, string description, PresetPropertyValue[] values, ICommand command)
        {
            DisplayText = displayText;
            Type = type;
            Description = description;

            if (string.IsNullOrWhiteSpace(Description))
                Description = "<no description>";

            Values = values;
            ClickCommand = command;
        }
    }

    public class ComponentInstanceViewModel : RootedViewModel, IDisposable, IReadOnlyComponentInstanceViewModel
    {
        private ElementInstanceViewModel parent;
        private ComponentViewModel component;

        private ObservableCollection<ComponentInstancePropertyViewModel> properties;
        public IReadOnlyObservableCollection<ComponentInstancePropertyViewModel> Properties { get; private set; }

        public string Type { get { return component.Type; } }
        public string ShortToolTipText { get { return component.Settings.ShortToolTipText; } }

        public ICommand AddPropertyCommand { get; private set; }

        public PresetProperty[] AvailableProperties { get; private set; }

        private bool isAvailable = true;
        public bool IsAvailable
        {
            get { return isAvailable; }
            set { SetDataValue(ref isAvailable, value); }
        }

        public ICommand CopyParametersCommand { get; private set; }
        public ICommand OverwritePasteParametersCommand { get; private set; }
        public ICommand AdditivePasteParametersCommand { get; private set; }

        public ComponentInstanceViewModel(RootViewModel root, ElementInstanceViewModel parent, ComponentViewModel component)
            : this(root, parent, component, null)
        {
            // nothing here
        }

        public ComponentInstanceViewModel(RootViewModel root, ElementInstanceViewModel parent, ComponentViewModel component, ComponentProperty[] componentProperties)
            : base(root)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (component == null)
                throw new ArgumentNullException("component");

            this.parent = parent;
            this.component = component;

            properties = new ObservableCollection<ComponentInstancePropertyViewModel>();

            CreatePresetProperties();

            if (componentProperties != null)
                AddProperties(componentProperties, true);

            Properties = new Bitcraft.UI.Core.ReadOnlyObservableCollection<ComponentInstancePropertyViewModel>(properties);

            component.PropertyChanged += component_PropertyChanged;

            RemoveComponentCommand = new AnonymousCommand(OnRemoveComponent);
            AddPropertyCommand = new AnonymousCommand(OnAddProperty);

            CopyParametersCommand = new AnonymousCommand(OnCopyParameters);
            OverwritePasteParametersCommand = new AnonymousCommand(OnOverwritePasteParameters);
            AdditivePasteParametersCommand = new AnonymousCommand(OnAdditivePasteParameters);
        }

        private void OnCopyParameters()
        {
            try
            {
                var element = new XElement("props", properties
                    .Select(p => new XElement("prop",
                        new XAttribute("n", p.Name),
                        new XAttribute("t", (int)p.Type),
                        new XAttribute("v", p.ProduceValue().GetUntypedValue().ToString()))));
                Clipboard.SetDataObject(element.ToString(), false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed to copy properties", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnOverwritePasteParameters()
        {
            OnPasteParameters(false);
        }

        private void OnAdditivePasteParameters()
        {
            OnPasteParameters(true);
        }

        private void OnPasteParameters(bool isAdditive)
        {
            try
            {
                var properties = ParsePropsXml(Clipboard.GetText());
                if (properties == null)
                    return;

                AddProperties(properties, isAdditive);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Failed to paste properties", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ComponentProperty[] ParsePropsXml(string xml)
        {
            var propsElement = XElement.Parse(xml);

            if (propsElement.Name != "props")
                return null;

            return propsElement
                .Elements("prop")
                .Select(e =>
                {
                    var type = (ComponentPropertyType)(int)e.Attribute("t");
                    object value;
                    if (Convert((string)e.Attribute("v"), type, out value) == false)
                        return null;
                    return CreatePropertyFromType((string)e.Attribute("n"), type, value);
                })
                .Where(x => x != null)
                .ToArray();
        }

        private static ComponentProperty CreatePropertyFromType(string name, ComponentPropertyType type)
        {
            switch (type)
            {
                case ComponentPropertyType.Boolean: return new ComponentProperty(name, default(bool));
                case ComponentPropertyType.Integer: return new ComponentProperty(name, default(int));
                case ComponentPropertyType.Float: return new ComponentProperty(name, default(float));
                case ComponentPropertyType.String: return new ComponentProperty(name, default(string));
                case ComponentPropertyType.Guid: return new ComponentProperty(name, default(Guid));
            }
            throw new ArgumentException("Invalid component property type");
        }

        private static ComponentProperty CreatePropertyFromType(string name, ComponentPropertyType type, object value)
        {
            switch (type)
            {
                case ComponentPropertyType.Boolean: return new ComponentProperty(name, (bool)value);
                case ComponentPropertyType.Integer: return new ComponentProperty(name, (int)value);
                case ComponentPropertyType.Float: return new ComponentProperty(name, (float)value);
                case ComponentPropertyType.String: return new ComponentProperty(name, (string)value);
                case ComponentPropertyType.Guid: return new ComponentProperty(name, (Guid)value);
            }
            throw new ArgumentException("Invalid component property type");
        }

        private static object GetDefaultValueFromType(ComponentPropertyType type)
        {
            switch (type)
            {
                case ComponentPropertyType.Boolean: return default(bool);
                case ComponentPropertyType.Integer: return default(int);
                case ComponentPropertyType.Float: return default(float);
                case ComponentPropertyType.String: return default(string);
                case ComponentPropertyType.Guid: return default(Guid);
            }
            throw new ArgumentException("Invalid component property type");
        }

        private void CreatePresetProperties()
        {
            PresetProperty customPresetProperty = null;

            var addPresetPropertyCommand = new AnonymousCommand(obj =>
            {
                if (obj is PresetProperty)
                {
                    var pp = (PresetProperty)obj;
                    var isPreset = pp != customPresetProperty;
                    AddProperty(CreatePropertyFromType(isPreset ? pp.DisplayText : string.Empty, pp.Type), pp.Description, isPreset, true);
                }
                else if (obj is PresetPropertyValue)
                {
                    var ppv = (PresetPropertyValue)obj;
                    AddProperty(CreatePropertyFromType(ppv.Parent.DisplayText, ppv.Parent.Type, ppv.Value), ppv.Parent.Description, true, true);
                }
            });

            List<PresetProperty> presetProperties;

            if (component.Settings.Properties == null || component.Settings.Properties.Length == 0)
                return;

            customPresetProperty = new PresetProperty(
                "[new custom property]",
                ComponentPropertyType.Boolean,
                "Add a custom property",
                null,
                addPresetPropertyCommand);

            presetProperties = component.Settings.Properties
                .Select(p => CreatePresetProperty(p, addPresetPropertyCommand))
                .Where(x => x != null)
                .ToList();

            presetProperties.Add(customPresetProperty);

            AvailableProperties = presetProperties.ToArray();
        }

        private static PresetProperty CreatePresetProperty(Property property, AnonymousCommand addPresetPropertyCommand)
        {
            ComponentPropertyType type;
            if (Convert(property.Type, out type) == false)
                return null;

            List<PresetPropertyValue> values = null;
            if (property.Values != null)
            {
                values = property.Values
                    .Select(v =>
                    {
                        object value;
                        if (Convert(v, type, out value) == false)
                            return null;
                        return new PresetPropertyValue(v.Trim(), value, addPresetPropertyCommand);
                    })
                    .Where(x => x != null)
                    .ToList();

                values.Add(new PresetPropertyValue("<empty>", GetDefaultValueFromType(type), addPresetPropertyCommand));
            }

            var result = new PresetProperty(
                property.Name,
                type,
                property.Description,
                values != null ? values.ToArray() : null,
                addPresetPropertyCommand);

            if (values != null)
            {
                foreach (var v in values)
                    v.SetParent(result);
            }

            return result;
        }

        private static bool Convert(string type, out ComponentPropertyType result)
        {
            result = ComponentPropertyType.Boolean;

            switch (type.ToLowerInvariant())
            {
                case "bool":
                case "boolean":
                    result = ComponentPropertyType.Boolean;
                    return true;
                case "int":
                case "integer":
                    result = ComponentPropertyType.Integer;
                    return true;
                case "float":
                    result = ComponentPropertyType.Float;
                    return true;
                case "string":
                    result = ComponentPropertyType.String;
                    return true;
                case "id":
                case "guid":
                    result = ComponentPropertyType.Guid;
                    return true;
            }

            return false;
        }

        private static bool IsValidValue(string value, ComponentPropertyType type)
        {
            object dummy;
            return Convert(value, type, out dummy);
        }

        private static bool Convert(string value, ComponentPropertyType type, out object result)
        {
            if (type == ComponentPropertyType.String)
            {
                result = value;
                return true;
            }

            result = null;

            if (value == null)
                return false;

            if (type == ComponentPropertyType.Boolean)
            {
                var tmp = value.ToLowerInvariant();
                if (tmp == "true" || tmp == "1" || tmp == "yes")
                {
                    result = true;
                    return true;
                }
                else if (tmp == "false" || tmp == "0" || tmp == "no")
                {
                    result = false;
                    return true;
                }
            }
            else if (type == ComponentPropertyType.Integer)
            {
                int i;
                if (int.TryParse(value.Trim(), out i))
                {
                    result = i;
                    return true;
                }
            }
            else if (type == ComponentPropertyType.Float)
            {
                float f;
                if (float.TryParse(value.Trim(), out f))
                {
                    result = f;
                    return true;
                }
            }
            else if (type == ComponentPropertyType.Guid)
            {
                Guid g;
                if (Guid.TryParse(value.Trim(), out g))
                {
                    result = g;
                    return true;
                }
            }

            return false;
        }

        private void component_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Type")
                OnPropertyChanged(e.PropertyName);
        }

        public ICommand RemoveComponentCommand { get; private set; }

        private void OnRemoveComponent()
        {
            Dispose();
            parent.RemoveComponent(this);
        }

        public Component ProduceComponent()
        {
            if (IsAvailable == false)
                return null;
            return new Component(Type, Properties.Where(x => x != null).Select(x => x.ProduceValue()).ToArray());
        }

        public void Dispose()
        {
            component.PropertyChanged -= component_PropertyChanged;
        }

        IReadOnlyObservableCollection<IReadOnlyComponentInstancePropertyViewModel> IReadOnlyComponentInstanceViewModel.Properties
        {
            get { return Properties; }
        }

        private void OnAddProperty()
        {
            if (AvailableProperties != null && AvailableProperties.Length > 0)
                return;

            AddProperty(null, null, false, true);
        }

        private void AddProperties(ComponentProperty[] componentProperties, bool isAdditive)
        {
            foreach (var prop in componentProperties.Where(x => x != null))
            {
                var isPreset = false;
                string description = null;
                if (AvailableProperties != null)
                {
                    var found = AvailableProperties.FirstOrDefault(x => x.DisplayText == prop.Name && x.Type == prop.Type);
                    if (found != null)
                    {
                        isPreset = true;
                        description = found.Description;
                    }
                }

                if (isAdditive)
                    AddProperty(prop, description, isPreset, false);
                else
                {
                    var existingProp = properties.FirstOrDefault(c => c.Name == prop.Name && c.Type == prop.Type);
                    if (existingProp == null)
                        AddProperty(prop, description, isPreset, false);
                    else
                    {
                        if (prop.Type == ComponentPropertyType.Boolean)
                            existingProp.BooleanValue = prop.BooleanValue;
                        else if (prop.Type == ComponentPropertyType.Integer)
                            existingProp.IntegerValue = prop.IntegerValue;
                        else if (prop.Type == ComponentPropertyType.Float)
                            existingProp.FloatValue = prop.FloatValue;
                        else if (prop.Type == ComponentPropertyType.String)
                            existingProp.StringValue = prop.StringValue;
                        else if (prop.Type == ComponentPropertyType.Boolean)
                            existingProp.GuidValue = prop.GuidValue;
                    }
                }
            }
        }

        private void AddProperty(ComponentProperty prop, string description, bool isPreset, bool isNew)
        {
            ComponentInstancePropertyViewModel item = null;

            item = new ComponentInstancePropertyViewModel(
                Root,
                isPreset,
                prop,
                description,
                isNew,
                () => { properties.Remove(item); Root.IsDataModified = true; });

            properties.Add(item);
            Root.IsDataModified = true;
        }
    }
}
