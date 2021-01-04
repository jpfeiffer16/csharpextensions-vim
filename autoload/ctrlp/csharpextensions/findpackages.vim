call add(g:ctrlp_ext_vars, {
\ 'init': 'ctrlp#csharpextensions#findpackages#init()',
\ 'accept': 'ctrlp#csharpextensions#findpackages#accept',
\ 'lname': 'Nuget Packages',
\ 'sname': 'packages',
\ 'type': 'tabs',
\ 'sort': 1,
\ 'nolim': 1,
\ })

let s:package_list = []
let s:Callback = v:null

function! ctrlp#csharpextensions#findpackages#set_callback(callback) abort
    let s:Callback = a:callback
endfunction

function! ctrlp#csharpextensions#findpackages#set_list(package_list) abort
    let s:package_list = a:package_list
endfunction

function! ctrlp#csharpextensions#findpackages#init() abort
    return s:package_list
endfunction

function! ctrlp#csharpextensions#findpackages#accept(mode, str) abort
    call ctrlp#exit()
    if (s:Callback != v:null)
        call s:Callback(a:str)
    endif
endfunction

let s:id = g:ctrlp_builtins + len(g:ctrlp_ext_vars)

function! ctrlp#csharpextensions#findpackages#id() abort
    return s:id
endfunction
