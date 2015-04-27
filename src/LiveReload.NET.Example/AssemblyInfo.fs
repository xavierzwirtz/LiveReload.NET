namespace System
open System.Reflection

[<assembly: AssemblyTitleAttribute("LiveReload.NET.Example")>]
[<assembly: AssemblyProductAttribute("LiveReload.NET")>]
[<assembly: AssemblyDescriptionAttribute("Implements a LiveReload server for .NET")>]
[<assembly: AssemblyVersionAttribute("1.0.0")>]
[<assembly: AssemblyFileVersionAttribute("1.0.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "1.0.0"
