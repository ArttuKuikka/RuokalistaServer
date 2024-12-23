using RuokalistaServer.Models;

namespace RuokalistaServer.ViewModels
{
	public class InfoTVViewModel
	{
		public IRuokalista? Ruokalista { get; set; }
		public bool ShowingNextWeeksMenu { get; set; }
		public bool ShowingCurrentWeeksMenu { get; set; }
		public bool ShowingKasvisruokalista { get; set; }
        public int Week { get; set; }

	}
}
