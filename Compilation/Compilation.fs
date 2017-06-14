module Compilation

open Microsoft.FSharp.Compiler.SourceCodeServices
open System.IO

type CompilationErrors = {
    errors: Microsoft.FSharp.Compiler.FSharpErrorInfo[]
    exitCode: int
}

let compile
    (code: string)
    : Result<System.Reflection.Assembly, CompilationErrors> =

    let checker = FSharpChecker.Create()
    
    let temporaryFileName = Path.GetTempFileName()
    let temporarySourceFileName = Path.ChangeExtension(temporaryFileName, ".fs")
    let temporaryAssemblyFileName = Path.ChangeExtension(temporaryFileName, ".dll")
    let temporaryDocumentationFileName = Path.ChangeExtension(temporaryFileName, ".xml")

    let projectOptions = 
        let sysLib nm = 
            if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then // file references only valid on Windows 
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) +
                @"\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\" + nm + ".dll"
            else
                let sysDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
                let (++) a b = System.IO.Path.Combine(a,b)
                sysDir ++ nm + ".dll" 

        let fsCore4300() = 
            if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then // file references only valid on Windows 
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86) +
                @"\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\FSharp.Core.dll"  
            else 
                sysLib "FSharp.Core"

        let allFlags = 
            [| 
                yield "--simpleresolution"
                yield "--noframework"
                yield "--debug:full"
                yield "--define:DEBUG"
                yield "--optimize-"
                yield sprintf "--doc:%s" temporaryDocumentationFileName
                yield "--warn:3"
                yield "--fullpaths"
                yield "--flaterrors"
                yield "--target:library"
                let references =
                    [ 
                        sysLib "mscorlib" 
                        sysLib "System"
                        sysLib "System.Core"
                        fsCore4300() 
                    ]
                for r in references do 
                yield "-r:" + r 
            |]
 
        { 
            FSharpProjectOptions.ProjectFileName = @"c:\_\compilation.fsproj" // Make a name that is unique in this directory.
            ProjectFileNames = [| temporarySourceFileName |]
            OtherOptions = allFlags 
            ReferencedProjects = [| |]
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = true 
            LoadTime = System.DateTime.Now // Note using 'Now' forces reloading
            UnresolvedReferences = None
            OriginalLoadReferences = []
            ExtraProjectInfo = None
        }
        
    let errors, exitCode, assembly =
        using 
            (new IO.MemoryFS([temporarySourceFileName, code] |> Map.ofSeq))   // MemoryFS will set/restore the Shim.FileSystem used by FSharpChecker
            (fun _ ->
                async {
                    let! parseProjectResults = 
                        projectOptions
                        |> checker.ParseAndCheckProject 

                    return! checker.CompileToDynamicAssembly(
                        [| "-o"; temporaryAssemblyFileName; "-a"; temporarySourceFileName |],
                        Some(stdout, stderr)
                        )
                } 
                |> Async.RunSynchronously
            )

    assembly
    |> Result.ofOption { CompilationErrors.errors = errors; exitCode = exitCode }