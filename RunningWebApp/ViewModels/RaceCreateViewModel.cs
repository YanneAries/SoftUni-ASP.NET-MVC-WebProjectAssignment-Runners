using RunningWebApp.Data.Enum;
using RunningWebApp.Models;

namespace RunningWebApp.ViewModels
{
	public class RaceCreateViewModel
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public Address Address { get; set; }
		public IFormFile Image { get; set; }
		public RaceCategory RaceCategory { get; set; }
	}
}
