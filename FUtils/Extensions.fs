namespace Utils

open System
open System.Runtime.CompilerServices

module Extensions =
    
    [<Extension>]
    type Comparable () =
        [<Extension>]
        static member Between (num: System.IComparable) (min: System.IComparable) (max: System.IComparable) : bool =
            num > min && num < max
