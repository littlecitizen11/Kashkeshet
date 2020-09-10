using System.Collections.Generic;

namespace Common
{
    public interface IMessageDestination
    {
        public List<string> Get();
    }
}
