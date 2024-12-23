using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RuokalistaServer.Models
{
    public interface IRuokalista
    {
        [JsonIgnore]
        public int Id { get; set; }
		[Range(1, 54, ErrorMessage = "Viikko ei voi olla pienempi kuin 1 tai suurempi kuin 54")]
		public int WeekId { get; set; }
		[Range(2020, 2100, ErrorMessage = "Virheellinen vuosiluku")]
		public int Year { get; set; }
		[MaxLength(150)]
		public string Maanantai { get; set; }
		[MaxLength(150)]
		public string Tiistai { get; set; }
		[MaxLength(150)]
		public string Keskiviikko { get; set; }
		[MaxLength(150)]
		public string Torstai { get; set; }
		[MaxLength(150)]
		public string Perjantai { get; set; }
    }
}
