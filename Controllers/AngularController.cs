using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASPxAngular.Controllers;

public class AngularController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewBag.App = "ng-aspxangular";
        ViewBag.RootTag = "app-root";
        ViewBag.Title = "ASPxAngular";
        // ViewBag.BasePath = "/";
        // ((AngularController)context.Controller).ViewBag.BasePath = Url.RouteUrl(new { controller = context.RouteData.Values["controller"], action = "Index" });
        base.OnActionExecuting(context);
    }


    [HttpGet("scripts/{*pathInfo}")]
    public ActionResult Scripts(string pathInfo)
    {
        return Redirect($"~/{ViewBag.App}/{pathInfo}");
    }


    [HttpGet("{*pathInfo}")]
    public virtual ActionResult CatchAll(string pathInfo)
    {
        var extensions = new string[] { ".js", ".css", ".svg" };
        if (extensions.Any(ext => pathInfo?.Split("?").FirstOrDefault().EndsWith(ext) == true))
        {
            return Scripts(pathInfo);
        }
        return View("Index");
    }

    [Authorize]
    [HttpGet("UserInfo")]
    public virtual ActionResult GetUserInfo()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Unauthorized();
        }

        return Json(new
        {
            name = User.Identity.Name,
            email = User.FindFirst(c => c.Type.Contains("emailaddress"))?.Value ?? "",
            claims = User.Claims
                .Select(c => new Dictionary<string, string> { { c.Type, c.Value } })
                .ToList()
        });
    }

}
