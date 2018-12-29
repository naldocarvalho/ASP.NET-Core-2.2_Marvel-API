using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SiteHeroisMarvel.Models;

namespace SiteHeroisMarvel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index([FromServices]APIMarvelClient client)
        {
            return View(client.ObterDadosPersonagem());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
