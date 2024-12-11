using RuokalistaServer.Data.Migrations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RuokalistaServer.Models
{
    public class KasvisRuokalista: IRuokalista
    {
        [JsonIgnore]
        public int Id { get; set; }
		
		public int WeekId { get; set; }
		
		public int Year { get; set; }
		[MaxLength(150)]
		public string? Maanantai { get; set; }
		[MaxLength(150)]
		public string? Tiistai { get; set; }
		[MaxLength(150)]
		public string? Keskiviikko { get; set; }
		[MaxLength(150)]
		public string? Torstai { get; set; }
		[MaxLength(150)]
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
