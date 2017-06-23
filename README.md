# A simple stack-based Virtual Machine 

The project is writtein in C# (Mono) and contains:

* An interpreter (with a tracer and a simple singlestep debugger)
* A compiler which allows to create standalone .NET assembly executables
* A formatter, which does (re)format the source

## Requirements

- Mono 4.8.0 or higher
- make (in order to build it, or you can do it manually as well)

## Structure

- __src/__: contains all the source code
- __Tools/__: contains all the Tools (Interpreter, Compiler, Formatter)
- __examples/__: contains example instruction files

## How to compile

In order to compile the source and all the tools simply execute:

	$ make

The compiled tools can be found in the directory: __Bin/__.

	(...snip...)
	├── Fmt.exe
	├── Interpreter.exe
	├── MSIL.exe
	└── VM.dll
	(Note: It also has *.dll's from third party)

## Demo

### Interpreter - Loop example

	$ mono Bin/Interpreter.exe -s examples/loopexample.vm
	10
	9
	8
	7
	6
	5
	4
	3
	2
	1

The according VM file:

	# Sample Instruction
	# Loop: Prints numbers starting from 10 to 1
	  PUSH 10                # Start at number 10
	  GSTORE 0               # Store it at the global memory at position 0
	
	loop:                    # loop is a label
	  GLOAD 0                # Load the value stored at position 0 onto the stack
	  PRINT                  # Print the number which is on the stack
	
	  GLOAD 0                # Load the counter number from memory, pos 0
	  PUSH 1                 # Push 1
	  ISUB                   # Decrement the number which is onto the stack by 1
	  GSTORE 0               # Store the result on the global memory at position 0
	  GLOAD 0                # Load the number again from the global memory
	
	  PUSH 0                 # Push 0 to see if we reached 0 already
	  IEQUAL                 # check if 0 == (value onto the stack; counter)
	  JUMPFALSE @loop        # if false, then we jump to the label 'loop'
	  HALT                   # Exit


Note: The interpreter's global data (GSTORE) can store an 'infinite' amount of entries. It's not limited since it uses a C# List underneath.

### Interpreter - Factorial example

	$ mono Bin/Interpreter.exe -s examples/fact.vm
	1
	2
	6
	24
	120
	720
	5040
	40320
	362880

The according VM file:

	# Factorials
	# Prints the factorials from 1 up to 10
	
	  PUSH 1
	  GSTORE 0
	
	loop:
	  GLOAD 0
	  CALL 1 @fact
	  PRINT
	
	  GLOAD 0
	  PUSH 1
	  IADD
	  GSTORE 0
	
	  GLOAD 0
	  PUSH 10
	  ILESSTHAN
	  JUMPTRUE @loop
	  HALT
	
	fact:
	  LOAD 0
	  PUSH 2
	  ILESSTHAN
	  JUMPFALSE @isBigger
	  PUSH 1
	  RET
	
	isBigger:
	  LOAD 0
	  LOAD 0
	  PUSH 1
	  ISUB
	  CALL 1 @fact
	  IMULT
	  RET


### .NET Compiler

Now let's compile the above example into a standalone .NET executable:

	$ mono Bin/MSIL.exe -s examples/loopexample.vm -o loopexample.exe
	$ mono loopexample.exe
	10
	9
	8
	7
	6
	5
	4
	3
	2
	1

Note: The compiler's global data (GSTORE) can store only 200 elements at the moment. It uses a pre-allocated array underneath. At the moment there is no optimization implemented to check how much data has been stored.

The generated MSIL code looks similiar to the initial VM file:

	IL_000a:  stloc.0
	IL_000b:  ldc.i4 10
	IL_0010:  stloc.1
	IL_0011:  ldloc.0
	IL_0012:  ldc.i4 0
	IL_0017:  ldloc.1
	IL_0018:  stelem.i4
	IL_0019:  ldloc.0
	IL_001a:  ldc.i4 0
	IL_001f:  ldelem.i4
	IL_0020:  call void class [mscorlib]System.Console::WriteLine(int32)
	IL_0025:  ldloc.0
	IL_0026:  ldc.i4 0
	IL_002b:  ldelem.i4
	IL_002c:  ldc.i4 1
	IL_0031:  sub
	IL_0032:  stloc.1
	IL_0033:  ldloc.0
	IL_0034:  ldc.i4 0
	IL_0039:  ldloc.1
	IL_003a:  stelem.i4
	IL_003b:  ldloc.0
	IL_003c:  ldc.i4 0
	IL_0041:  ldelem.i4
	IL_0042:  ldc.i4 0
	IL_0047:  ceq
	IL_0049:  brfalse IL_0019
	IL_004e:  ret

***IMPORTANT:*** The MSIL Compiler does not support the 'call' instruction because I have not implemented it yet. Please use the interpreter if you want to play with call's :-).

### Formatter

The Formatter is a great way to (re)format your written VM files, as you can see here:

	$ cat examples/badly-formatted.vm
	# Sample Instruction
	    # TestData Entry
	push        3
	push 10 # A comment
	
	iadd gstore 0
	  gload 0
	    push    10 #push 10 onto stack
	isub
	
	
	      print
	halt

Now let's run the __Bin/Fmt.exe__ tool:

	$ mono Bin/Fmt.exe -s examples/badly-formatted.vm
	# Sample Instruction
	# TestData Entry
	  PUSH 3
	  PUSH 10                # A comment
	
	  IADD
	  GSTORE 0
	  GLOAD 0
	  PUSH 10                #push 10 onto stack
	  ISUB
	
	
	  PRINT
	  HALT


### Error handling

Note: The 'pos' always refers to the position counted from the very beginning of the file.

__Recognizion of invalid input:__

	Got type Error (Invalid input '้' at pos: 67) on line 3 but expected either an instruction or a comment
	
__Simple type & instruction checking:__

	Given line: 'push ab' (ab is not allowed, only integers)

	Error on line 2: Instruction 'PUSH' has wrong parameter count. Expected 1, got 0
	Error on line 2: Opcode for instruction 'ab' not found.

## TODO
- Writing unit tests
- Adding more instructions
- Rename a few instructions (ie. JUMPFALSE, JUMPTRUE, etc.)
- .NET Compiler: Implement 'call' 
- Add stack index bound checks (currently it throws an exception which is not really nice)

## Motivation

I was mainly coding this VM in order to learn more about the C# programming language and also in order to learn (more) about stack based VM and what's necessary to lex & parse source code. It also has helped me to understand MSIL better.

I wanted to start with the simplest form of parsing and therefore the idea of creating a stack-based VM incl. parser was born.

Also I think using this stack based VM is a great way to learn more about how VM's work.

## Development status

It is a hobby project so I don't know whether I will add more features or not, but feel free to fork it and continue (and maybe send Pull-Requests :-)).

## License

*The entire c# code is licensed under MIT License:*

Copyright (c) 2017 Andreas Näpflin

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
