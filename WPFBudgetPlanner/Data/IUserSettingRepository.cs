using System.Threading.Tasks;
using WPFBudgetPlanner.Models;

namespace WPFBudgetPlanner.Data;

public interface IUserSettingRepository
{
    Task<UserSetting?> GetAsync();
    Task UpsertAsync(UserSetting settings);
    Task<int> SaveChangesAsync();
}