using System;
using System.Security.Claims;
using API.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

//lecontrolleur de test qui va retourner les erreurs
public class BuggyController : BaseAPIController
{
    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized()
    {
        return Unauthorized();
    }

    [HttpGet("badrequest")]
    public IActionResult GetBadRequest()
    {
        return BadRequest();
    }

    [HttpGet("notfound")]
    public IActionResult GetNotFound()
    {
        return NotFound();
    }

    [HttpGet("internalerror")]
    public IActionResult GetInternalError()
    {
        throw new Exception("this is a test exception");
    }

    /*  [HttpPost("validationerror")]
     public IActionResult GetValidationError(Product product)
     {
         return Ok();
     } */

    //Voir explication dans notes Section 6

    [HttpPost("validationerror")]
    public IActionResult GetValidationError(CreateProductDto product)
    {
        return Ok();
    }

    [Authorize]
    [HttpGet("secret")]
    public IActionResult GetSecret()
    {
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Ok("Hello " + name + " with the id of " + id);
    }

    
    
}
