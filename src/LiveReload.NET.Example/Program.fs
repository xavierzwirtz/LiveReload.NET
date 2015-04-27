module Program
    open Nancy
    open Nancy.Conventions
    open LiveReload

    let colors = [
        "red"
        "green"
        "blue"
        "pink"
    ]
    let mutable colorIndex = 0
    type App() as this =
        inherit NancyModule()
        do
            this.Get.["/"] <- fun _ -> new Nancy.Responses.GenericFileResponse("Content/index.html", "text/html") :> obj
            this.Get.["/app.css"] <- fun _ -> 
                colorIndex <- (colorIndex + 1)
                if colorIndex > ((colors |> Seq.length) - 1) then
                    colorIndex <- 0

                let color = colors |> Seq.nth (colorIndex)
                let text = sprintf "body { background-color: %s }" color
                let textBytes = System.Text.Encoding.UTF8.GetBytes(text)
                let r = new Response()
                r.ContentType <- "text/css"
                r.Contents <- System.Action<System.IO.Stream> (fun s -> s.Write(textBytes, 0, textBytes.Length))
                r :> obj

    let live = new Server()
    let url = "http://localhost:8080"
    let host = new Nancy.Hosting.Self.NancyHost(System.Uri url)
    live.Start()
    host.Start()
    System.Diagnostics.Process.Start(url) |> ignore
    printfn "WebServer running at %s" url

    let timer = new System.Timers.Timer(2500.0)
    timer.Elapsed.Add (fun _ -> live.SendReload("app.css"))
    timer.Start()

    System.Console.ReadLine() |> ignore
    host.Dispose()
    live.Stop()
    timer.Stop()