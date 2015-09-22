[![Issue Stats](http://issuestats.com/github/xavierzwirtz/LiveReload.NET/badge/issue)](http://issuestats.com/github/xavierzwirtz/LiveReload.NET)
[![Issue Stats](http://issuestats.com/github/xavierzwirtz/LiveReload.NET/badge/pr)](http://issuestats.com/github/xavierzwirtz/LiveReload.NET)

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

- [@xavierzwirtz](https://github.com/xavierzwirtz)
