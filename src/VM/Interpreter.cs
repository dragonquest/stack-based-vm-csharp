using System;
using System.Collections.Generic;

namespace VM
{
    public partial class Interpreter
    {
        private IMonitor _monitor = null;
        private IDebugger _debugger = null;

        public Interpreter()
        {
            _monitor = new Monitor();
            _debugger = null;
        }

        public void Attach(IDebugger debugger)
        {
            _debugger = debugger;
        }

        public void SetMonitor(IMonitor monitor)
        {
            _monitor = monitor;
        }

        public void Run(List<int> code, int main)
        {
            var ctx = new Context(code);
            ctx.IP = main;

            var opcode = ctx.Code[ctx.IP];
            while (opcode != Bytecode.Halt && ctx.IP < ctx.Code.Count)
            {
                if (_debugger != null)
                {
                    ctx = _debugger.BeforeInstruction(ctx);
                }

                ctx.IP++;
                switch (opcode)
                {
                    case Bytecode.Push:
                        {
                            var val = ctx.Code[ctx.IP++];
                            ctx.Stack.Push(val);
                            ctx.SP++;
                        }
                        break;

                    case Bytecode.IEqual:
                        {
                            var b = ctx.Stack.Pop();
                            var a = ctx.Stack.Pop();
                            ctx.SP--;

                            if (a == b)
                            {
                                ctx.Stack.Push(1);
                            }
                            else
                            {
                                ctx.Stack.Push(0);
                            }
                        }
                        break;

                    case Bytecode.ILessThan:
                        {
                            var b = ctx.Stack.Pop();
                            var a = ctx.Stack.Pop();
                            ctx.SP--;

                            if (a < b)
                            {
                                ctx.Stack.Push(1);
                            }
                            else
                            {
                                ctx.Stack.Push(0);
                            }
                        }
                        break;

                    case Bytecode.Jump:
                        {
                            ctx.IP = ctx.Code[ctx.IP++];
                        }
                        break;

                    case Bytecode.JumpTrue:
                        {
                            var addr = ctx.Code[ctx.IP++];
                            var val = ctx.Stack.Pop();
                            ctx.SP--;
                            if (val == 1)
                            {
                                ctx.IP = addr;
                            }
                        }
                        break;

                    case Bytecode.JumpFalse:
                        {
                            var addr = ctx.Code[ctx.IP++];
                            var val = ctx.Stack.Pop();
                            ctx.SP--;
                            if (val == 0)
                            {
                                ctx.IP = addr;
                            }
                        }
                        break;

                    case Bytecode.IAdd:
                        {
                            var b = ctx.Stack.Pop();
                            var a = ctx.Stack.Pop();
                            ctx.Stack.Push(a + b);
                            ctx.SP--;
                        }
                        break;

                    case Bytecode.ISub:
                        {
                            var b = ctx.Stack.Pop();
                            var a = ctx.Stack.Pop();
                            ctx.Stack.Push(a - b);
                            ctx.SP--;
                        }
                        break;

                    case Bytecode.IMult:
                        {
                            var b = ctx.Stack.Pop();
                            var a = ctx.Stack.Pop();
                            ctx.Stack.Push(a * b);
                            ctx.SP--;
                        }
                        break;

                    case Bytecode.IDiv:
                        {
                            var b = ctx.Stack.Pop();
                            var a = ctx.Stack.Pop();
                            ctx.Stack.Push(a / b);
                            ctx.SP--;
                        }
                        break;

                    case Bytecode.GStore:
                        {
                            var val = ctx.Stack.Pop();
                            ctx.SP--;

                            int addr = ctx.Code[ctx.IP];
                            ctx.IP++;
                            ctx.Globals[addr] = val;
                        }
                        break;

                    case Bytecode.GLoad:
                        {
                            int addr = ctx.Code[ctx.IP];
                            ctx.IP++;
                            int val = ctx.Globals[addr];
                            ctx.Stack.Push(val);
                            ctx.SP++;
                        }
                        break;

                    case Bytecode.Print:
                        {
                            var val = ctx.Stack.Pop();
                            ctx.SP--;

                            _monitor.Display(String.Format("{0}\n", val));
                        }
                        break;

                    case Bytecode.PrintChar:
                        {
                            var val = ctx.Stack.Pop();
                            ctx.SP--;

                            _monitor.Display(String.Format("{0}", (char)val));
                        }
                        break;

                    case Bytecode.Load:
                        {
                            var arg  = ctx.Code[ctx.IP++];
                            var callArg = ctx.Stack[ctx.BP - (3 + arg)];

                            ctx.Stack.Push(callArg);
                            ctx.SP++;
                        }
                        break;

                    case Bytecode.Call:
                        {
                            var returnAddr = ctx.IP + 2;
                            var numArgs  = ctx.Code[ctx.IP++];
                            var targetAddr = ctx.Code[ctx.IP++];

                            ctx.Stack.Push(ctx.BP);
                            ctx.Stack.Push(numArgs);
                            ctx.Stack.Push(returnAddr);

                            ctx.SP += 3;
                            ctx.BP = ctx.SP;

                            ctx.IP = targetAddr;
                        }
                        break;

                    case Bytecode.Ret:
                        {
                            // Copying the remaining parameters which are on the
                            // stack into an intermediate "backup" stack:
                            var backupStack = new List<int>();
                            var currentSP = ctx.SP;
                            for (int i = ctx.BP; i < currentSP; i++)
                            {
                                var val = ctx.Stack.Pop();
                                backupStack.Push(val);
                                ctx.SP--;
                            }

                            var returnAddr  = ctx.Stack.Pop();
                            var numArgs = ctx.Stack.Pop();
                            ctx.BP  = ctx.Stack.Pop();
                            ctx.SP -= 3;

                            // Cleaning up the stack
                            while (numArgs > 0)
                            {
                                ctx.Stack.Pop();
                                ctx.SP--;
                                numArgs -= 1;
                            }

                            // Restoring the stack from the "backup stack"
                            // This way we can return values from the "function": 
                            while (backupStack.Count > 0)
                            {
                                ctx.Stack.Push(backupStack.Pop());
                                ctx.SP++;
                            }
                            backupStack.Clear();

                            ctx.IP = returnAddr;
                        }
                        break;
                }

                if (_debugger != null)
                {
                    ctx = _debugger.AfterInstruction(ctx);
                }

                opcode = ctx.Code[ctx.IP];
            }
        }
    }
}
