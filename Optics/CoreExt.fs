namespace CoreExt

//http://apollo13cn.blogspot.com.au/2012/08/f-extension-methods.html
//For C# access also, use this syntax instead
//[<System.Runtime.CompilerServices.ExtensionAttribute>]
//[<AutoOpen>]
//module ExtensionMethodForCSharp = 
//    [<System.Runtime.CompilerServices.ExtensionAttribute>]
//    let IsFSharp(str:string) = str = "F#"

module Option =

    let tryParseWith 
        (f: string -> bool * 'T) 
        (s: string)
        : 'T option =
    
        match f s with
        | true, value -> Some value
        | false, _ -> None

    let tryParseBoolean =
        tryParseWith System.Boolean.TryParse

    let tryParseDateTime =
        tryParseWith System.DateTime.TryParse

    let tryParseDateTimeExact (format: string) (style: System.Globalization.DateTimeStyles) =
        tryParseWith (fun s -> System.DateTime.TryParseExact(s, format, null, style))
        
    let tryParseDouble =
        tryParseWith System.Double.TryParse

    let tryParseInt16 =
        tryParseWith System.Int16.TryParse

    let tryParseInt32 = 
        tryParseWith System.Int32.TryParse

    let tryParseInt64 =
        tryParseWith System.Int64.TryParse
        
    let tryParseSingle =
        tryParseWith System.Single.TryParse