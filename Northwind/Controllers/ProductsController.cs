using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Northwind.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Northwind.Controllers;

public class ProductsController : Controller
{
    private readonly string baseUrl;
    private readonly string appJson;

        
      

    public async Task<List<Category>> GetCategoriesAsync()
    {
        var categories = new List<Category>();
        try
        {
            using (var client = new HttpClient())
            {
                ConfigClient(client);
                var response = await client.GetAsync("categories");
                if(response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    categories = JsonSerializer.Deserialize<List<Category>>(json);
                }
            }
                
        }

        catch (Exception e)
        {
            ViewBag.ErrorMessage = e.Message;
        }

        return categories;
    }



    public async Task<Product> GetProduct(int id)
    {
        Product product = new Product();
        try
        {
            using (var client = new HttpClient())
            {
                
                ConfigClient(client);
                var response = await client.GetAsync($"Products/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    product = JsonSerializer.Deserialize<Product>(json);
                }
            }

        }

        catch (Exception e)
        {
            ViewBag.ErrorMessage = e.Message;
        }

        return product; 
    }


    public ProductsController(IConfiguration config) {
        baseUrl = config.GetValue<string>("BaseUrl");
        appJson = config.GetValue<string>("AppJson");
        
    }


    private void ConfigClient(HttpClient client)
    {
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(appJson));
        client.BaseAddress = new Uri(baseUrl);
    }




    // GET: ProductsController
    [AllowAnonymous]
    public async Task<ActionResult> Index(int? CategoryId = 1)
    {
        try
        {
            var Categories = await GetCategoriesAsync();

            using (var client = new HttpClient())
            {
                ConfigClient(client);

                var response = await client.GetAsync($"/api/Products/ByCategory/{CategoryId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var Products = JsonSerializer.Deserialize<List<Product>>(json);
                    ViewBag.categoryID = new SelectList(Categories.ToList(), nameof(Category.categoryId), nameof(Category.categoryName));
                    if(CategoryId != 0)
                    {
                        return View(Products);
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = response.StatusCode;
                    return View("Error"); 
                }
            }

            
            return View();
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Error: " + ex.Message;
            return View("Error"); // Return the Error view in case of an exception
        }
    }



    // GET: ProductsController/Details/5
    //[AllowAnonymous]
    [Authorize]
    public async Task<ActionResult> Details(int id)
    {
        try
        {
            Product product = await GetProduct(id);
            if (product == null)
            {
                ViewBag.ErrorMessage = $"Product id {id} not found.";
                return View("Error");
            }

            List<Category> Categories = await GetCategoriesAsync();
            Category productCategory = Categories.FirstOrDefault(c => c.categoryId == product.categoryId);

            if (productCategory != null)
            {
                ViewBag.CategoryName = productCategory.categoryName;
                ViewBag.categoryID = product.categoryId;
                ViewBag.ProductName = product.productName;
                return View(product); 
            }
            else
            {
                ViewBag.ErrorMessage = $"Product id {id} not found.";
                return View("Error");
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }



}
