module Sandbox

open Microsoft.FSharp.Compiler.SourceCodeServices
open System.IO
open System.Reflection



//let compileToDynamic 
//    (s: string)
//    : Assembly =
//    use reader = System.IO.string
//    let chk = FSharpChecker.Create()
//    async {
//        let! asd = chk.ParseAndCheckProject ()
//        asd.
//    }
//    let compileErrors, compileExitCode, assembly =
//        chk.CompileToDynamicAssembly(
//            [| 
//                "-o"
//                null
//                "-a"

//            |],
//            execute = None
//            )
//    assembly

let inline FirstName_< ^a when ^a : (member firstName : string) > () =
    (fun o -> (^a : (member firstName : string) o)),
    (fun (value: string) (o: ^a) ->
        o |> FSharp.CoreExt.Reflection.ReflectionUtils.recordTypeWith 
            (fun (k, v) ->
                match k,v with
                | "firstName", _ -> upcast value
                | _, value -> value
            )
    ),
    (fun (value: string) (o: ^a) ->
        { o with
            ^a.firstName = value
        }
    )

type Lenses = Optics.LenProvider<"c:\dev\myproj\debug\bin\myproj.dll">
let firstNameGetter, firstNameSetter = Lenses.FirstName_()

let get = fst
let set = snd

    
type Bear = {
    firstName: string
}
let pete:Bear = {
    firstName = "Pete"
}

type Person = {
    firstName: int
}

let david:Person = {
    firstName = 123
}

let davidsName = FirstName_() |> get <| david 

let petesName = FirstName_() |> get <| pete



//open Microsoft.FSharp.Reflection
 
//let (?) x m args =
//    let args =
//        if box args = null then (* the unit value is null, once boxed *)
//            [||]
//        elif FSharpType.IsTuple (args.GetType()) then
//            FSharpValue.GetTupleFields args
//        else
//            [|args|]
//    let result = x.GetType().InvokeMember(m, System.Reflection.BindingFlags.GetProperty ||| System.Reflection.BindingFlags.InvokeMethod, null, x, args)
//    args |> Array.head |> (fun x -> x.ToString())




//type Bear = {
//    firstName: int
//}
//let pete:Bear = {
//    firstName = 123
//}

//let asd2 = getFirstName_ pete

//let asd = setFirstName_ "asd" pete 

//let petesName = pete ? first3Name()