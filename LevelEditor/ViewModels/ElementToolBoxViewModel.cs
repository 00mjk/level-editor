using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Extensibility.ReadOnlyViewModels;

namespace LevelEditor.ViewModels
{
    public class ElementToolBoxViewModel : RootedViewModel, IReadOnlyElementToolBoxViewModel
    {
        public IEnumerable<ElementToolBoxCategoryViewModel> Categories { get; private set; }

        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (SetValue(ref searchText, value))
                {
                    foreach (var element in Categories.SelectMany(x => x.Elements))
                        element.SearchTextChanged(SearchText);
                }
            }
        }

        public ElementToolBoxViewModel(RootViewModel root)
            : base(root)
        {
            var dic = new Dictionary<string, List<ElementToolBoxElementViewModel>>();

            foreach (var element in Root.Settings.GameElements.Elements)
            {
                var categories = SplitCategories(element.Categories);

                foreach (var categ in categories)
                {
                    List<ElementToolBoxElementViewModel> itemList;

                    if (dic.TryGetValue(categ, out itemList) == false)
                    {
                        itemList = new List<ElementToolBoxElementViewModel>();
                        dic[categ] = itemList;
                    }

                    itemList.Add(new ElementToolBoxElementViewModel(root, element));
                }
            }

            var result = new ElementToolBoxCategoryViewModel[dic.Keys.Count];

            var n = 0;
            foreach (var kv in dic)
                result[n++] = new ElementToolBoxCategoryViewModel(root, kv.Key, kv.Value.ToArray());

            Categories = new ReadOnlyCollection<ElementToolBoxCategoryViewModel>(result);
        }

        private IEnumerable<string> SplitCategories(string categories)
        {
            return categories
                .Split(',', ';', ':')
                .Select(s => s.Trim())
                .Where(s => s.Length > 0);
        }

        IEnumerable<IReadOnlyElementToolBoxCategoryViewModel> IReadOnlyElementToolBoxViewModel.Categories
        {
            get { return Categories; }
        }
    }
}
