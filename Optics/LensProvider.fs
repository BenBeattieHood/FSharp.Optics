namespace Optics

open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices

open ProviderImplementation
open ProviderImplementation.ProvidedTypes
open System.Reflection

[<TypeProvider>]
type _LensProvider (config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces ()
    
    let ns = "Optics"
    let asm = Assembly.GetExecutingAssembly()
    let ctxt = ProvidedTypesContext.Create(config)
    let runtimeAssembly = config.RuntimeAssembly

    let types = 
        asm.GetTypes()
        |> Array.filter
            (fun t ->
                t.IsSealed && 
                t.IsClass &&
                t.IsSerializable &&
                t.IsNestedAssembly = false &&
                t.BaseType = typeof<obj>
            )

    let lensProvider = ctxt.ProvidedTypeDefinition(asm, ns, "LensProvider", Some typeof<obj>)
    do lensProvider.DefineStaticParameters(
        [ ProvidedStaticParameter("AssemblyPath", typeof<string>) ],
        fun typeName args ->
            match args with
            | ([| :? string as assemblyPath |]) ->
                let provider = ProvidedTypeDefinition(asm, ns, typeName, Some typeof<obj>, HideObjectMethods = true)

                let sourceAssembly = System.Reflection.Assembly.LoadFile config.RuntimeAssembly

                for t in types do
                    let method = ProvidedMethod(t.Name, [ProvidedParameter("arg", typeof<unit>)], typeof<unit>)
                    method.IsStaticMethod <- true
                    method.InvokeCode <- fun args -> <@@ () @@>
                    method.AddXmlDocDelayed(fun () -> "This is a static method")
                    provider.AddMember method

                provider

            | _ ->
                invalidArg "AssemblyPath" "LensProvider arg must be the path to the assembly to interrogate"
        )

    //let createTypes () =
    //    let myType = 
    //    let myProp = ctxt.ProvidedProperty("MyProperty", typeof<string>, IsStatic = true, getterCode = (fun args -> <@@ "Hello world" @@>))
    //    myType.AddMember(myProp)
    //    [myType]

    do
        this.AddNamespace(ns, [lensProvider])

[<assembly:TypeProviderAssembly>]
do ()