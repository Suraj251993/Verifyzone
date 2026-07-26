using Microsoft.AspNetCore.Mvc;

namespace OrgCheck.Controllers
{
    public class BgvController : Controller
    {
        public BgvController()
        {
            
        }
        [IgnoreAntiforgeryToken]
        public IActionResult Index()
        {
            return RedirectToActionPermanent("Index", "Home");
        }        
    }
}
