namespace Someren.Models;

public class Drink
{
    public int DrinkID { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsAlcoholic { get; set; }

    public Drink()
    {
        Name = string.Empty;
    }

    public Drink(int drinkID, string name, decimal price, int stock, bool isAlcoholic)
    {
        DrinkID = drinkID;
        Name = name;
        Price = price;
        Stock = stock;
        IsAlcoholic = isAlcoholic;
    }
}