using System;
using System.Collections.Generic;

namespace Util.Exceptions
{
    public class MultiException : Exception
    {
        public List<Exception> All;

        public int Count
        {
            get
            {
                return All.Count;
            }
        }

        public override string Message
        {
            get
            {
                var errorMessages = new List<string>();

                foreach (var ex in All)
                {
                   errorMessages.Add(ex.Message) ;
                }

                return String.Join(", ", errorMessages.ToArray());
            }
        }

        public MultiException()
        {
            All = new List<Exception>();
        }

        public void Add(Exception e)
        {
            All.Add(e);
        }

        public void ThrowIfFailed()
        {
            if (Count > 0)
            {
                throw this;
            }
        }
    }
}
