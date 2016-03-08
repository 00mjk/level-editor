using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    public class UniqueIdentifierManager
    {
        private ushort identifier;

        public ushort GetNext()
        {
            // increment first then return
            return ++identifier;
        }

        public void Initialize(ushort largestKnownIdentifier)
        {
            identifier = largestKnownIdentifier;
        }

        public void Update(ushort knownIdentifier)
        {
            if (knownIdentifier > identifier)
                identifier = knownIdentifier;
        }

        public void Reset()
        {
            identifier = 0;
        }
    }
}
