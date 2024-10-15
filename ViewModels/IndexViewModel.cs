using RuokalistaServer.Models;

namespace RuokalistaServer.ViewModels
{
    public class IndexViewModel
    {
        public IRuokalista? Ruokalista { get; set; }
        public bool ShowingNextWeeksMenu { get; set; }
        public bool ShowingCurrentWeeksMenu { get; set; }
        public bool NextWeeksMenuExists { get; set; }
        public bool KasviruokalistaExists { get; set; }
        public bool ShowingKasvisruokalista { get; set; }
    }
}
