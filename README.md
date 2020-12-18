# csharpextensions-vim
A collection of useful extension methods for C# files in Vim/NeoVim

## Installation

The only currently supported installation method is `vim-plug` since this plugin uses a post-install script
to build the included dotnet code.

```vim
Plug 'jpfeiffer16/csharpextensions-vim', { 'do':  'dotnet build ./tools/tools.sln' }
```

## Commands

* `csharpextensions#GenerateClass()`
    - Generates a skeleton class in the current buffer.

* `csharpextensions#GetResharperDiagnostics()`
    - Runs the Resharper command line tool, parses the results and populates the quick-fix list with the found diagnostics.
       - This command requires the jb.exe global tool to be installed. You can get it with `dotnet tool install -g JetBrains.ReSharper.GlobalTools`
