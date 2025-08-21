using System;
using API.DTOs;
using Core.Entities;
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
}
