using Someren.Models;

namespace Someren.Repositories;

public interface IDrinkRepository
{
    List<Drink> GetAllDrinks();
    Drink? GetById(int id);
    void UpdateStock(int drinkId, int newStock);
}