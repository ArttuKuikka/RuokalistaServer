using RuokalistaServer.Models;

namespace RuokalistaServer.ViewModels
{
    public class IndexViewModel
    {
        public Ruokalista? Ruokalista { get; set; }
        public bool ShowingNextWeeksMenu { get; set; }
        public bool ShowingCurrentWeeksMenu { get; set; }
        public bool NextWeeksMenuExists { get; set; }
    }
}
