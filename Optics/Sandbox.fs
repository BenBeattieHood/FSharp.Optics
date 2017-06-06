module Sandbox

open Optics


[<Lens>]
type Address = {
    street1: string
    street2: string
    city: string
}

[<Lens>]
type Person = {
    firstName: string
    lastName: string
    address: Address
}