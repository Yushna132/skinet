using System;
using System.Linq.Expressions;
using System.Security.Claims;
using API.DTOs;
using API.Extensions;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace API.Controllers;

public class AccountController(SignInManager<AppUser> signInManager) : BaseAPIController
{
    //Cet endpoint /api/account/register :
    // Reçoit un objet RegisterDTO depuis le client.
    // Crée un utilisateur AppUser.
    // Utilise UserManager (via SignInManager) pour l’ajouter en base avec un mot de passe sécurisé.
    // Retourne soit un 200 OK, soit un 400 Bad Request avec les erreurs.

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDTO registerDTO)
    {
        var user = new AppUser
        {
            FirstName = registerDTO.FirstName,
            LastName = registerDTO.LastName,
            Email = registerDTO.Email,
            UserName = registerDTO.Email
        };

        var result = await signInManager.UserManager.CreateAsync(user, registerDTO.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem();
        }
        return Ok();
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return NoContent();
    }

    //[Authorize]
    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated == false) return NoContent();
        var user = await signInManager.UserManager.GetUserByEmailWithAddress(User);

        return Ok(new
        {
            user.FirstName,
            user.LastName,
            user.Email,
            Address = user.Address?.ToDto()
        });
    }

    //Savoir rapidement si l’utilisateur est connecté ou pas.
    [HttpGet("auth-state")]
    public ActionResult GetAuthState()
    {
        return Ok(new { IsAuthenticated = User?.Identity?.IsAuthenticated ?? false });
    }

    [Authorize]
    [HttpPost("address")]
    public async Task<ActionResult<Address>> CreateOrUpdateAddress(AddressDto addressDto)
    {
        var user = await signInManager.UserManager.GetUserByEmailWithAddress(User);
        if (user.Address == null)
        {
            user.Address = addressDto.ToEntity();
        }
        else
        {
            user.Address.UpdateFromDto(addressDto);
        }

        var result = await signInManager.UserManager.UpdateAsync(user);
        if (!result.Succeeded) return BadRequest("Problem updating user address");
        return Ok(user.Address.ToDto());
    }

}
