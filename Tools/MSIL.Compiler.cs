using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Collections.Generic;

using VM;
using VM.Parsing;
using VM.Parsing.AST;

public partial class MSIL
{
    private class Compiler
    {
        public Compiler() {}

        public void Compile(string inputFile, string outputFile)
        {
            string source = File.ReadAllText(inputFile);

            VM.Parsing.AST.Tree tree;
            var errors = VM.Parser.TryParse(source, out tree);
            if (errors.Count > 0)
            {
                foreach (var err in errors)
                {
                    Console.Error.WriteLine(err);
                }
                return;
            }

            var assemblyBuilder = createAssembly("Application");
            var typeBuilder = createModuleType(assemblyBuilder, outputFile);
            var methodBuilder = createMethod(typeBuilder, "Main");
            ILGenerator gen = methodBuilder.GetILGenerator();

            var treeWalker = new Walker();

            var symbolCollector = new VM.CodeGeneration.MSIL.SymbolCollector(gen);

            treeWalker.Walk(symbolCollector, tree);
            var symbolTable = symbolCollector.GetSymbolTable();

            var compiler = new VM.CodeGeneration.MSIL.Compiler(gen, symbolTable);
            treeWalker.Walk(compiler, tree);

            if (compiler.GetErrors().Count > 0)
            {
                foreach (var err in compiler.GetErrors())
                {
                    Console.Error.WriteLine(err);
                }
                return;
            }

            typeBuilder.CreateType();
            assemblyBuilder.SetEntryPoint(methodBuilder, PEFileKinds.ConsoleApplication);
            assemblyBuilder.Save(outputFile);

            //return compiler.GetILGenerator();
        }

        private AssemblyBuilder createAssembly(string name)
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = name;
            AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);

            return assemblyBuilder;
        }

        private TypeBuilder createModuleType(AssemblyBuilder builder, string filename)
        {
            ModuleBuilder module = builder.DefineDynamicModule(filename);

            return module.DefineType("MainType", TypeAttributes.Public | TypeAttributes.Class);
        }

        private MethodBuilder createMethod(TypeBuilder builder, string name)
        {
            return builder.DefineMethod(name, MethodAttributes.HideBySig | MethodAttributes.Static | MethodAttributes.Public, typeof(void), new Type[] { typeof(string[]) });
        }
    }
}
