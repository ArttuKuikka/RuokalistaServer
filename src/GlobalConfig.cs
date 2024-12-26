namespace RuokalistaServer
{
    public static class GlobalConfig
    {
        public static bool IGEnabled = string.Equals(Environment.GetEnvironmentVariable("IG_ENABLED"), "true", StringComparison.OrdinalIgnoreCase);
        public static bool InfotvEnabled = string.Equals(Environment.GetEnvironmentVariable("Infotv_ENABLED"), "true", StringComparison.OrdinalIgnoreCase);
        public static bool AanestysEnabled = string.Equals(Environment.GetEnvironmentVariable("Aanestys_ENABLED"), "true", StringComparison.OrdinalIgnoreCase);
        public static bool APIEnabled = string.Equals(Environment.GetEnvironmentVariable("API_ENABLED"), "true", StringComparison.OrdinalIgnoreCase);
        public static bool KasvisruokalistaEnabled = string.Equals(Environment.GetEnvironmentVariable("Kasvisruokalista_ENABLED"), "true", StringComparison.OrdinalIgnoreCase);

        public static string PrimaryColor = Environment.GetEnvironmentVariable("PrimaryColor") ?? "#ffa500";

        public static string RootUser = Environment.GetEnvironmentVariable("RootUser") ?? "";

        public static string BrandingName = Environment.GetEnvironmentVariable("Branding") ?? "Kouluruokalista.fi";

        public static string? StaticContentHost = Environment.GetEnvironmentVariable("StaticContentHost");

        public static string DefaultLanguage = Environment.GetEnvironmentVariable("DefaultLanguage") ?? "fin";
	}
}
