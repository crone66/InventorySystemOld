using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem;
namespace InventorySystemExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Inventory inv = new Inventory(7);
            inv.AddItem(1, 49, 10, false);
            inv.AddItem(2, 1, 10, true);
            inv.AddItem(2, 2, 10, true);
            inv.AddItem(3, 12, 10, true);

            for (int i = 0; i < inv.InventorySlots.Count; i++)
            {
                Console.WriteLine(inv.InventorySlots[i].ToString());
            }
            Console.ReadKey();
        }
    }
}
