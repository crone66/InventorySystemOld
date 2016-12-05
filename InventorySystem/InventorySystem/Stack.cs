using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem
{
    public struct Stack
    {
        public int Index;
        public int ItemId;
        public int Value;

        public Stack(int index, int itemId, int value)
        {
            Index = index;
            ItemId = itemId;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("{{ Index: {0}; ItemName: {1}; Value: {2} }}", Index.ToString(), ItemId, Value.ToString());
        }
    }
}
