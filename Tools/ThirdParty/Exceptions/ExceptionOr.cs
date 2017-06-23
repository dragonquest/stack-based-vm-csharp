using System;

namespace Util.Exceptions
{
    public class ExceptionOr<T>
    {
        private readonly object _target;

        public ExceptionOr(object target)
        {
            _target = target;
        }

        public static implicit operator ExceptionOr<T>(Exception ex)
        {
           return new ExceptionOr<T>(ex);
        }

        public static implicit operator ExceptionOr<T>(T val)
        {
           return new ExceptionOr<T>(val);
        }

        public bool HasFailed()
        {
            // XXX(an): The if is necessary otherwise
            // an exception is thrown regarding
            // InvalidCastException.
            if (_target.GetType() == typeof(T))
            {
                return false;
            }
            return true;
        }

        public bool HasFailed<T1>()
        {
            if (_target.GetType() == typeof(T1))
            {
                return true;
            }
            return false;
        }

        // XXX(an):   Supporting up to 6 type parameters.
        //            Apparently it has to be done this way
        //            since C# does not support variadic type parameters.
        //            Also solving this via extension methods
        //            resulted in type inference problems.
        //
        // TODO(an):  Find out how to solve this in a cleaner way 
        public bool HasFailed<T1, T2>()
        {
            if (HasFailed<T1>() || HasFailed<T2>())
            {
                return true;
            }

            return false;
        }

        public bool HasFailed<T1, T2, T3>()
        {
            if (HasFailed<T1, T2>() || HasFailed<T3>())
            {
                return true;
            }

            return false;
        }

        public bool HasFailed<T1, T2, T3, T4>()
        {
            if (HasFailed<T1, T2, T3>() || HasFailed<T4>())
            {
                return true;
            }

            return false;
        }

        public bool HasFailed<T1, T2, T3, T4, T5>()
        {
            if (HasFailed<T1, T2, T3, T4>() || HasFailed<T5>())
            {
                return true;
            }

            return false;
        }

        public bool HasFailed<T1, T2, T3, T4, T5, T6>()
        {
            if (HasFailed<T1, T2, T3, T4, T5>() || HasFailed<T6>())
            {
                return true;
            }

            return false;
        }

        public Rethrow Rethrow()
        {
            return new Rethrow(GetException());
        }

        public bool IsOk()
        {
            return !HasFailed();
        }

        public T GetValue()
        {
            if (HasFailed())
            {
                throw (Exception)_target;
            }

            return (T)_target;
        }

        public Exception GetException()
        {
            return (Exception)_target;
        }
    }
}

