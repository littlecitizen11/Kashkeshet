using MenuBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClientCheck


{
    public class MenuBuilt
    {
        private Client _client;
        public MenuBuilt(Client client)
        {
            _client = client;
        }
        public void Execute()
        {
            Menu<int> menu = new Menu<int>();
            menu.AddOption(1, "Send global text", _client.SendGlobalMessage);
            menu.AddOption(2, "Send Private Message", _client.SendPrivateMessage);
            //menu.AddOption(3, "Send Message to Group", _client.SendGroupMessage);
            RunIntMenu intMenu = new RunIntMenu();
            intMenu.Menu = menu;
            intMenu.Run();
        }
    }
}
