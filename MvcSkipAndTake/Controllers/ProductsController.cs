﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AWMvcPaging.Models;

namespace AWMvcPaging.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AdventureWorksLT2012Context _context;

        public ProductsController(AdventureWorksLT2012Context context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string sortOrder,  int? pageIndex)
        {
            ViewData["CategorySortParam"] = String.IsNullOrEmpty(sortOrder) ? "Category_desc" : "";
            ViewData["ModelSortParam"] = sortOrder == "Model" ? "Model_desc" : "Model";
            ViewData["NameSortParam"] = sortOrder == "Name" ? "Name_desc" : "Name";

            var products = _context.Product
                 .Include(p => p.ProductCategory)
                 .Include(p => p.ProductModel)
                 .Where(p => p.ThumbnailPhotoFileName != "no_image_available_small.gif");

            switch (sortOrder)
            {
                case "Category_desc":
                    products =
                       products.OrderByDescending(p => p.ProductCategory.Name);
                    break;
                case "Model":
                    products = products.OrderBy(p => p.ProductModel.Name);
                    break;
                case "Model_desc":
                    products = products.OrderByDescending(p => p.ProductModel.Name);
                    break;
                case "Name":
                    products = products.OrderBy(p => p.Name);
                    break;
                case "Name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductCategory.Name);
                    break;
            }

            int pageSize = 10;

            return View(await PaginatedList<Product>.CreateAsync(products.AsNoTracking(), pageIndex ?? 1, pageSize));


            //return View(await products.AsNoTracking().ToListAsync());


            //var adventureWorksLT2012Context = _context.Product
            //    .Include(p => p.ProductCategory)
            //    .Include(p => p.ProductModel)
            //    .Where(p => p.ThumbnailPhotoFileName != "no_image_available_small.gif")
            //    .OrderBy(p => p.ProductCategory.Name)
            //    .ThenBy(p => p.ProductModel.Name)
            //    .ThenBy(p => p.Name);
            //return View(await adventureWorksLT2012Context.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductModel)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategory, "ProductCategoryId", "Name");
            ViewData["ProductModelId"] = new SelectList(_context.ProductModel, "ProductModelId", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,ProductNumber,Color,StandardCost,ListPrice,Size,Weight,ProductCategoryId,ProductModelId,SellStartDate,SellEndDate,DiscontinuedDate,ThumbNailPhoto,ThumbnailPhotoFileName,Rowguid,ModifiedDate")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategory, "ProductCategoryId", "Name", product.ProductCategoryId);
            ViewData["ProductModelId"] = new SelectList(_context.ProductModel, "ProductModelId", "Name", product.ProductModelId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategory, "ProductCategoryId", "Name", product.ProductCategoryId);
            ViewData["ProductModelId"] = new SelectList(_context.ProductModel, "ProductModelId", "Name", product.ProductModelId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,ProductNumber,Color,StandardCost,ListPrice,Size,Weight,ProductCategoryId,ProductModelId,SellStartDate,SellEndDate,DiscontinuedDate,ThumbNailPhoto,ThumbnailPhotoFileName,Rowguid,ModifiedDate")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductCategoryId"] = new SelectList(_context.ProductCategory, "ProductCategoryId", "Name", product.ProductCategoryId);
            ViewData["ProductModelId"] = new SelectList(_context.ProductModel, "ProductModelId", "Name", product.ProductModelId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductModel)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
