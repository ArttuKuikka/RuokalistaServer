namespace RuokalistaServer.Models
{
	public class ÄänestysTulos
	{
		public Ruokalista ruokalista { get; set; }
		public VoteModel? votes { get; set; }
		public bool isCurrentWeek {  get; set; }
		public int currentDay { get; set; }
	}
}
