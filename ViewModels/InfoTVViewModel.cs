using RuokalistaServer.Models;

namespace RuokalistaServer.ViewModels
{
	public class InfoTVViewModel
	{
		public Ruokalista? Ruokalista { get; set; }
		public bool ShowingNextWeeksMenu { get; set; }
		public bool ShowingCurrentWeeksMenu { get; set; }
		public int Week { get; set; }

	}
}
