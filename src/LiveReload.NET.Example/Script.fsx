// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#r "../../packages/FSharp.Data/lib/portable-net40+sl5+wp8+win8/FSharp.Data.dll"
#r "../../packages/vtortola.WebSocketListener/lib/net45/vtortola.WebSockets.dll"
#r "../../packages/vtortola.WebSocketListener/lib/net45/vtortola.WebSockets.Rfc6455.dll"
#r "../../packages/Microsoft.Tpl.Dataflow/lib/portable-net45+win8+wpa81/System.Threading.Tasks.Dataflow.dll"

#load "Server.fs"
open LiveReload

let server = Server.start None
