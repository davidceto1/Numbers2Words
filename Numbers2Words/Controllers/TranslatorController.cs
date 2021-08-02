using Microsoft.AspNetCore.Mvc;
using Numbers2WordsLogic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Numbers2Words.Controllers
{
    public class TranslatorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Translate(string number)
        {
            if (decimal.TryParse(number, out var ParsedNumber))
            {
                ViewData["Number"] = ParsedNumber.ToString("#.##");
                ViewData["Translation"] = Translator.Translate(ParsedNumber.ToString("#.##"));
                return View();
            }
            else
            {
                ViewData["Number"] = number;
                return View("DisplayError");
            }
        }
    }
}
