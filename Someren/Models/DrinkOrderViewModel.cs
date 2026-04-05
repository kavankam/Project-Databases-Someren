namespace Someren.Models;

public class DrinkOrderViewModel
{
    public List<Student> Students { get; set; }
    public List<Drink> Drinks { get; set; }
    public int StudentID { get; set; }
    public int DrinkID { get; set; }
    public int Quantity { get; set; }
    
    public Student? SelectedStudent { get; set; }
    public Drink? SelectedDrink { get; set; }

    public DrinkOrderViewModel()
    {
        Students = new List<Student>();
        Drinks = new List<Drink>();
        Quantity = 1;
    }
}