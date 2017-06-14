module IO

open System
open System.IO
open System.Collections.Generic
open System.Text
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library

type MemoryFS(files: Map<string, string>) as this = 
    
    let defaultFileSystem = Shim.FileSystem
    do Shim.FileSystem <- this

    interface IFileSystem with
        // Implement the service to open files for reading and writing
        member __.FileStreamReadShim(fileName) = 
            match files |> Map.tryFind fileName with
            | Some text -> new MemoryStream(Encoding.UTF8.GetBytes(text)) :> Stream
            | _ -> defaultFileSystem.FileStreamReadShim(fileName)

        member __.FileStreamCreateShim(fileName) = 
            defaultFileSystem.FileStreamCreateShim(fileName)

        member __.FileStreamWriteExistingShim(fileName) = 
            defaultFileSystem.FileStreamWriteExistingShim(fileName)

        member __.ReadAllBytesShim(fileName) = 
            match files |> Map.tryFind fileName with
            | Some text -> Encoding.UTF8.GetBytes(text)
            | _ -> defaultFileSystem.ReadAllBytesShim(fileName)

        // Implement the service related to temporary paths and file time stamps
        member __.GetTempPathShim() = 
            defaultFileSystem.GetTempPathShim()
        member __.GetLastWriteTimeShim(fileName) = 
            defaultFileSystem.GetLastWriteTimeShim(fileName)
        member __.GetFullPathShim(fileName) = 
            defaultFileSystem.GetFullPathShim(fileName)
        member __.IsInvalidPathShim(fileName) = 
            defaultFileSystem.IsInvalidPathShim(fileName)
        member __.IsPathRootedShim(fileName) = 
            defaultFileSystem.IsPathRootedShim(fileName)

        // Implement the service related to file existence and deletion
        member __.SafeExists(fileName) = 
            files.ContainsKey(fileName) || defaultFileSystem.SafeExists(fileName)
        member __.FileDelete(fileName) = 
            defaultFileSystem.FileDelete(fileName)

        // Implement the service related to assembly loading, used to load type providers
        // and for F# interactive.
        member __.AssemblyLoadFrom(fileName) = 
            defaultFileSystem.AssemblyLoadFrom fileName
        member __.AssemblyLoad(assemblyName) = 
            defaultFileSystem.AssemblyLoad assemblyName 

    interface IDisposable with
        member __.Dispose() =
            Shim.FileSystem <- defaultFileSystem
