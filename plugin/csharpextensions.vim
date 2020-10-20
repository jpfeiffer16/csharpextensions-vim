function! csharpextensions#GenerateClass()
    let file = expand('%:p')
    let topLevelDirectory = fnamemodify(file, ':h') 
    let directory = topLevelDirectory
    let csprojFiles = []
    while empty(csprojFiles)
        let files = globpath(directory, '*', 0, 1)
        let csprojFiles = filter(files, {idx, val -> val =~ '.csproj' })
        let directory = fnamemodify(directory, ':h')
    endwhile
    if !empty(csprojFiles)
        let csprojFile = get(csprojFiles, 0)
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
