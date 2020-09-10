using System;

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
