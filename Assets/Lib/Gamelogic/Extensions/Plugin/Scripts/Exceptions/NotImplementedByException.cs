using System;

namespace Gamelogic.Extensions
{
    public class NotImplementedByException : NotImplementedException
    {
        public NotImplementedByException(Type type)
            : base("Not implemented by " + type)
        {

        }
    }
}
