using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Transactional
{
    public interface ITransactionalCommand
    {
        void Do();
        void Undo();
    }
}
