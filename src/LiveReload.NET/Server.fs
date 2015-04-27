namespace LiveReload

    type Server(port: int option) =
        
        let mutable started = false
        let mutable cancellationToken : System.Threading.CancellationTokenSource option = None
        let mutable connectionManager :  ConnectionManager option = None

        member this.Start() = 
            if started then
                failwith "Cannot start when the server is already started."
            started <- true
            cancellationToken <- Some(new System.Threading.CancellationTokenSource())
            connectionManager <- Some (Communication.start port cancellationToken.Value.Token)

        member this.SendReload path =
            if not started then
                failwith "Cannot sendRelaod when server is not started."
            Communication.sendReload cancellationToken.Value.Token path connectionManager.Value.Connections 

        member this.Stop() = 
            started <- false
            if cancellationToken.IsSome then
                cancellationToken.Value.Cancel()

        interface System.IDisposable with
            member this.Dispose() = 
                this.Stop()