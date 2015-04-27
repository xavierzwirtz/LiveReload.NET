namespace LiveReload
open vtortola.WebSockets

type internal ConnectionManager() =
    let mutable connections = List.empty<WebSocket>
    
    member this.addConnection socket =
        lock this (fun () ->
            connections <- connections @ [socket] 
        )
    member this.removeClosed() = 
        lock this (fun () ->
            connections <- 
                connections |> List.filter(fun c -> c.IsConnected)
        )
    member this.Connections with get() = lock this (fun () -> connections)

module internal Communication = 
    open System.Net
    open System.Threading.Tasks
    open FSharp.Data
    open FSharp.Data.JsonExtensions

    module Async =
        let AwaitTaskVoid : (Task -> Async<unit>) =
            Async.AwaitIAsyncResult >> Async.Ignore

    let handleMessage (socket : WebSocket) (connectionManager : ConnectionManager) (cancellationToken) (message : string) =
        let invalidRequest() =
            Some (sprintf "error: 'invalid message: %s'" message)

        if message = null then
            None
        else
            let parsed = 
                try
                    Some (JsonValue.Parse(message))
                with
                    | _ -> None
            let response = 
                match parsed with 
                    | Some parsed -> 
                        let x = parsed?command
                        match parsed.TryGetProperty("command") with
                        | Some command ->
                            match command.AsString() with 
                            | "info" -> None
                            | "hello" -> 
                                connectionManager.addConnection socket
                                Some "{\
                                    \"command\": \"hello\",\
                                    \"protocols\": [\
                                        \"http://livereload.com/protocols/official-7\"\
                                    ],\
                                    \"server\": \"LiveReload.NET\"\
                                }"
                            | _ -> invalidRequest()
                        | None -> 
                            invalidRequest()
                    | None -> 
                        invalidRequest()

            match response with 
            | Some response -> Some (socket.WriteStringAsync(response, cancellationToken) |> Async.AwaitTaskVoid)
            | None -> None 

    let rec talkToSocket (connectionManager : ConnectionManager) (socket : WebSocket) (cancellationToken : System.Threading.CancellationToken) =
        if socket.IsConnected && not (cancellationToken.IsCancellationRequested) then
            let talk = async {
                let! message = socket.ReadStringAsync(cancellationToken) |> Async.AwaitTask
                match handleMessage socket connectionManager cancellationToken message with
                | None -> ignore()
                | Some x -> do! x
            }
            talk |> Async.RunSynchronously
            talkToSocket connectionManager socket cancellationToken
        
    let rec startListener (server : WebSocketListener) (connectionManager : ConnectionManager) cancellationToken = 
        Task.Run((fun() ->
            let task = server.AcceptWebSocketAsync(cancellationToken)
            task.Wait()
            let socket = task.Result
            talkToSocket connectionManager socket cancellationToken
            startListener server connectionManager cancellationToken
        ), cancellationToken)

    let start (port : int) cancellationToken = 
        
        let server = new WebSocketListener(IPEndPoint(IPAddress.Any, port))
        let connManager = ConnectionManager()

        let rfc6455 = new vtortola.WebSockets.Rfc6455.WebSocketFactoryRfc6455(server)
        server.Standards.RegisterStandard(rfc6455)
        server.Start()

        //let cancellationToken = new System.Threading.CancellationTokenSource()

        let listenTask = startListener server connManager cancellationToken

        cancellationToken.Register(fun () ->
            server.Dispose()
        ) |> ignore

        connManager

    let sendReload cancellationToken (path : string) (sockets : WebSocket seq) =
        let path = Newtonsoft.Json.JsonConvert.SerializeObject(path)
        let message = sprintf "{
            \"command\": \"reload\",
            \"path\": %s,
            \"liveCSS\": true}" path
        for socket in sockets do
            socket.WriteStringAsync(message, cancellationToken) |> Async.AwaitTaskVoid |> Async.StartImmediate