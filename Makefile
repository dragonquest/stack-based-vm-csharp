SRCS_VM = $(shell find ./src/ -name '*.cs' -and -not -path './Tool/*' -and -not -name '*_test.cs')
SRCS_TOOLS_THIRD_PARTY = $(shell find ./Tools/ThirdParty/ -name '*.cs'  -and -not -name '*_test.cs')

all: tools

thirdparty:
	cp Bin/ThirdParty/CommandLine.dll Bin/CommandLine.dll

lib:
	mcs /out:Bin/VM.dll -target:library $(SRCS_VM)

tools: interpreter dotnetcompiler fmt

interpreter: thirdparty lib
	mcs -debug /out:Bin/Interpreter.exe Tools/Interpreter.cs Tools/Interpreter.Options.cs Tools/Interpreter.BytecodeCompiler.cs $(SRCS_TOOLS_THIRD_PARTY) -r:Bin/VM.dll -r:Bin/CommandLine.dll

dotnetcompiler: thirdparty lib
	mcs -debug /out:Bin/MSIL.exe Tools/MSIL.cs Tools/MSIL.Options.cs Tools/MSIL.Compiler.cs -r:Bin/VM.dll $(SRCS_TOOLS_THIRD_PARTY) -r:Bin/CommandLine.dll

fmt: thirdparty lib
	mcs -debug /out:Bin/Fmt.exe Tools/Fmt.cs Tools/Fmt.Options.cs Tools/Fmt.Formatter.cs -r:Bin/VM.dll $(SRCS_TOOLS_THIRD_PARTY) -r:Bin/CommandLine.dll

clean:
	-rm Bin/CommandLine.dll
	-rm Bin/VM.dll
	-rm Bin/Interpreter.exe
	-rm Bin/Interpreter.exe.mdb
	-rm Bin/MSIL.exe
	-rm Bin/MSIL.exe.mdb
	-rm Bin/Fmt.exe
	-rm Bin/Fmt.exe.mdb

.PHONY: clean thirdparty tools
