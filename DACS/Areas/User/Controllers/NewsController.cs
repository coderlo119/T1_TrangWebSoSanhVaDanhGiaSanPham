﻿using DACS.Interface;
using DACS.Models.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DACS.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class NewsController : Controller
    {
		private readonly INews _news;
        public NewsController(INews news)
        {
            _news = news;
        }
        public async Task<IActionResult> Index(string Searchtext, int? page)
        {
            var news = await _news.GetWithIsActiveAsync();
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 5;
            int pageNum = page ?? 1;
            if (!string.IsNullOrEmpty(Searchtext))
            {
                news = news.Where(news => news.Alias.ToUpper().Contains(Searchtext) || news.Title.ToUpper().Contains(Searchtext)).ToList();
            }
            return View(news.ToPagedList(pageNum, pageSize));
        }
        public async Task<IActionResult> Display(int id)
        {
            var news = await _news.GetByIdAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }
    }
}
