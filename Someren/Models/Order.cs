namespace Someren.Models;

public class Order
{
    public int StudentID { get; set; }
    public int DrinkID { get; set; }
    public int Quantity { get; set; }

    public Order()
    {
    }

    public Order(int studentID, int drinkID, int quantity)
    {
        StudentID = studentID;
        DrinkID = drinkID;
        Quantity = quantity;
    }
}