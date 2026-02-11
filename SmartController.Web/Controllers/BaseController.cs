using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartController.Shared.Enums;

namespace SmartController.Web.Controllers;

[Authorize]
public abstract class BaseController : Controller
{
    protected int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    protected string UserEmail => User.FindFirstValue(ClaimTypes.Email) ?? "";
    protected string UserName => User.FindFirstValue(ClaimTypes.Name) ?? "";
    protected UserRole UserRole => Enum.Parse<UserRole>(User.FindFirstValue(ClaimTypes.Role) ?? "User");
    protected int? DistributorId
    {
        get
        {
            var val = User.FindFirstValue("DistributorId");
            return string.IsNullOrEmpty(val) ? null : int.Parse(val);
        }
    }

    protected bool IsAdmin => UserRole == UserRole.Admin;
    protected bool IsDistributor => UserRole == UserRole.Distributor;
}
