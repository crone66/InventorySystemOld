using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventorySystem
{
    public class Inventory
    {
        private List<Stack> inventorySlots;
        private int maxInventorySlots;

        public List<Stack> InventorySlots
        {
            get
            {
                return inventorySlots;
            }
        }

        public Inventory()
            : this(32)
        {
        }

        public Inventory(int inventorySlots)
        {
            if (inventorySlots <= 0)
                throw new ArgumentOutOfRangeException("inventorySlots", inventorySlots, "Value must be greater then zero.");

            this.inventorySlots = new List<Stack>();
            this.maxInventorySlots = inventorySlots;
        }

        public int AddItem(int itemId, int value, int maxStackValue, bool fill)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException("value", value, "Value must be greater then zero.");

            List<Stack> stacks = inventorySlots.Where(s => s.ItemId == itemId).ToList();
            if (stacks == null)
                throw new Exception("Item doesn't exists!");


            int rest = value - ((maxStackValue * stacks.Count) - stacks.Sum(s => s.Value));
            if (rest > 0)
            {
                int requiredStacks = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(rest) / Convert.ToDouble(maxStackValue)));
                bool enoughSpace = requiredStacks + inventorySlots.Count <= maxInventorySlots;
                if(enoughSpace || fill)
                {
                    SetToMax(itemId, maxStackValue);
                    return CreateStacks(requiredStacks, itemId, rest, maxStackValue);
                }
            }
            else
            {
                return FillSlots(itemId, value, maxStackValue);
            }

            return value;
        }

        public int RemoveItem(int itemId, int value, bool ignoreNegativeResults)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException("value", value, "Value must be greater then zero.");

            Stack[] stacks = inventorySlots.FindAll(s => s.ItemId == itemId).ToArray();
            if (stacks == null)
                throw new Exception("Item doesn't exists!");

            int avalibleValue = stacks.Sum(s => s.Value);
            if(avalibleValue >= value || ignoreNegativeResults)
            {
                int i = 0;
                while (value > 0 || i < stacks.Length)
                {
                    int toRemove = value > stacks[i].Value ? stacks[i].Value : value;
                    if (RemoveItemFromStack(stacks[i].Index, toRemove, ignoreNegativeResults))
                        value -= toRemove;

                    i++;
                }
            }

            return value;
        }

        public bool RemoveItemFromStack(int stackIndex, int value, bool ignoreNegativeResults)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException("value", value, "Value must be greater then zero.");

            int fromIndex = inventorySlots.FindIndex(s => s.Index == stackIndex);
            if (fromIndex >= 0)
            {
                int result = inventorySlots[fromIndex].Value - value;
                if(result > 0)
                {
                    inventorySlots[fromIndex] = new Stack(stackIndex, inventorySlots[fromIndex].ItemId, result);
                    return true;
                }
                else if(result <= 0)
                {
                    if (ignoreNegativeResults || result == 0)
                    {
                        inventorySlots.RemoveAt(fromIndex);
                        return true;                     
                    }
                }
            }
            return false;
        }

        public bool MoveSlots(int sourceStackIndex, int destinationStackIndex)
        {
            int fromIndex = inventorySlots.FindIndex(s => s.Index == sourceStackIndex);
            if(fromIndex >= 0)
            {
                int toIndex = inventorySlots.FindIndex(s => s.Index == destinationStackIndex);
                if(toIndex >= 0)
                {
                    Stack sourceStack = inventorySlots[fromIndex];
                    Stack destinationStack = inventorySlots[toIndex];
                    inventorySlots[fromIndex] = new Stack(destinationStackIndex, sourceStack.ItemId, sourceStack.Value);
                    inventorySlots[toIndex] = new Stack(sourceStackIndex, destinationStack.ItemId, destinationStack.Value);
                    return true;
                }
                else
                {
                    inventorySlots[fromIndex] = new Stack(destinationStackIndex, inventorySlots[fromIndex].ItemId, inventorySlots[fromIndex].Value);
                    return true;
                }
            }

            return false;
        }

        public bool MergeSlots(int sourceIndex, int destinationIndex, int maxStackValue)
        {
            if (maxStackValue <= 0)
                throw new ArgumentOutOfRangeException("maxStackValue", maxStackValue, "maxStackValue must be greater then zero.");

            int fromIndex = inventorySlots.FindIndex(s => s.Index == sourceIndex);
            if (fromIndex >= 0)
            {
                int toIndex = inventorySlots.FindIndex(s => s.Index == destinationIndex);
                if (toIndex >= 0)
                {
                    Stack sourceStack = inventorySlots[fromIndex];
                    Stack destinationStack = inventorySlots[toIndex];
                    if (sourceStack.ItemId == destinationStack.ItemId)
                    {
                        int freeSlots = maxStackValue - destinationStack.Value;
                        if (freeSlots >= sourceStack.Value)
                        {
                            inventorySlots[toIndex] = new Stack(destinationStack.Index, destinationStack.ItemId, maxStackValue);
                            inventorySlots.RemoveAt(fromIndex);
                        }
                        else
                        {
                            inventorySlots[fromIndex] = new Stack(sourceStack.Index, sourceStack.ItemId, sourceStack.Value - freeSlots);
                            inventorySlots[toIndex] = new Stack(destinationStack.Index, destinationStack.ItemId, maxStackValue);
                        }
                        return true;
                    }
                }
            }

            return false;
        }


        private int FillSlots(int itemId, int value, int maxStackValue)
        {
            int itemsLeft = value;
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].ItemId == itemId)
                {
                    if (maxStackValue > inventorySlots[i].Value)
                    {
                        int freeSlots = maxStackValue - inventorySlots[i].Value;
                        if (freeSlots >= itemsLeft)
                        {
                            inventorySlots[i] = new Stack(inventorySlots[i].Index, itemId, inventorySlots[i].Value + itemsLeft);
                            itemsLeft = 0;
                        }
                        else
                        {
                            inventorySlots[i] = new Stack(inventorySlots[i].Index, itemId, maxStackValue);
                            itemsLeft -= freeSlots;
                        }
                    }
                }
            }

            return itemsLeft;
        }

        private void SetToMax(int itemId, int maxStackValue)
        {
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].ItemId == itemId)
                    inventorySlots[i] = new Stack(inventorySlots[i].Index, itemId, maxStackValue);
            }
        }

        private int CreateStacks(int requiredStacks, int itemId, int value, int maxStackValue)
        {
            while (requiredStacks > 0 && inventorySlots.Count < maxInventorySlots)
            {
                if (maxStackValue < value)
                {
                    inventorySlots.Add(new Stack(GetUnusedIndex(), itemId, maxStackValue));
                    value -= maxStackValue;
                }
                else
                {
                    inventorySlots.Add(new Stack(GetUnusedIndex(), itemId, value));
                    value = 0;
                }
                requiredStacks--;
            }

            return value;
        }

        private int GetUnusedIndex()
        {
            int[] indices = inventorySlots.Select(s => s.Index).OrderBy(i => i).ToArray();
            for (int i = 0; i < indices.Length; i++)
            {
                if (i != indices[i])
                    return i;
            }
            return indices.Length;
        }
    }
}
