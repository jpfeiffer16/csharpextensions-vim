# csharpextensions-vim
A collection of useful extension methods for C# files in Vim/NeoVim

## Installation

The only currently supported installation method is `vim-plug` since this plugin uses a post-install script
to build the included Dotnet code.

```vim
Plug 'jpfeiffer16/csharpextensions-vim', { 'do':  'dotnet build ./tools/tools.sln' }
```

## Commands

* `CSECreateClass`
    - Generates a skeleton class in the current buffer.

* `CSEGetResharperDiagnostics`
    - Runs the Resharper command line tool, parses the results and populates the quick-fix list with the found diagnostics.
       - This command requires the jb.exe global tool to be installed. You can get it with `dotnet tool install -g JetBrains.ReSharper.GlobalTools`

* `CSEScript`
    - Opens a new tab and starts a minimal Linqpad-like session.
    - It will drop you in a new csx file so you can play around with a C# concept as you write code.
    - The included dll at the top gives you a pretty basic Linqpad like `.Dump<T>()` extension method that will
        dump the contents of the object you call it on to the bottom split.
    - Once the window has opened, pressing <leader>= will open a nuget package search dialog and put the selected package into your csx file.
        - This requires the ctrlp plugin at the moment.
