# ZGen Code Generator


This is a source generator for dotnet. A dotnet tool.

The templates can be part of a source tree or be kept in a directory outside the source.


It is a template generator that can store templates somewhere in the directory tree under the current directory of the CLI.



## Installation


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
That can also be placed in a script file in your path.

## Features


- Live previews

- Add section

- Remove Section

- Update Section

- Download Readme

## License


[LGPL 2.1](https://www.gnu.org/licenses/old-licenses/lgpl-2.1.en.html)