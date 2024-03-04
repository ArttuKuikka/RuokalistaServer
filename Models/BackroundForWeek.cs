namespace RuokalistaServer.Models
{
	public class BackgroundForWeek
	{
		public int? Id { get; set; }
		public int WeekId { get; set; }
		public int Year { get; set; }
		public string FileName { get; set; }
	}
}
