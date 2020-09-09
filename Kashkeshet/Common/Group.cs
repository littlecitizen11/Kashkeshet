using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Group
    {
        public List<User> Participants { get; set; }
        public string Name { get; set; }
        public Group(List<User> participants, string name)
        {
            Participants = participants;
            Name = name;
        }
    }
}
