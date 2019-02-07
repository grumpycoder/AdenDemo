using Aden.Web;
using Alsde.Extensions;
using Alsde.Security.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace AdenDemo.Web.Controllers
{
    public class AccountController : Controller
    {
        //Callback url from TPA login
        public ActionResult LoginCallback(string token)
        {
            var tokenKey = new TokenKey(token, Constants.TpaAccessKey);

            var result = IdentityManager.TokenSignin(Constants.WebServiceUrl, tokenKey);

            Session["LoginFailureMessage"] = string.Empty;
            if (result.IsFailure)
            {
                Session["LoginFailureMessage"] = result.Error;
                return RedirectToAction("LoginFailure", "Account");
            }

            var identity = result.Value;
            if (identity == null) throw new Exception("No identity returned from Token signin");

            // Add custom claims to User to store Section information
            var claims = identity.Claims.ToList();

            //TODO: Remove magic strings
            var claim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value.ToLower().Contains("section"));
            var isAdministrator = claims.Exists(c => c.Type == ClaimTypes.Role && c.Value.ToLower().StartsWith("adenapp") && c.Value.ToLower().EndsWith("administrators"));
            //claims.Any(c => c.Value.ToLower().Contains("administrator"));

            if (claim != null)
            {
                var section = claim.Value.SplitCamelCase().Split(' ').ToList();
                var idxSection = section.IndexOf("Section");
                var idxApp = section.IndexOf("App");

                var sectionName = string.Join(" ", section.Skip(idxApp + 1).Take(idxSection - idxApp - 1).ToList());
                identity.AddClaim(new Claim("Section", sectionName));
                //identity.AddClaim(new Claim(ClaimTypes.Role, "AdenAppUsers"));
            }

            if (isAdministrator) return RedirectToAction("Submissions", "Home");

            return RedirectToAction("Assignments", "Home");

        }


        public ActionResult Signout()
        {
            IdentityManager.IdentitySignout();

            var logoutUrl = $"https://{Constants.LogoutUrl}{Constants.AimApplicationViewKey}";

            return Redirect(logoutUrl);
        }

        public ActionResult Unauthorized()
        {
            return View();
        }

        public ActionResult LoginFailure(string message)
        {
            ViewBag.Message = Session["LoginFailureMessage"];
            return View();
        }
    }
}