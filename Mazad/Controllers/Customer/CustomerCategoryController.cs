using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Mazad.Api.Controllers;
using Mazad.Core.Domain.Users.Authentication;
using Mazad.Core.Shared.Contexts;
using Mazad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mazad.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerCategoryController : BaseController
{
    private readonly MazadDbContext _context;

    public CustomerCategoryController(MazadDbContext context)
    {
        _context = context;
    }

}