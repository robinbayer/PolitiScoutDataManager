using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Overthink.PolitiScout.Controllers
{
    public class PersonUIController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
