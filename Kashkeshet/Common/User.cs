using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable]
    public class User
    {
        
        public string UserName { get; set; }
        public User(string userName)
        {
            UserName = userName;
        }
    }
}
