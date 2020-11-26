function! ale#sources#csharpextensions#WantResults(buffer) abort
    if !empty(get(g:, 'resharper_diagnostics', []))
        call ale#other_source#ShowResults(bufnr('%'), 'Resharper', g:resharper_diagnostics)
    endif
endfunction
