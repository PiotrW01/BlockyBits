using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src
{
    public class Inventory
    {
        Item[] items;
        int itemCount = 0;

        public Inventory(int size)
        {
            items = new Item[size];
        }


        public bool AddItem(Item item)
        {
            return false;
        }

        public bool RemoveItem(Item item)
        {
            return false;
        }

        public bool RemoveItemAt(int index)
        {
            return false;
        }



    }
}
