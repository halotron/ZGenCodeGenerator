# ZGen Code Generator


This is a source generator for dotnet. A dotnet tool.

The templates can be part of a source tree or be kept in a directory outside the source.


It is a template generator that can store templates somewhere in the directory tree under the current directory of the CLI.



## Installation

This tool is distributed through nuget.org, so it can be installed as a ususal dotnet cli tool.
```

dotnet tool install -g zgen

```
## Usage in CLI or command line

```
 dotnet tool run zgen --help
```

For convenciene you can add an alias to your shell profile

```
alias zgen="dotnet tool run zgen"
```
Or on windows you can add a zgen.bat in your path with the following content

```
dotnet tool run zgen
```

## Currently available source generators provided:
- z : This uses z<NUMBER> in directory name, file name and file content.

## License

Since this is a dotnet tool and will never be part of your application, the license has ZERO effect on what project it is used on.
Use it on any project with any license.

[LGPL 2.1](https://www.gnu.org/licenses/old-licenses/lgpl-2.1.en.html)