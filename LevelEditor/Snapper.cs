using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.ViewModels;

namespace LevelEditor
{
    public class Snapper : IDisposable
    {
        private bool snap;
        private RootViewModel root;

        public Snapper(RootViewModel root)
        {
            if (root == null)
                throw new ArgumentNullException("root");

            this.root = root;

            Initialize();
        }

        public Snapper(RootedViewModel rooted)
        {
            if (rooted == null)
                throw new ArgumentNullException("rooted");
            if (rooted.Root == null)
                throw new ArgumentException("Invalid 'rooted' argument.");

            this.root = rooted.Root;

            Initialize();
        }

        private static int balance = 0;

        private void Initialize()
        {
            snap = root.LayerData.IsSnappingToGrid;
            balance++;
        }

        public void Dispose()
        {
            if (root != null)
            {
                balance--;
                root.LayerData.IsSnappingToGrid = snap;
                root = null;
            }
        }
    }
}
