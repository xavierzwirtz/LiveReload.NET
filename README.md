[![Issue Stats](http://issuestats.com/github/VoiceOfWisdom/LiveReload.NET/badge/issue)](http://issuestats.com/github/VoiceOfWisdom/LiveReload.NET)
[![Issue Stats](http://issuestats.com/github/VoiceOfWisdom/LiveReload.NET/badge/pr)](http://issuestats.com/github/VoiceOfWisdom/LiveReload.NET)

# LiveReload.NET

This library makes communicating with LiveReload from .NET incredibly easy.

## Example

```f#
open LiveReload

let server = new Server(None)
server.Start()
server.SendReload("css/app.css")
```

## Maintainer(s)

- [@voiceofwisdom](https://github.com/voiceofwisdom)
