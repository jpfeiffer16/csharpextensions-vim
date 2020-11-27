function! ale#sources#csharpextensions#WantResults(buffer) abort
    if !empty(get(g:, 'resharper_diagnostics', []))
        let filename = expand('%:p')
        if has_key(g:resharper_diagnostics, filename)
          call ale#other_source#ShowResults(bufnr('%'), 'Resharper', g:resharper_diagnostics[filename])
        endif
    endif
endfunction
