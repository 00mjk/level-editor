using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LayerDataReaderWriter;
using LevelEditor.Behaviors;
using LevelEditor.Core;
using LevelEditor.Core.Settings.V1;
using LevelEditor.Extensibility.ReadOnlyViewModels;

namespace LevelEditor.ViewModels
{
    public class ElementToolBoxElementViewModel : RootedViewModel, IDragSource, IReadOnlyElementToolBoxElementViewModel
    {
        public LayerBlockElementDefinition Definition { get; private set; }

        public string IconFullPath { get; private set; }
        public ushort Type { get; private set; }

        private string fileNameWithoutExtension;
        private string typeName;

        private bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
            private set { SetValue(ref isVisible, value); }
        }

        public ElementToolBoxElementViewModel(RootViewModel root, LayerBlockElementDefinition elementDef)
            : base(root)
        {
            Definition = elementDef;

            IconFullPath = elementDef.IconFullPath;
            Type = elementDef.Type;

            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(IconFullPath).ToLowerInvariant();
            typeName = elementDef.Type.ToString().ToLowerInvariant();
        }

        public DragDropEffects BeginDrag(Point position)
        {
            return DragDropEffects.Copy;
        }

        public void SearchTextChanged(string newString)
        {
            if (string.IsNullOrWhiteSpace(newString))
                IsVisible = true;
            else
            {
                newString = newString.ToLowerInvariant();
                IsVisible = fileNameWithoutExtension.Contains(newString) || typeName.Contains(newString);
            }
        }
    }

    public class ElementToolBoxCategoryViewModel : RootedViewModel, IReadOnlyElementToolBoxCategoryViewModel
    {
        public string Name { get; private set; }
        public IEnumerable<ElementToolBoxElementViewModel> Elements { get; private set; }

        public ElementToolBoxCategoryViewModel(RootViewModel root, string categoryName, IEnumerable<ElementToolBoxElementViewModel> elements)
            : base(root)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Invalid 'categoryName' value.", "categoryName");
            if (elements == null)
                throw new ArgumentNullException("elements");

            Name = categoryName;
            Elements = elements;
        }

        IEnumerable<IReadOnlyElementToolBoxElementViewModel> IReadOnlyElementToolBoxCategoryViewModel.Elements
        {
            get { return Elements; }
        }
    }
}
