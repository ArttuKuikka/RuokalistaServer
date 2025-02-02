namespace RuokalistaServer.Models
{
	public class NewUserToken
	{
		public int Id { get; set; }
		public required string Token { get; set; }
		public DateTime Expiration { get; set; }
		public bool isUsed { get; set; }
		public string? UserHint { get; set; }
	}
}
