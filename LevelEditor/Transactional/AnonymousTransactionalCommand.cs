using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Transactional
{
    public class AnonymousTransactionalCommand : ITransactionalCommand
    {
        private Action redo;
        private Action undo;

        public AnonymousTransactionalCommand(Action redo, Action undo)
        {
            if (redo == null || undo == null)
                throw new ArgumentException("Both 'redo' and 'undo' arguments are mandatory.");

            this.redo = redo;
            this.undo = undo;
        }

        public void Do()
        {
            redo();
        }

        public void Undo()
        {
            undo();
        }
    }
}
