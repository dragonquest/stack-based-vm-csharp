using System;
using System.Reflection.Emit;

// Needed for the stack:
using System.Collections.Generic;
using System.Linq;

using VM.Parsing.AST;

namespace VM.CodeGeneration.MSIL
{
    public partial class Compiler : Visitor, IVisitor
    {
        private class CodeGenHelper
        {
            public LocalBuilder AllocArray<T>(ILGenerator gen, int size)
            {
                LocalBuilder arr = gen.DeclareLocal(typeof(T));
                gen.Emit(OpCodes.Ldc_I4, size);
                gen.Emit(OpCodes.Newarr, typeof(T).GetElementType());
                gen.Emit(OpCodes.Stloc, arr);

                return arr;
            }

            public LocalBuilder AllocStack<T>(ILGenerator gen)
            {
                var genericStack = typeof(List<>);
                Type[] typeArgs = { typeof(T) };
                var stackType = genericStack.MakeGenericType(typeArgs);
                var ctor = TypeBuilder.GetConstructor(stackType, genericStack.GetConstructors().First());

                LocalBuilder stack = gen.DeclareLocal(typeof(List<T>));

                gen.Emit(OpCodes.Newobj, ctor);
                gen.Emit(OpCodes.Stloc, stack);

                return stack;
            }

            public void StackPush<T>(ILGenerator gen, LocalBuilder stack, int val)
            {
                var add = typeof(List<T>).GetMethod("Add");

                gen.Emit(OpCodes.Ldloc, stack);
                gen.Emit(OpCodes.Ldc_I4, val);
                gen.Emit(OpCodes.Callvirt, add);
            }

            public void StackPushFromCILStack<T>(ILGenerator gen, LocalBuilder stack, LocalBuilder tmp)
            {
                var add = typeof(List<T>).GetMethod("Add");

                gen.Emit(OpCodes.Stloc, tmp);
                gen.Emit(OpCodes.Ldloc, stack);
                gen.Emit(OpCodes.Ldloc, tmp);
                gen.Emit(OpCodes.Callvirt, add);
            }

            // Pops the value onto the .NET stack
            public void StackPop<T>(ILGenerator gen, LocalBuilder stack, LocalBuilder tmp)
            {
                // Properties access. Internally they just call 'get_':
                var getCount = typeof(List<int>).GetMethod("get_Count");
                var getItem = typeof(List<int>).GetMethod("get_Item");

                // Public method (That's the reason why the method is in CamelCase):
                var removeAt = typeof(List<int>).GetMethod("RemoveAt");

                gen.Emit(OpCodes.Ldloc, stack);
                gen.Emit(OpCodes.Callvirt, getCount);
                gen.Emit(OpCodes.Ldc_I4, 1);
                gen.Emit(OpCodes.Sub);
                gen.Emit(OpCodes.Stloc, tmp);

                gen.Emit(OpCodes.Ldloc, stack);
                gen.Emit(OpCodes.Ldloc, tmp);
                gen.Emit(OpCodes.Callvirt, getItem);
                // XXX(an): Item is onto the stack now
                //          if this does not work we need to introduce another tmp
                gen.Emit(OpCodes.Ldloc, stack);
                gen.Emit(OpCodes.Ldloc, tmp);
                gen.Emit(OpCodes.Callvirt, removeAt);
            }

            // Gets the stack size and pushes it onto the MSIL Stack
            public void StackSize<T>(ILGenerator gen, LocalBuilder stack)
            {
                // Properties access. Internally they just call 'get_':
                var getCount = typeof(List<int>).GetMethod("get_Count");

                gen.Emit(OpCodes.Ldloc, stack);
                gen.Emit(OpCodes.Callvirt, getCount);
                gen.Emit(OpCodes.Ldc_I4, 1);
                gen.Emit(OpCodes.Sub);
            }
        }
    }
}

