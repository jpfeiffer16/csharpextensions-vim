function! csharpextensions#proc#RunCommand(command, cb) abort
    if has('nvim')
        call RunNeoVim(a:command, a:cb)
    else
        call RunVim(a:command, a:cb)
    endif
endfunction

function! RunNeoVim(command, cb) abort
    let s:neoVimLines = []
    let s:neoVimCallback = a:cb
    function! OnStdout(id, data, event)
        for line in a:data
            if !empty(trim(line))
                call add(s:neoVimLines, line)
            endif
        endfor
    endfunction
    function! OnExit(id, data, event)
        call s:neoVimCallback(s:neoVimLines)
    endfunction
    let opts = { "stdout_buffered" : v:true, "on_stdout" : "OnStdout", "on_exit" : "OnExit" }
    let job = jobstart(a:command, opts)
endfunction

function! RunVim(command, cb) abort
    let s:vimLines = []
    let s:vimCallback = a:cb
    function! OnExit(channel)
        call s:vimCallback(s:vimLines)
    endfunction
    function! OnStdout(msg, channel)
        call add(s:vimLines, a:msg)
    endfunction
    let job = job_start(a:command, { "mode" : "nl", "out_cb" : "OnStdout", "close_cb" : "OnExit" })
endfunction
