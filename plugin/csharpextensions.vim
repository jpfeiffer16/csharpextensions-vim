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
        let filesOfType = filter(files, {idx, val -> val =~# a:fileType.'$' })
        let directory = fnamemodify(directory, ':h')
    endwhile

    if len(filesOfType) > 1
        throw "Unable to find one file of type '".a:fileType."'"
    endif

    return filesOfType[0]
endfunction

function! csharpextensions#GetResharperDiagnostics() abort
    let slnFile = GetFileType("sln")
    let g:resharper_diagnostics_temp_file = tempname().".xml"
    " call system('jb inspectcode -a -o="'.tempFile.'" '.slnFile)
    call csharpextensions#proc#RunCommand('jb inspectcode -a -o="'.g:resharper_diagnostics_temp_file.'" '.slnFile, function("s:ResharperInspectDone"))
endfunction

function! s:ResharperInspectDone(output)
    cexpr system("dotnet ".s:plugin_path."/tools/ResharperDiagnosticsConverter/ResharperDiagnosticsConverter/bin/Debug/netcoreapp3.1/ResharperDiagnosticsConverter.dll quickfix ".g:resharper_diagnostics_temp_file)
    copen
    " let g:resharper_diagnostics = []
    let g:resharper_diagnostics = {}
    let highlightResult = system("dotnet ".s:plugin_path."/tools/ResharperDiagnosticsConverter/ResharperDiagnosticsConverter/bin/Debug/netcoreapp3.1/ResharperDiagnosticsConverter.dll highlight ".g:resharper_diagnostics_temp_file)
    let lines = split(highlightResult, "\n")
    for line in lines
        let parts = split(line, "|")
" {'lnum': 10, 'vcol': 1, 'col': 34, 'filename': 'AngelDoc/DocumentationGenerator.cs', 'end_lnum': 10, 'type': 'E', 'end_col': 34, 'text': 'Invalid token '';'' in class, record, struct, or interface member declaration'}
        if len(parts) < 6
            echoerr "Error reading line:"
            echoerr parts
        endif
        let filename = parts[0]
        " Resharper will always report files with back-slashes.
        " If we are on unix, we need to replace them with forward-slashes
        if has('unix')
            let filename = substitute(filename, "\\", "/", "g")
        endif
        if (!has_key(g:resharper_diagnostics, filename))
            let g:resharper_diagnostics[filename] = []
        endif
        call add(g:resharper_diagnostics[filename], {
            \ "lnum" : parts[1],
            \ "col" : parts[2],
            \ "end_lnum" : parts[3],
            \ "end_col" : parts[4],
            \ "filename" : filename,
            \ "text" : parts[5],
            \ "type" : "W"
        \ })
    endfor

endfunction

function! s:ALEWantResults() abort
  " if getbufvar(g:ale_want_results_buffer, '&filetype') ==# 'cs'
    call ale#sources#csharpextensions#WantResults(g:ale_want_results_buffer)
  " endif
endfunction

function! csharpextensions#RunScript(noCache) abort
    w
    let command = 'dotnet script '
    if (a:noCache)
        let command = command.' --no-cache '
    endif
    let command = command.s:csxTempFile
    let output = split(system(command), '\n')
    call writefile(output, s:previewFile)
    execute "pedit ".s:previewFile
    " call setbufline(s:resultsBuffer, 1, output)
endfunction

function! csharpextensions#ScratchBuffer() abort
    let s:csxTempFile = tempname().'.csx'
    let s:previewFile = s:csxTempFile.'.txt'
    call writefile([
                \ '#r "'.s:plugin_path.'/tools/CsxExtensions/bin/Debug/netstandard2.0/CsxExtensions.dll"',
                \ '',
                \ 'using CsxExtensions;',
                \ ''
                \], s:csxTempFile)
    execute "tabnew"
    execute "tcd ".fnamemodify(s:csxTempFile, ':h')
    execute "edit ".s:csxTempFile
    " new
    let s:resultsBuffer = bufnr('%')
    " silent wincmd p
    " let command = ":call csharpextensions#RunScript()"
    nnoremap <buffer> <leader><Enter> :call csharpextensions#RunScript(v:false)<CR>
    nnoremap <buffer> <leader>\ :call csharpextensions#RunScript(v:true)<CR>
    normal G
endfunction

augroup CSharpExtensions_Integrations
    autocmd!
    autocmd User ALEWantResults call s:ALEWantResults()
augroup END

command! CSECreateClass :call csharpextensions#GenerateClass()<CR>
command! CSEGetResharperDiagnostics :call csharpextensions#GenerateClass()<CR>
command! CSEScript :call csharpextensions#ScratchBuffer()

