using Microsoft.Data.SqlClient;
using Someren.Models;

namespace Someren.Repositories
{
    public interface IActivityRepository
    {
        List<Activity> GetAll(string? searchTerm); //with a seachterm

        void Add (Activity activity);

        Activity? GetById(int id); //find activity using id 

        void Update (Activity activity);

        void Delete (int id);

        bool ActivityNameExists(string name, int? excludedId = null); /*stops the database from doing bool yes 
                                                                        when editing an existing activity*/
    }
}
