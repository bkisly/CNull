# C? - language documentation

## General information

**C?** (read as *C-null*) is an **interpreted, procedural high-level language**, which syntax is similar to C, C++ or C# languages. The main feature of the C? language is **default nullability of all data types** (including primitive types) and **null-access control**.

Goals of the C? language:
- connecting **the ability for quick coding in scripting style** known from Python or JavaScript, along with clarity and consistency of the syntax and easy-to-read naming convetions known from the C# language, what makes C programmer-friendly,
- **language built-in information about the success of executed operations** in the program, known from the Rust language.
### General set of functional requirements

- **Interpreted** language.
- **Procedural** paradigm.
- **Static and weak typing**. All casting operations in C? are executed implicitly (if they are semantically correct). There are different types of implicit conversions.
- **Essential mathematical operations** on numeric types (addition, subtraction, multiplication, division, modulo, prioritizing using parenthesis).
- **String concatenation** and **access to particular characters** by index.
- Ability to create **custom, programmer-defined functions**, which can contain local variables.
- **`if`-`else` statements, `while` loop.**
- **Single-line comments** available.
- **Default acceptance of `null` value for all data types**, including primitive types.
- **Built-in data collection** (dictionary), as the example of a complex type, having its own interface for adding, retrieving, setting and deleting elements. The key in this dictionary can never be a `null` value,
- Errors signalized during improper null access.
- **Exception handling in `try`-`catch` blocks**, known from other programming languages. All exceptions in C? are strings. Possible exception filtering in `catch` blocks.
- **Possibility to create multi-file programs** and **import selected functions from other modules**, similarly to the Python language.
- Available functions for interacting with standard input and output, closed in the **standard library**.

### Non-functional requirements

- **Multi-platform** - interpreter for the C? language is available for Windows, macOS and Linux operating systems.
- **Reliability of the interpreter** - every non-standard situation is handled properly by the interpreter, thus it is impossible for it to terminate the execution unexpectedly.
- **Programmer-friendly** - syntax and naming conventions defined by the C? language support the clarity of the code and being easy-to-read.
- **Ability for quick coding** - a little amount of preparatory code is required for C? programs to start working.
- **Security** - being high-level and featuring data access control and reliability of the interpreter, C? is a secure language.

### Executing the programs and interacting with the interpreter

Interpreter of the C? language provides the CLI, enabling the user to execute their programs from the terminal. Launching the programs is similar to Python:

```bash
cnull [main-file-path]
```

Conventionally, the extension of C? source files is `.cnull`. It is also possible to launch C? programs in a more simple way, if we work with a multi-module program. If a file `Program.cnull` exists in the working directory, the program can be launched using the following command:

```bash
cnull
```

If our working directory is different than the project directory, we can also used the command:

```bash
cnull [path-to-the-directory]
```

This will result in executing the program from the `Program.cnull` file from the given directory, or notifying about the error in case of failure.

It is possible to pass command-line arguments using `--args` parameter:

```shell
cnull --args first second third
```

**Other commands**:
- `cnull --help` - displays the instructions.
- `cnull --version` - displays the current version of the interpreter.

## Language specification

### Language elements

#### Literals, identifiers and operators

1. **Constants**
	- **numeric constants** - interpreted in decimal format. Zero-prefixed numeric values are not allowed.
		- integer - `0, -250, 12345`
		- floating-point - `0.24`, `-1.4567`, `1234.5678`, `0.290`. 
	- **string literals** - embraced by double quotes - `"sample text"`, `"lorem ipsum"`, `""`
	- **char literals** - embraced by single quotes. Can contain contain only a single character, cannot be empty - `'a'`, `'A'`, `'h'`
	- **boolean literals** - `true`, `false`
	- **null value literal** - `null`
2. **Identifiers** - can contain only letters, underscore characters and digits (which cannot be the prefix of the identifier). Identifiers building rules remain the same for variables and functions. Maximum identifier length is 256 characters - `myVariable`, `MYVARIABLE`, `My_Variable`, `_myVariable1`, `____`. 
3. **Operators**
	- **arithmetical** - `+`, `-`, `*`, `/`, `%`
	- **prioritizing** - `(`, `)`
	- **assignment** - `=`
	- **relational** - `<`, `<=`, `>`, `>=`, `==`, `!=`
	- **boolean** - `&&`, `||`, `!`, `?` (null-checking)
	- **member access** - `.`
4. **Setting code block boundaries** - `{`, `}`
5. **Comments** - `// sample single-line comment`
### Data types

C? is **statically and weakly typed**. By default, **every data type accepts the `null` value**.  The C? languages, similarly to C#, separates the types into 2 essential groups:
- **value types** - all primitive types (including `string` type). They are **immutable**, **copied during assignment** and **passed by value** as an argument.
- **reference types** - all complex types. They are **mutable**, **the reference is copied during assignment**, they are **passed by reference** as an argument.

#### Value types

1. **`bool`** - boolean type
2. **`int`** - integer type (32-bit signed integer number)
3. **`float`** - single precision floating-point number
4. **`string`** - characters string
5. **`char`** - single character (unsigned byte)

#### Exceptions

Exceptions in C? are in fact strings, which can be thrown and handled the same way as regular exceptions. There is a defined set of several standard exceptions, which are thrown e.x. during an invalid cast or invalid null access.

### Data in the program

- **Weak and static** typing
- Arguments are passed to the function accordingly to their type group:
	- primitive types - **by value**
	- complex types - **by reference**
- **Type conversions**:
	- performed implicitly, since the assignment operator can simultaneously perform casting if needed. For instance, when trying to assign a value of type `float` to a variable of type `int`, it will be automatically converted to the target type. In case of failure, an exception will be thrown.
	- there are also more non-typical type conversions related to strings:
		- comparing an integer value to a string value results in comparing the length of that string to that number
		- assigning any primitive value (excluding `null`) to a variable of type `string` will result in converting that value to string
- **The scope of a variable** is limited to the block in which it is defined, including all nested blocks
- **Variables hiding** can occur during redefining a variable with the same name in an inner block. In this case, the most nested variable will be processed.
- **Importing functions** from external modules is done by specifying the name of a module and the name of a function. Standard library functions are additionally grouped in submodules (so that they are accessed as `CNull.Submodule.Function`).

### Error handling

Errors are handled by a separate module of the project. All occurrance of an error in a specific module of the interpreter (e.x. lexer, parser or the interpreter itself) will result in signalizing an error on behalf of error handler component. Having the information regarding the specific error, the error handler performs proper actions (such as sending the output to standard error stream or saving information to logs).

Handled types of errors:
1. **Source access errors** - raised during detected issues when accessing source files (such as trying to access a non-existing file)
2. **Compilation errors** - errors related to the static analysis of the code (all syntax or lexical errors)
3. **Semantic errors** - raised during detecting semantically incorrect code (e.x. invalid typing)
4. **Runtime errors** - exceptions raised during program interpretation, must be handled by the user (e.x. invalid null access).

Errors messages are formed the following way:
- for compilation or semantic errors:

```bash
C? error: [error type]
Source: [file or module name] (line [line number], column [column number])
```

- for unhandled exceptions - displaying stack trace:

```
C? unhandled exception ([exception name]):
	at [module and function name, in which the exception was thrown] (line [line number])
```

### Examples

#### Essentials - variables, imports, operations on primitive types and interaction with the user

##### Valid cases

**1. Calculating the sum of numbers given by the user**

```csharp
import CNull.Console.Write;
import CNull.Console.WriteLine;
import CNull.Convert.StringToInt;

void Main()
{
	Write("Input the first number: ");
	int first = StringToInt(ReadLine());

	Write("Input the second number: ");
	int second = StringToInt(ReadLine());
	
	int result = first + second;
	WriteLine(result);
}
```

Result:
```
Input the first number: 30
Input the second number: 20
50
```

**2. Complex arithmetical operation with implicit casts**

```csharp
import CNull.Console.WriteLine;

void Main()
{
	int a = 23;
	float b = 14.2;
	
	int c = (b + a) * (2 - b) / 2;
	WriteLine(c);
}
```

Result:
```
-222
```

**3. Implicit conversions to `string`**

```csharp
import CNull.Console.WriteLine;

void Main()
{
	int a = 23;
	string b = 14;
	
	string c = a + b;
	WriteLine(c);
}
```

Result:
```
2314
```

**4. Variable declarations with optional initializations and primitive types demonstrations**

```csharp
void Main()
{
	int a;
	a = 2000;
	a = -2000;
	
	char c = 'A';
	c = '';
	string d = "Sample text";
	d = "";
	
	bool e = true;
	e = false;
	
	float f = 2.1234355;
	f = -2.424244;
	f = 123123.123123;
	f = 0.123123;
	f = 0.12312313;
	
	a = null;
	b = null;
	c = null;
	d = null;
	e = null;
	f = null;
}
```
 
**5. Examples of valid identifiers**

```csharp
void Main()
{
	int someVariable;
	int _someVariable;
	int SOME_VARIABLE;
	int _;
	int ________;
	int someVariable1;
	int some_variable_1;
	int _12345;
}
```

**6. Boolean expressions**

```csharp
import CNull.Console.WriteLine;

bool a = true;
bool b = false;
bool c = null;
bool d = !(a || b) && a?;

WriteLine(a?);
WriteLine(c?);
WriteLine(d);
```

Result:
```
True
False
false
```

##### Invalid cases

**1. `import` directives not defined at the beginning of the file**

```csharp
bool a = true;
import CNull.Console.WriteLine;
WriteLine(a);
```

Result:
```
C? error on line 2, column 1: Import statement must be placed at the top of the file.
```

**2. Using the keyword as an identifier**

```csharp
int bool = 2;
```

Result:
```
C? error on line 1, column 5: Keywords cannot be used as identifiers.
```

**3. Invallid `null` access**

```csharp
void Main()
{
	int a = 1;
	int b;
	
	int c = a + b;
}
```

Result:
```
C? unhandled exception (NullValueException):
	at Program.Main (line: 6)
	at Program.<entry point>
```

#### Complex examples - functions, control flow i and exceptions handling

**1. Recursive factorial function containing comments and returning `null` in case of invalid input. Example featuring stack overflow.**

```csharp
import CNull.Console.WriteLine;

int Factorial(int n)
{
	if (n?) // Check if the given value is null. Return null if yes.
	{
		return null;
	}

	if (n == 0 || n == 1)
	{
		return 1;
	}
	else
	{
		return n * Factorial(n - 1);
	}
}

void Main()
{
	WriteLine(Factorial(0));
	WriteLine(Factorial(1));
	WriteLine(Factorial(5));
	WriteLine(Factorial(null)?);
	WriteLine(Factorial(1000000000));
}
```

Result:
```
1
1
120
True
C? unhandled exception (Stack overflow):
	at Program.Factorial (line 16)
	at Program.Factorial (line 16)
	...
```

**2. Passing primitive types by value**

```csharp
import CNull.Console.WriteLine;

void Process(int value)
{
	while (value < 50)
	{
		value = value + 1;
	}

	WriteLine(value);
}

void Main()
{
	int a = 20;
	Process(a);
	WriteLine(a);
}
```

Result:
```
50
20
```

**3. Operations on a dictionary**

```csharp
import CNull.Console.WriteLine;

void Main()
{
	dict<int, bool> d;
	
	d.Add(1, true);
	d.Add(2, false);
	
	WriteLine(d.Get(1));
	WriteLine(d.Get(3));
	WriteLine(d.Get(200)?);
	
	d.Add(null, null);
}
```

Result:
```
True
False
True
C? unhandled exception (NullValueException):
	at Program.Main (line 15)
```

**4. Multiple, nested function calls along with passing data by value**

```csharp
import CNull.Console.WriteLine;

int A(int val)
{
	if (val < 10)
	{
		return 5;
	}

	return val + 40;
}

int B(int val)
{
	int counter = 0;
	while (counter < 5)
	{
		val = val + 1;

		if (val > 50)
		{
			return val;
		}

		counter = counter + 1;
	}

	return A(val);
}

int C(int val)
{
	if (val > 20)
	{
		return A(val);
	}

	return B(val);
}

void Main()
{
	int a = 20;
	WriteLine(C(100));
	WriteLine(C(10));
	WriteLine(C(a));
	WriteLine(a);
}
```

Result:
```
140
55
65
20
```

### EBNF grammar specification

#### Lexical layer

Grammar specifiaction on the lexical layer is included in the [lexical_grammar.ebnf](docs/lexical_grammar.ebnf) file.

#### Syntactic layer

Grammar specification on the syntactic layer is included in the [syntactic_grammar.ebnf](docs/syntactic_grammar.ebnf) file.

## Implementation

### Project structure

Project C? is implemented as a modular, C# application. Each module is either a separate C# class library or a console application as the front-end layer. The whole core layer of the interpreter for this language is highly separated from the presentation layer and used standard input and output, which makes it a distributable and reusable library for different front-ends.

Main modules of the project:
- **`CNull.Source`** - class library performing the access to the source of the code and passing it to the lexer in a unified form, convenient for performing lexical analysis. Main elements of this library are the following:
	- interface for getting characters from the source to be used by lexer,
	- class performing the access to the source using streams,
	- helpers for source access layer,
	- source access errors definitions.
- **`CNull.Lexer`** - class library performing lexical analysis, including tokenization of the source code and exposing the tokens to the parser in a convenient form to perform syntactic analysis. Main elements of this library are the following:
	- lexical analyser (including its interface exposed to the parser)
	- generic implementation of a token,
	- enums and maps for token types,
	- lexical errors definitions,
	- helpers for the lexer.
- **`CNull.Parser`** - class library performing syntactic analysis and building abstract syntax tree of the code and exposing it to the semantic analyser. Main elements of this library are the following:
	- syntactic analyser (including its interface exposed to the semantic analyser),
	- abstractions for representing syntax tree,
	- implementation of the comments-filtering proxy,
	- syntax errors definitions,
	- helpers for the parser.
- **`CNull.Semantics`** - (*for future implementation*) class library responsible for performing syntactic analysis of the code. Main elements of this library are the following:
	- semantic analyser (including its interface exposed to the interpreter),
	- semantic errors definitions,
	- helpers for the semantic analyser.
- **`CNull.Interpreter`** - class library containing the implementation of the interpreter - this is the place in which C? programs are actually executed. Main elements of this library are the following:
	- interpreter (including its interface exposed to the core library facade),
	- standard library,
	- types resolver,
	- classes handling call contexts and helper actions for the interpreter,
	- facade exposing the simplified interface for client applications and configuring all components.
- **`CNull`** - console application handling interaction with the user and their commands. Its only responsibility is exposing the CLI and delegating program execution to the referenced core library.

Additional modules:
- **`CNull.ErrorHandler`** -  module responsible for handling the errors raised by all core modules. It performs all tasks related to error handling (including serializing information about errors to logs) and passing the information to the front-end layer.
- **`CNull.Common`** - module containing common elements for all project layers.

Test projects:
- **`CNull.*.Tests`** - unit tests, where `*`is the name of the tested module, e.x. `CNull.Lexer.Tests`.
- **`CNull.IntegrationTests`** - integration tests.

Communication between core modules is limited to the "adjacent" ones, so that a module can access only the one which is logically directly under it (i.e. lexer can only reference source, parser can only reference lexer and so on). Each of the core modules can access the error handler. All modules can access the common module.

To simplify testing and objects management, main components operate only on the interfaces of their dependencies, which instantiation is managed by DI framework.

It is worth mentioning that the interpreter is not directly connected to console's standard input and output. Instead, it gets callbacks, which operate on corresponding streams. This way, presentation layer for the interpreter can be implemented not only as a CLI.

### Testing

As in every complex project, unit and integration tests can be featured:
- **unit tests** strictly separate all tested components from each other, to isolate each test case and ensure their atomicy. Particuarly, data source is mocked and input data is passed as strings. *Moq* framework is used for mocking.
- **integration tests** vallidate the behavior of several components connected together.