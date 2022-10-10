using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data.DbConfig;

namespace MovieAPI.Controllers
{
    public class ChatController : Controller
    {
        private readonly MovieAPIDbContext context;

        public ChatController(MovieAPIDbContext db)
        {
            context = db;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
