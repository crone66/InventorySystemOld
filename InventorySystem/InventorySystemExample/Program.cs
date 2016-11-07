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
            Inventory inv = new Inventory(30);
            inv.AddItem("test", 49, 10, false);
            inv.AddItem("test2", 1, 1, true);
            inv.AddItem("test", 12, 10, true);

            for (int i = 0; i < inv.InventorySlots.Count; i++)
            {
                Console.WriteLine(inv.InventorySlots[i].ToString());
            }
            Console.ReadKey();
        }
    }
}
