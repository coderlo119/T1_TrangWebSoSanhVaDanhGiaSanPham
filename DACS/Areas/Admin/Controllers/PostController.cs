﻿using DACS.Interface;
using DACS.Models.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DACS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PostController : Controller
    {
        private readonly IPost _post;
        public PostController(IPost post)
        {
            _post = post;
        }
        public async Task<IActionResult> Add()
        {
            var post = await _post.GetAllAsync();
            return View();
        }
        public async Task<IActionResult> Index(string Searchtext, int? page)
        {
            var post = await _post.GetAllAsync();
            if (page == null)
            {
                page = 1;
            }
            int pageSize = 3;
            int pageNum = page ?? 1;
            if (!string.IsNullOrEmpty(Searchtext))
            {
                post = post.Where(post => post.Alias.Contains(Searchtext) || post.Title.Contains(Searchtext)).ToList();
            }
            return View(post.ToPagedList(pageNum, pageSize));
        }
        [HttpPost]
        public async Task<IActionResult> Add(Post post, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện
                    post.ImageUrl = await SaveImage(imageUrl);
                }
                post.CreateDate = DateTime.Now;
                post.ModifiedDate = DateTime.Now;
                post.Alias = Models.Common.Filter.FilterChar(post.Title);
                await _post.AddAsync(post);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var posts = await _post.GetAllAsync();
            return View(post);
        }
        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName); // Thay đổi đường dẫn theo cấu hình của bạn
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối
        }
        public async Task<IActionResult> Update(int id)
        {
            var post = await _post.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var posts = await _post.GetAllAsync();

            return View(post);
        }
        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                post.ModifiedDate = DateTime.Now;
                post.Alias = Models.Common.Filter.FilterChar(post.Title);
                await _post.UpdateAsync(post);
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _post.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }
        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _post.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
