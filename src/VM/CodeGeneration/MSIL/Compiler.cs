using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using VM;
using VM.Util;
using VM.Parsing;
using VM.Parsing.AST;

namespace VM.CodeGeneration.MSIL
{
    public partial class Compiler : Visitor, IVisitor
    {
        private readonly ILGenerator _gen;
        private readonly CodeGenHelper _msilHelper;
        private readonly LocalBuilder _globals;
        private readonly LocalBuilder _stack;
        private readonly LocalBuilder _result;

        private readonly LocalBuilder _tmp1;
        private readonly LocalBuilder _tmp2;

        private readonly LocalBuilder _bp; // base pointer (stack)

        private readonly global::VM.Bytecode _bytecode;
        private readonly SymbolTable _symbolTable;

        private Dictionary<int,Label> _labels;

        private List<IError> _errors;

        public Compiler(ILGenerator gen, SymbolTable symbolTable)
        {
            _bytecode = new global::VM.Bytecode();

            _gen = gen;
            _symbolTable = symbolTable;
            _msilHelper = new CodeGenHelper();
            _globals = _msilHelper.AllocArray<int[]>(_gen, 200);
            _stack = _msilHelper.AllocStack<int>(_gen);

            // Generic result variable for Pop, etc.
            _result = _gen.DeclareLocal(typeof(int));

            // The tmp's are needed for storing values temporarily (mostly from/onto stack)
            _tmp1 = _gen.DeclareLocal(typeof(int));
            _tmp2 = _gen.DeclareLocal(typeof(int));

            _bp  = _gen.DeclareLocal(typeof(int));

            _labels = new Dictionary<int,Label>();
            _errors = new List<IError>();
        }

        public override void VisitEnter(Line line)
        {
            if (line.Type == LineType.Label)
            {
                var labelSymbol = _symbolTable.Lookup(SymbolType.Label, line.Label);
                if (labelSymbol == null)
                {
                    _errors.Add(new Error(String.Format("Failed to find symbol label: {0}", line.Label)));
                    return;
                }
                var msilSymbol = labelSymbol as MSILSymbol;

                _gen.MarkLabel(msilSymbol.Label);
            }
        }

        public override void Visit(Instruction ins)
        {
            // TODO: Do error checking
            Tuple<int, global::VM.Bytecode.Instruction> opcodeWithInstruction = _bytecode.FindByName(ins.Name);
            var opcode = opcodeWithInstruction.Item1;
            var instruction = opcodeWithInstruction.Item2;

            if (opcode == -1)
            {
                _errors.Add(new Error(String.Format("Error: Opcode for instruction '{0}' not found", instruction.Name)));
                return;
            }

            // _gen.EmitWriteLine(instruction.Name);
            switch (opcode)
            {
                case Bytecode.Push:
                    if (ins.Parameters[0].Type != ParameterType.Integer)
                    {
                        _errors.Add(new Error(String.Format("Parameter type is not an integer on line {0}", ins.LineNumber)));
                        return;
                    }

                    _msilHelper.StackPush<int>(_gen, _stack, ins.Parameters[0].IntegerValue);
                    // _gen.Emit(OpCodes.Ldc_I4, ins.Parameters[0].IntegerValue);

                    break;

                case Bytecode.IAdd:
                    _msilHelper.StackPop<int>(_gen, _stack, _tmp1);
                    _gen.Emit(OpCodes.Stloc, _tmp1);

                    _msilHelper.StackPop<int>(_gen, _stack, _tmp2);
                    _gen.Emit(OpCodes.Stloc, _tmp2);

                    _gen.Emit(OpCodes.Ldloc, _tmp2);
                    _gen.Emit(OpCodes.Ldloc, _tmp1);

                    _gen.Emit(OpCodes.Add);
                    _msilHelper.StackPushFromCILStack<int>(_gen, _stack, _tmp1);
                    break;

                case Bytecode.IMult:
                    _msilHelper.StackPop<int>(_gen, _stack, _tmp1);
                    _gen.Emit(OpCodes.Stloc, _tmp1);

                    _msilHelper.StackPop<int>(_gen, _stack, _tmp2);
                    _gen.Emit(OpCodes.Stloc, _tmp2);

                    _gen.Emit(OpCodes.Ldloc, _tmp2);
                    _gen.Emit(OpCodes.Ldloc, _tmp1);

                    _gen.Emit(OpCodes.Mul);
                    _msilHelper.StackPushFromCILStack<int>(_gen, _stack, _tmp1);
                    break;

                case Bytecode.GStore:
                    if (ins.Parameters[0].Type != ParameterType.Integer)
                    {
                        _errors.Add(new Error(String.Format("Parameter type is not an integer on line {0}", ins.LineNumber)));
                        return;
                    }

                    _gen.Emit(OpCodes.Ldloc, _globals);
                    _gen.Emit(OpCodes.Ldc_I4, ins.Parameters[0].IntegerValue);
                    _msilHelper.StackPop<int>(_gen, _stack, _tmp1);
                    _gen.Emit(OpCodes.Stelem_I4);
                    break;

                case Bytecode.GLoad:
                    if (ins.Parameters[0].Type != ParameterType.Integer)
                    {
                        _errors.Add(new Error(String.Format("Parameter type is not an integer on line {0}", ins.LineNumber)));
                        return;
                    }

                    _gen.Emit(OpCodes.Ldloc, _globals);
                    _gen.Emit(OpCodes.Ldc_I4, ins.Parameters[0].IntegerValue);
                    _gen.Emit(OpCodes.Ldelem_I4);
                    _msilHelper.StackPushFromCILStack<int>(_gen, _stack, _tmp1);
                    break;

                case Bytecode.Jump:
                    {
                        var labelName = ins.Parameters[0].StringValue;
                        var labelSymbol = _symbolTable.Lookup(SymbolType.Label, labelName);
                        if (labelSymbol == null)
                        {
                            _errors.Add(new Error(String.Format("Failed to find symbol label: {0} defined at line: {1}", labelName, ins.LineNumber)));
                            return;
                        }

                        var msilSymbol = labelSymbol as MSILSymbol;
                        _gen.Emit(OpCodes.Br, msilSymbol.Label);
                    }
                    break;

                case Bytecode.JumpTrue:
                    {
                        var labelName = ins.Parameters[0].StringValue;
                        var labelSymbol = _symbolTable.Lookup(SymbolType.Label, labelName);
                        if (labelSymbol == null)
                        {
                            _errors.Add(new Error(String.Format("Failed to find symbol label: {0} defined at line: {1}", labelName, ins.LineNumber)));
                            return;
                        }

                        var msilSymbol = labelSymbol as MSILSymbol;
                        _msilHelper.StackPop<int>(_gen, _stack, _tmp1);
                        _gen.Emit(OpCodes.Brtrue, msilSymbol.Label);
                    }
                    break;

                case Bytecode.JumpFalse:
                    {
                        var labelName = ins.Parameters[0].StringValue;
                        var labelSymbol = _symbolTable.Lookup(SymbolType.Label, labelName);
                        if (labelSymbol == null)
                        {
                            _errors.Add(new Error(String.Format("Failed to find symbol label: {0} defined at line: {1}", labelName, ins.Parameters[0].LineNumber)));
                            return;
                        }

                        var msilSymbol = labelSymbol as MSILSymbol;
                        _msilHelper.StackPop<int>(_gen, _stack, _tmp1);
                        _gen.Emit(OpCodes.Brfalse, msilSymbol.Label);
                    }
                    break;

                case Bytecode.IEqual:
                    _msilHelper.StackPop<int>(_gen, _stack, _tmp1);
                    _gen.Emit(OpCodes.Stloc, _tmp1);

                    _msilHelper.StackPop<int>(_gen, _stack, _tmp2);
                    _gen.Emit(OpCodes.Stloc, _tmp2);

                    _gen.Emit(OpCodes.Ldloc, _tmp2);
                    _gen.Emit(OpCodes.Ldloc, _tmp1);

                    _gen.Emit(OpCodes.Ceq);
                    _msilHelper.StackPushFromCILStack<int>(_gen, _stack, _tmp1);
                    break;

                case Bytecode.ILessThan:
                    _msilHelper.StackPop<int>(_gen, _stack, _tmp1);
                    _gen.Emit(OpCodes.Stloc, _tmp1);

                    _msilHelper.StackPop<int>(_gen, _stack, _tmp2);
                    _gen.Emit(OpCodes.Stloc, _tmp2);

                    _gen.Emit(OpCodes.Ldloc, _tmp2);
                    _gen.Emit(OpCodes.Ldloc, _tmp1);

                    _gen.Emit(OpCodes.Clt);
                    _msilHelper.StackPushFromCILStack<int>(_gen, _stack, _tmp1);
                    break;

                case Bytecode.ISub:
                    _msilHelper.StackPop<int>(_gen, _stack, _tmp1);
                    _gen.Emit(OpCodes.Stloc, _tmp1);

                    _msilHelper.StackPop<int>(_gen, _stack, _tmp2);
                    _gen.Emit(OpCodes.Stloc, _tmp2);

                    _gen.Emit(OpCodes.Ldloc, _tmp2);
                    _gen.Emit(OpCodes.Ldloc, _tmp1);

                    _gen.Emit(OpCodes.Sub);
                    _msilHelper.StackPushFromCILStack<int>(_gen, _stack, _tmp1);
                    break;

                case Bytecode.IDiv:
                    _msilHelper.StackPop<int>(_gen, _stack, _tmp1);
                    _gen.Emit(OpCodes.Stloc, _tmp1);

                    _msilHelper.StackPop<int>(_gen, _stack, _tmp2);
                    _gen.Emit(OpCodes.Stloc, _tmp2);

                    _gen.Emit(OpCodes.Ldloc, _tmp2);
                    _gen.Emit(OpCodes.Ldloc, _tmp1);

                    _gen.Emit(OpCodes.Div);
                    _msilHelper.StackPushFromCILStack<int>(_gen, _stack, _tmp1);
                    break;

                case Bytecode.Print:
                    _msilHelper.StackPop<int>(_gen, _stack, _tmp1);
                    _gen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[]{typeof(int)}));
                    break;

                case Bytecode.PrintChar:
                    _msilHelper.StackPop<int>(_gen, _stack, _tmp1);
                    _gen.Emit(OpCodes.Call, typeof(Console).GetMethod("Write", new Type[]{typeof(char)}));
                    break;

                case Bytecode.Call:
                    {
                        int ilOffset = _gen.ILOffset;
                        Label returnAddress = getLabel(ilOffset);

                        var numArgs = ins.Parameters[0].IntegerValue;
                        var labelName = ins.Parameters[1].StringValue;
                        var labelSymbol = _symbolTable.Lookup(SymbolType.Label, labelName);
                        if (labelSymbol == null)
                        {
                            _errors.Add(new Error(String.Format("Failed to find symbol label: {0} defined at line: {1}", labelName, ins.LineNumber)));
                            return;
                        }

                        _gen.Emit(OpCodes.Ldloc, _bp);
                        _msilHelper.StackPushFromCILStack<int>(_gen, _stack, _tmp1);
                        _msilHelper.StackPush<int>(_gen, _stack, numArgs);
                        _msilHelper.StackPush<int>(_gen, _stack, ilOffset);

                        _msilHelper.StackSize<int>(_gen, _stack);
                        _gen.Emit(OpCodes.Stloc, _bp);

                        var msilSymbol = labelSymbol as MSILSymbol;
                        _gen.Emit(OpCodes.Br, msilSymbol.Label);

                        storeLabel(returnAddress, ilOffset);
                    }
                    break;

                case Bytecode.Halt:
                    _gen.Emit(OpCodes.Ret);
                    break;
            }

            return;
        }

        private Label getLabel(int offset)
        {
            Console.WriteLine("GetLabel: {0}", offset);
            if (_labels.ContainsKey(offset))
            {
                return _labels[offset];
            }

            return new Label();
        }

        private void storeLabel(Label label, int offset)
        {
            if (_labels.ContainsKey(offset))
            {
                return;
            }

            Console.WriteLine("Mark label for {0} ({1})", offset, _gen.ILOffset);
            _gen.MarkLabel(label);
            Console.WriteLine("Marked label: {0}", label);
            _labels[offset] = label;
        }

        public List<IError> GetErrors()
        {
            return _errors;
        }
    }
}


