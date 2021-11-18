namespace Api_OberiChat

open System.ComponentModel.DataAnnotations

module Models =
  
    [<CLIMutable>]
    type User =
        {
            Id : int
            [<Required>]
            Surname : string
            [<Required>]
            Firstname : string
            [<Required>]
            Email : string
            [<Required>]
            Age :  int
        }