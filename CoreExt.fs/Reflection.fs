namespace FSharp.CoreExt.Reflection

module ReflectionUtils =
    let recordTypeWith<'T> (mapWith: (string * obj) -> obj) (o:'T): 'T =
        let oType = typeof<'T>
        let ctorParamValues =
            oType.GetProperties()
            |> Array.map
                (fun propertyInfo ->
                    sprintf "%c%s" (System.Char.ToLowerInvariant propertyInfo.Name.[0]) (propertyInfo.Name.Substring 1),
                    propertyInfo.GetValue o
                )
            |> Array.map
                (fun kv ->
                    fst kv, 
                    mapWith kv
                )
            |> Map.ofArray
        let ctor = o.GetType().GetConstructors().[0]
    
        ctor.GetParameters()
        |> Array.map
            (fun parameterInfo ->
                ctorParamValues
                |> Map.find parameterInfo.Name
            )
        |> ctor.Invoke
        |> (fun x -> x :?> 'T)
