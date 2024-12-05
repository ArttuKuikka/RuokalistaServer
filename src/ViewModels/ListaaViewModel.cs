using RuokalistaServer.Models;

namespace RuokalistaServer.ViewModels
{
    public class ListaaViewModel
    {
        public List<Ruokalista> Ruokalistat { get; set; }
        public List <KasvisRuokalista>? KasvisRuokalistat { get; set; }
    }
}
