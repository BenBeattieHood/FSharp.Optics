module LensTypeProviderTests

open System
open Xunit
open Swensen.Unquote

open Optics

type Test2 = {
    firstName: string
}

type Bear = StaticProperty.Provided.LensProvider<"">


//let asd = Bear.Test1 ()
()