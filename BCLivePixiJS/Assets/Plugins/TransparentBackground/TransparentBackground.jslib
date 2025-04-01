var LibraryGLClear = {
    glClear: function(mask)
    {
        if (mask == 0x00004000)
        {
            var v = GLctx.getParameter(GLctx.COLOR_WRITEMASK);
            if (!v[0] && !v[1] && !v[2] && v[3])
                // We are trying to clear alpha only -- skip.
                return;
        }
        GLctx.clear(mask);
    },
    
    registerVisibilityChangeEvent: function () 
    {
        document.addEventListener("visibilitychange", function () {
          SendMessage("TrackVisibility", "OnVisibilityChange", document.visibilityState);
        });
        if (document.visibilityState != "visible")
          SendMessage("TrackVisibility", "OnVisibilityChange", document.visibilityState);
    },
    
    clickClose: function (gameId, gameState) 
    {
        console.log("[clickClose] gameId " + UTF8ToString(gameId) + gameState);
        window[UTF8ToString(gameId)].clickClose(gameState);
    },
    
    PrintBuild: function()
    {
        var buildNumber = 62;
        console.log("Build number : " + buildNumber);
    },
    
    WebGlDebug: function(l,c)
    {
        console.log('%c' + UTF8ToString(l), 'color : '+ UTF8ToString(c) +';');
    },

    CopyText: async function(gameId, m)
    {
        window[UTF8ToString(gameId)].copyText(UTF8ToString(m));
        // let ip = document.createElement('input');
        // ip.value = UTF8ToString(m);
        // document.body.appendChild(ip);
        // ip.select();
        // ip.setSelectionRange(0, 99999);
        // navigator.clipboard.writeText(ip.value).then(t=>console.log('copied')).catch(e=>document.execCommand('copy'));
        //
        // document.body.removeChild(ip);
    },
    
    CopyMD5: async function(m)
    {
        var ua = window.navigator.userAgent;
        var iOS = !!ua.match(/iPad/i) || !!ua.match(/iPhone/i);
        var webkit = !!ua.match(/WebKit/i);
        var iOSSafari = iOS && webkit && !ua.match(/CriOS/i);
        
        if (iOSSafari)
        {
           alert("Trình duyệt của bạn không hỗ trợ tính năng này");
           return;
        }
        
        try 
        {
            var clipText = UTF8ToString(m);
            await navigator.clipboard.writeText(clipText);
        } 
        catch (e) 
        {
            // Clipboard API not available
            console.log('Failed to write to clipboard. Clipboard API not available on this browser.');
        }
    }
}
mergeInto(LibraryManager.library, LibraryGLClear); 