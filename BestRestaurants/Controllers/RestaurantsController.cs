using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using BestRestaurants.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BestRestaurants.Controllers;

public class RestaurantsController : Controller
{
  private readonly BestRestaurantsContext _db;

  public RestaurantsController(BestRestaurantsContext db)
  {
    _db = db;
  }

  private async Task<List<Restaurant>> SearchMethod(string searchQuery)
  {
    IQueryable<Restaurant> restaurantsList = _db.Set<Restaurant>();

    if (searchQuery != null)
    {
      return await restaurantsList?.Where(restaurant => restaurant.RestaurantName.Contains(searchQuery) || restaurant.Description.Contains(searchQuery) || restaurant.Cuisine.Type.Contains(searchQuery) ).ToListAsync();
    }
    else
    {
      return await restaurantsList.ToListAsync();
    }
  }

  // public ActionResult Index()
  // {
  //   List<Restaurant> model = _db.Restaurant.ToList();
  //   ViewBag.PageTitle = "View All Restaurants";
  //   return View(model);
  // }

  public async Task<IActionResult> Index(string searchQuery)
  {
    List<Restaurant> resultList = await SearchMethod(searchQuery);
    ViewBag.PageTitle = "View Restaurants";
    return View(resultList);
  }


  public ActionResult Create()
  {
    ViewBag.CuisineId = new SelectList(_db.Cuisine, "CuisineId", "Type");
    return View();
  }

  [HttpPost]
  public ActionResult Create(Restaurant restaurant)
  {
    if (restaurant.CuisineId ==0)
    {
      return RedirectToAction("Create");
    }
  
    _db.Restaurant.Add(restaurant);
    _db.SaveChanges();
    return RedirectToAction("Index");
  }

  public ActionResult Details(int id)
  {
    Restaurant selectedRestaurant = _db.Restaurant
                                        .Include(shop => shop.Cuisine)
                                        .FirstOrDefault(shop => shop.RestaurantId == id);
    ViewBag.PageTitle = $"Details - {selectedRestaurant.RestaurantName}";
    return View(selectedRestaurant);
  }
  public ActionResult Edit(int id)
  {
    Restaurant thisRestaurant = _db.Restaurant.FirstOrDefault(shop => shop.RestaurantId == id);
    ViewBag.CuisineId = new SelectList(_db.Cuisine, "CuisineId", "Type");
    return View(thisRestaurant);
  }

  [HttpPost]
  public ActionResult Edit(Restaurant restaurant)  
  
  {
    _db.Restaurant.Update(restaurant);
    _db.SaveChanges();
    return RedirectToAction("Index");
  }

  public ActionResult Delete(int id)
  {
    Restaurant thisRestaurant = _db.Restaurant.FirstOrDefault(restaurant => restaurant.RestaurantId == id);
    ViewBag.PageTitle = $"Confirm Delete - {thisRestaurant.RestaurantName}";
    return View(thisRestaurant);
  }

  [HttpPost, ActionName("Delete")]
  public ActionResult DeleteConfirmed(int id)
  {
    Restaurant thisRestaurant = _db.Restaurant.FirstOrDefault(restaurant => restaurant.RestaurantId == id);
    _db.Restaurant.Remove(thisRestaurant);
    _db.SaveChanges();
    return RedirectToAction("Index");
  }
}