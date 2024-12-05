using RuokalistaServer.Models;

namespace RuokalistaServer.ViewModels
{
    public class IGViewModel
    {
        public IRuokalista Ruokalista { get; set; }
        public string Päivät { get; set; }

        public bool ShowingKasvisruokalista { get; set; }
	}
}
