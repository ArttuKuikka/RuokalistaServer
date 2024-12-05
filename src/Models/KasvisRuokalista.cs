using RuokalistaServer.Data.Migrations;
using System.Text.Json.Serialization;

namespace RuokalistaServer.Models
{
    public class KasvisRuokalista: IRuokalista
    {
        [JsonIgnore]
        public int Id { get; set; }
        public int WeekId { get; set; }
        public int Year { get; set; }
        public string? Maanantai { get; set; }
        public string? Tiistai { get; set; }
        public string? Keskiviikko { get; set; }
        public string? Torstai { get; set; }
        public string? Perjantai { get; set; }

        public static bool IsNull(KasvisRuokalista kasvisRuokalista)
        {
            return
            kasvisRuokalista.Maanantai == null &&
            kasvisRuokalista.Tiistai == null &&
            kasvisRuokalista.Keskiviikko == null &&
            kasvisRuokalista.Torstai == null &&
            kasvisRuokalista.Perjantai == null;
        }
    }

}
