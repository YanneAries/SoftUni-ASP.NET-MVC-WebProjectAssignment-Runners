using Microsoft.EntityFrameworkCore;
using RunningWebApp.Data;
using RunningWebApp.Interfaces;
using RunningWebApp.Models;

namespace RunningWebApp.Repository
{
	public class RaceRepository : IRaceRepository
	{
		private readonly ApplicationDbContext context;
		public RaceRepository(ApplicationDbContext context)
		{
			this.context = context;
		}

		public bool Add(Race race)
		{
			context.Add(race);
			return Save();
		}

		public bool Delete(Race race)
		{
			context.Remove(race);
			return Save();
		}

		public async Task<IEnumerable<Race>> GetAll()
		{
			return await context.Races.ToListAsync();
		}

		public async Task<IEnumerable<Race>> GetAllRacesByCity(string city)
		{
			return await context.Races.Where(c => c.Address.City.Contains(city)).ToListAsync();
		}

		public async Task<Race> GetByIdAsync(int id)
		{
			return await context.Races.Include(i => i.Address).FirstOrDefaultAsync(i => i.Id == id);
		}

		public bool Save()
		{
			var saved = context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public bool Update(Race race)
		{
			context.Update(race);
			return Save();
		}
	}
}
