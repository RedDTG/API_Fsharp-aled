namespace Api_OberiChat

open Api_OberiChat.Models
open Microsoft.EntityFrameworkCore
open System.Linq

module DataContext =

    type UserContext(options : DbContextOptions<UserContext>) = 
        inherit DbContext(options)
    
        [<DefaultValue>]
        val mutable Users : DbSet<User>
        member public this._Users      with    get()   = this.Users 
                                           and     set value  = this.Users <- value 

        //returns if the Item exists 
        member this.UserExist (id:int) = this.Users.Any(fun x -> x.Id = id)

        //Returns the Item with the given id
        member this.GetUser (id:int) = this.Users.Find(id)

    let Initialize (context : UserContext) =
        //context.Database.EnsureDeleted() |> ignore //Deletes the database
        context.Database.EnsureCreated() |> ignore //check if the database is created, if not then creates it
        //default Items for testing
        let Users : User[] = 
            [|
                { Id = 0; Surname = "Do the software"; Firstname = "yabai"; Email = "email@email.com"; Age = 50 }
                { Id = 1; Surname = "Create the Article"; Firstname = "yabai"; Email = "email@email.com"; Age = 50 }
                { Id = 2; Surname = "Upload to the internet"; Firstname = "yabai"; Email = "email@email.com"; Age = 50 }
            |];
        if not(context.Users.Any()) then
                context.Users.AddRange(Users) |> ignore
                context.SaveChanges() |> ignore  