using System;
using Jareel;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string json = "{\"session\":{\"DebugEnabled\":true,\"DebugTriggered\":true},\"actions\":{\"history\":[]},\"inventory\":{\"mainBag\":[null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null],\"potions\":[null,null,null,null,null,null,null,null,null,null]},\"ui\":{\"InCombat\":false,\"InventoryOpen\":true},\"inventoryUI\":{\"OpenBag\":\"Potions\",\"SelectedSlot\":-1,\"Open\":true}}";
            var data = Json.Read(json);
        }
    }
}
