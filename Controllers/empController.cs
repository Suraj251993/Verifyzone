using Microsoft.AspNetCore.Mvc;

namespace OrgCheck.Controllers
{
    public class empController : Controller
    {
        public empController()
        {
            
        }
        [IgnoreAntiforgeryToken]
        public IActionResult Index()
        {
            return RedirectToActionPermanent("Index", "Home");
        }        
    }
}
