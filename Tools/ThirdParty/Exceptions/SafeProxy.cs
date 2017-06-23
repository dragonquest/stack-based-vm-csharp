using System;
using System.Reflection;
using System.Dynamic;

namespace Util.Exceptions
{
    public class Safe : DynamicObject
    {
        readonly object _target;

        public Safe(object target)
        {
            _target = target;
        }

        public static dynamic Wrapper(object target)
        {
            return new Safe(target);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            try
            {
                result = _target.GetType().InvokeMember(binder.Name, BindingFlags.InvokeMethod, null, _target, args);
            }
            catch (Exception e)
            {
                result = e;

                if (e.InnerException != null)
                {
                    result = e.InnerException;
                }
            }

            return true;
        }
    }
}
