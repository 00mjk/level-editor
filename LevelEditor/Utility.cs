using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    public static class Utility
    {
        public static double DataFloatRound(float value)
        {
            return Math.Round(value, 6);
        }

        public static byte FindAvailableUniqueIdentifier(IEnumerable<byte> existingIdentifiers)
        {
            if (existingIdentifiers == null)
                throw new ArgumentNullException("existingIdentifiers");

            var ordered = existingIdentifiers
                .OrderBy(x => x)
                .ToArray();

            if (ordered.Length >= 256)
                return 0;

            byte id = 0;

            var e = ordered.GetEnumerator();
            while (e.MoveNext())
            {
                var current = (byte)e.Current;
                if (current == id)
                    id++;
                else
                    break;
            }

            return id;
        }
    }
}
