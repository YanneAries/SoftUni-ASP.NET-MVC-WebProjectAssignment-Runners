using Microsoft.EntityFrameworkCore;
using RunningWebApp.Data;
using RunningWebApp.Interfaces;
using RunningWebApp.Models;

namespace RunningWebApp.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public DashboardRepository(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Club>> GetAllUserClubs()
        {
            var currentUser = httpContextAccessor.HttpContext?.User.GetUserId();
            var userClubs = context.Clubs.Where(r => r.AppUser.Id == currentUser);
            return await userClubs.ToListAsync();
        }

        public async Task<List<Race>> GetAllUserRaces()
        {
            var currentUser = httpContextAccessor.HttpContext?.User.GetUserId();
            var userRaces = context.Races.Where(r => r.AppUser.Id == currentUser);
            return await userRaces.ToListAsync();
        }

        public async Task<AppUser> GetUserById(string id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetByIdNoTracking(string id)
        {
            return await context.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public bool Update(AppUser user)
        {
            context.Users.Update(user);
            return Save();
        }

        public bool Save()
        {
            var saved = context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
