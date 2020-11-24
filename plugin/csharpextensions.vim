let s:plugin_path = expand('<sfile>:p:h:h')

function! csharpextensions#GenerateClass() abort
    let csprojFile = GetFileType('csproj')
    let file = expand('%:p')
    let topLevelDirectory = fnamemodify(file, ':h') 
    let directory = topLevelDirectory
    if !empty(csprojFile)
        let namespaceString = fnamemodify(csprojFile, ':t:r')
        let csprojFolder = fnamemodify(csprojFile, ':h')
        let csprojFolderLen = len(csprojFolder)
        let dirLen = len(topLevelDirectory)
        let lenDiff = dirLen - csprojFolderLen
        let namespacePath = file[dirLen - lenDiff : dirLen]
        if len(namespacePath) > 1
            let namespaceString = trim(namespaceString.substitute(namespacePath, '[/|\\]', '.', 'g'), '.')
        endif
        let classString = fnamemodify(file, ':t:r')
        execute 'normal! iusing System;'
        execute 'normal! o'
        execute 'normal! onamespace '.namespaceString
        execute 'normal! o{'
        execute 'normal! opublic class '.classString
        execute 'normal! o{'
        execute 'normal! o}'
        execute 'normal! o}'
        execute 'normal! kko'
    endif
endfunction

function! GetFileType(fileType) abort
    let file = expand('%:p')
    let topLevelDirectory = fnamemodify(file, ':h') 
    let directory = topLevelDirectory
    let filesOfType = []
    while empty(filesOfType)
        let files = globpath(directory, '*', 0, 1)
        let filesOfType = filter(files, {idx, val -> val =~ (".".a:fileType) })
        let directory = fnamemodify(directory, ':h')
    endwhile

    if len(filesOfType) > 1
        throw "Unable to find one file of type '".fileType."'"
    endif

    return filesOfType[0]
endfunction

function! csharpextensions#GetResharperDiagnostics() abort
    let slnFile = GetFileType("sln")
    let tempFile = tempname().".xml"
    execute "!jb inspectcode -o='".tempFile."' ".slnFile
    " cexpr system('find . -name whatever.txt -printf "%p:1:1:%f\n"')
    cexpr system('dotnet '.s:plugin_path.'/tools/ResharperDiagnosticsConverter/bin/Debug/netcoreapp3.1/ResharperDiagnosticsConverter.dll '.tempFile)
    " echom 'dotnet '.s:plugin_path.'/tools/ResharperDiagnosticsConverter/bin/Debug/netcoreapp3.1/ResharperDiagnosticsConverter.dll '.tempFile
endfunction
