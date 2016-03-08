using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Transactional
{
    public class TransactionalCommandStack
    {
        private int index;
        private List<ITransactionalCommand> commands = new List<ITransactionalCommand>();

        private ITransactionalCommand savePointCommand;

        public void AddCommand(ITransactionalCommand command, bool runIt)
        {
            if (command == null)
                return;

            if (index < commands.Count)
                commands.RemoveRange(index, commands.Count - index);
            commands.Add(command);
            index = commands.Count;

            if (runIt)
                command.Do();
        }

        public void MarkAsSavePoint()
        {
            if (index == 0)
                savePointCommand = null;
            else
                savePointCommand = commands[index - 1];
        }

        private bool isSavePoint;
        public bool IsSavePoint
        {
            get { return isSavePoint; }
            set
            {
                if (isSavePoint != value)
                {
                    isSavePoint = true;
                    // TODO raise IsSavePointChanged event
                }
            }
        }

        public bool Undo()
        {
            if (index == 0)
                return false;

            index--;
            commands[index].Undo();

            if (index > 0)
                IsSavePoint = commands[index - 1] == savePointCommand;
            else
                IsSavePoint = savePointCommand == null;

            return true;
        }

        public bool Redo()
        {
            if (index >= commands.Count)
                return false;

            commands[index].Do();
            index++;

            IsSavePoint = commands[index - 1] == savePointCommand;
                
            return true;
        }

        public void Clear()
        {
            commands.Clear();
            index = 0;
        }
    }
}
