using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RuokalistaServer.Models
{
	public class VoteModel
	{
		[Key]
		public int Id { get; set; }
		[ForeignKey("Ruokalista")]
		public int ruokalistaId { get; set; }

		public int level1_votes_maanantai { get; set; }
		public int level2_votes_maanantai { get; set; }
		public int level3_votes_maanantai { get; set; }
		public int level4_votes_maanantai { get; set; }

		public int level1_votes_tiistai { get; set; }
		public int level2_votes_tiistai { get; set; }
		public int level3_votes_tiistai { get; set; }
		public int level4_votes_tiistai { get; set; }

		public int level1_votes_keskiviikko { get; set; }
		public int level2_votes_keskiviikko { get; set; }
		public int level3_votes_keskiviikko { get; set; }
		public int level4_votes_keskiviikko { get; set; }

		public int level1_votes_torstai { get; set; }
		public int level2_votes_torstai { get; set; }
		public int level3_votes_torstai { get; set; }
		public int level4_votes_torstai { get; set; }

		public int level1_votes_perjantai { get; set; }
		public int level2_votes_perjantai { get; set; }
		public int level3_votes_perjantai { get; set; }
		public int level4_votes_perjantai { get; set; }
	}
}
