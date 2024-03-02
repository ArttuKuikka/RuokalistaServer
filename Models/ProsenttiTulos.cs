namespace RuokalistaServer.Models
{
    public class ProsenttiTulos
    {
        public Ruokalista ruokalista { get; set; }
        public ViikonProsentit prosentit { get; set; }
    }

    public class ViikonProsentit
    {
        public double maanantai { get; set; }
        public double tiistai { get; set; }
        public double keskiviikko { get; set; }
        public double torstai { get; set; }
        public double perjantai { get; set; }
    }
}
