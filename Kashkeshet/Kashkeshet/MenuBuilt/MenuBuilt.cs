using Kashkeshet.Clients;
using MenuBuilder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kashkeshet
{
    public class MenuBuilt
    {
        /// <summary>
        /// Add Option by the client Actions
        /// </summary>
        //private readonly Client _client;
        private readonly SendData _send;
        public MenuBuilt(SendData send)
        {
            _send = send;
        }
        public void Execute()
        {
            Menu<int> menu = new Menu<int>();
            menu.AddOption(1, "Send global text", _send.SendGlobalMessage);
            menu.AddOption(2, "Send Private Message", _send.SendPrivateMessage);
            menu.AddOption(3, "Show active chats", _send.ShowActiveChats);
            RunIntMenu intMenu = new RunIntMenu();
            intMenu.Menu = menu;
            intMenu.Run();
        }
    }
}
