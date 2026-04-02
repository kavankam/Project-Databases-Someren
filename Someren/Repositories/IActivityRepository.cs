using System.Collections.Generic;
using System.Threading.Tasks;

namespace Someren.Models;

public interface IActivityRepository
{
    public Activity? GetById(int id);
    IEnumerable<Activity> GetAll();
    void Add(Activity activity);
    void Update(Activity activity);
    void Delete(Activity activity);
    bool SaveChanges();
    
}