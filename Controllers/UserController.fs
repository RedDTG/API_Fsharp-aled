namespace Api_OberiChat

open System.Collections.Generic
open Microsoft.AspNetCore.Mvc
open Api_OberiChat.DataContext
open Api_OberiChat.Models
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Logging
open System.Linq;

[<Route("api/Users")>]
[<ApiController>]
type UserController (logger : ILogger<UserController>) = 
    inherit ControllerBase()
    // new (context : UserContext) as this =
    //     UserController () then
    //     this._Context <- context
    
    [<HttpGet>]
    member this.Get() =
        ActionResult<IEnumerable<User>>(this._Context.Users)

    //GET: api/Users/search
    [<Route("search")>]
    [<HttpGet>]
    member this.Get([<FromBody>] _Values : User[] ) =
        if base.ModelState.IsValid then //check the entry
            
            let Users : List<User> = new List<User>() // initiates the list (i like to keep the stuff typed)
            
            for value in _Values do //Search all the Users 
                if (value.Id = 0) then // if the id is not informed then search by the Name
                    Users.AddRange(this._Context.Users.Where(fun x -> x.Firstname = value.Firstname).ToList())
                else if(this._Context.UserExist(value.Id)) then // search by the the id
                    Users.Add(this._Context.Users.Find(value.Id))

            if (Users.Count = 0) then
                ActionResult<IActionResult>(base.NotFound("NOT FOUND!, The search returned 0 values"))
            else
                ActionResult<IActionResult>(base.Ok(Users))
        else 
            ActionResult<IActionResult>(base.BadRequest(base.ModelState))

    [<HttpGet("{id}")>]
    member this.Get(id:int) = 
        if base.ModelState.IsValid then  //check the entry
            if not ( this._Context.UserExist(id) ) then //check the existence of the User
                ActionResult<IActionResult>(base.NotFound("NOT FOUND!, There is no User with this code: " + id.ToString())) // User does not exist
            else
                ActionResult<IActionResult>(base.Ok(this._Context.GetUser(id)))
        else
            ActionResult<IActionResult>(base.BadRequest(base.ModelState))

    [<HttpPost>]
    member this.Post([<FromBody>] _User : User) =
        if (base.ModelState.IsValid) then 
            if not( isNull _User.Firstname ) then
                if ( _User.Id <> 0 ) then //check if the ID is set
                    ActionResult<IActionResult>(base.BadRequest("BAD REQUEST, the UserID is autoincremented")) // the User is autoincremented
                else 
                        this._Context.Users.Add(_User) |> ignore
                        this._Context.SaveChanges() |> ignore
                        ActionResult<IActionResult>(base.Ok(this._Context.Users.Last()))
            else
                ActionResult<IActionResult>(base.BadRequest("BAD REQUEST!, the field Initials can not be null"))                    
        else
            ActionResult<IActionResult>(base.BadRequest(base.ModelState))

    [<HttpPut("{id}")>]
     member this.Put( id:int, [<FromBody>] _User : User) =
        if (base.ModelState.IsValid) then 
            if not( isNull _User.Firstname ) then
                if (_User.Id <> id) then 
                    ActionResult<IActionResult>(base.BadRequest())
                else
                        try//error handler
                            this._Context.Entry(_User).State = EntityState.Modified |> ignore
                            this._Context.SaveChanges() |> ignore
                            ActionResult<IActionResult>(base.Ok(_User))
                        with ex ->
                            if not( this._Context.UserExist(id) ) then
                                ActionResult<IActionResult>(base.NotFound())
                            else 
                                ActionResult<IActionResult>(base.BadRequest())
            else
                ActionResult<IActionResult>(base.BadRequest())                                
        else    
            ActionResult<IActionResult>(base.BadRequest(base.ModelState))

    [<HttpDelete("{id}")>]
    member this.Delete(id:int) =
        if (base.ModelState.IsValid) then 
            if not( this._Context.UserExist(id) ) then 
                ActionResult<IActionResult>(base.NotFound())
            else (
                    this._Context.Users.Remove(this._Context.GetUser(id)) |> ignore
                    this._Context.SaveChanges() |> ignore
                    ActionResult<IActionResult>(base.Ok(this._Context.Users.Last()))
            )
        else
            ActionResult<IActionResult>(base.BadRequest(base.ModelState))

    [<DefaultValue>]
    val mutable _Context : UserContext