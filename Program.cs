using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler();
}
app.UseStatusCodePages();

List<Fruit> fruits = new List<Fruit>();
int globalId = 0;

app.MapGet("/", () => "Hello World!");
app.MapGet("/fruits", () => Results.Ok(fruits));
app.MapGet("/fruits/{id}", (int id) => GetFruit(id, fruits));
app.MapPost("/create_fruit/{title}", (string title) => CreateFruit(ref globalId, title, fruits));
app.MapDelete("/delete_fruit/{title}", (string title) => DeleteFruit(title, fruits));

app.Run("http://localhost:3000/");

IResult CreateFruit(ref int id, string title, List<Fruit> fruits)
{
  Fruit? fruit = fruits.Find(fruit => fruit.Title == title);

  if (fruit == null)
  {
    Fruit new_fruit = new Fruit(++id, title);
    fruits.Add(new_fruit);
    return Results.Created($"/fruits/{id}", new_fruit);
  }
  else
  {
    return Results.Problem(statusCode: 400);
  }
 
};

IResult GetFruit(int id, List<Fruit> fruits)
{
  Fruit? fruit = fruits.Find(fruit => fruit.Id == id);

  if (fruit == null)
  {
    return Results.NoContent();
  }
  else
  {
    return Results.Ok(fruit);
  }
};

IResult DeleteFruit(string title, List<Fruit> fruits)
{
  Fruit? fruit = fruits.Find( fruit => fruit.Title == title);
  if (fruit == null)
  {
    return Results.NoContent();
  }
  else
  {
    fruits.Remove(fruit);
    return Results.Ok($"{title} was deleted from database");
  }
}

class Fruit
{
  public int Id {get; set;}
  public string Title {get; set;}
  public int Stock {get; set;}
  public Fruit(int id, string title)
  {
    this.Id = id;
    this.Title = title;
    this.Stock = new Random().Next(100);
  }
}
