# ZGen Code Generator

This is a simple template source generator for dotnet that is installed as a CLI dotnet tool.

The templates can be part of a source tree or be kept in a directory outside the source.
The templates are stored in a directory called `.zgentemplates`.

The result of the chosen template is generated into the current directory.

At the moment, only the z code generator is available. It is a simple template code generator that replaces any occurence of a variable with its value.
The variables are called z&lt;NUMBER&gt; and can occur in directory names or files names.
In a text file, it must be surrounded by `{{` and `}}`. Like this: `{{z1}}`.

It is a template generator that can store templates somewhere in the directory tree under the current directory of the CLI.


## Installation

This tool is distributed through nuget.org, so it can be installed as a ususal dotnet cli tool.
```

dotnet tool install -g zgencodegenerator

```
## Update
```
dotnet tool update -g zgencodegenerator
```
## Uninstall
```
dotnet tool uninstall -g zgencodegenerator
```
## Usage in command line

In windows command line, the tool can be invoked as `zgen` or `zgen.exe`.
Otherwise it should be possible to access it like this:
```
 dotnet tool run zgen --help
```

For convenciene you can add an alias to your shell profile if needed

```
alias zgen="dotnet tool run zgen"
```
Or on windows you can add a zgen.bat in your path with the following content if for some reason it will not
already be available already in your command line path as zgen.

```
dotnet tool run zgen
```

## Currently available source generators provided:
- z : This uses z&lt;NUMBER&gt; variables in directory name, file name and file content.

## License

Since this is a dotnet tool and will never be part of your application, the license has ZERO effect on what project it is used on.
Use it on any project with any license.

[LGPL 2.1](https://www.gnu.org/licenses/old-licenses/lgpl-2.1.en.html)