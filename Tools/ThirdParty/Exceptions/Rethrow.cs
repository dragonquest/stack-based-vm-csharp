using System;

namespace Util.Exceptions
{
    public class Rethrow
    {
        private readonly Exception _ex;

        public Rethrow(Exception ex)
        {
            _ex = ex;
        }

        public Rethrow If<T1>()
        {
            if (_ex is T1)
            {
                throw _ex;
            }
            return this;
        }

        public Rethrow If<T1, T2>()
        {
            If<T1>();
            If<T2>();
            return this;
        }

        public Rethrow If<T1, T2, T3>()
        {
            If<T1, T2>();
            If<T3>();
            return this;
        }

        public Rethrow If<T1, T2, T3, T4>()
        {
            If<T1, T2, T3>();
            If<T4>();
            return this;
        }

        public Rethrow If<T1, T2, T3, T4, T5>()
        {
            If<T1, T2, T3, T4>();
            If<T5>();
            return this;
        }

        public Rethrow If<T1, T2, T3, T4, T5, T6>()
        {
            If<T1, T2, T3, T4, T5>();
            If<T6>();
            return this;
        }

        public Rethrow IfNot<T1>()
        {
            if (!(_ex is T1))
            {
                throw _ex;
            }
            return this;
        }
    }
}
