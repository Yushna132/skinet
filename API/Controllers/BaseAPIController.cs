using System;
using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
// on a créer cette classe enfin de mettre les codes par défaut qui existe dans tous les API
[Route("api/[controller]")]
[ApiController]
public class BaseAPIController : ControllerBase
{

    //protected => it make it accessible in this controller and other controller that derived
    //from this controller

    protected async Task<ActionResult> CreatePageResult<T>(IGenericRepository<T> repo,
    ISpecification<T> spec, int pageIndex, int pageSize) where T: BaseEntity
    {
        var items = await repo.ListAsync(spec);
        var counts = await repo.CountAsync(spec);
        var pagination = new Pagination<T>(pageIndex, pageSize, counts, items);
        return Ok(pagination);
    }


}
