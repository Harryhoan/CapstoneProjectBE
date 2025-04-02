namespace Application.Commons
{
    public class AppConfiguration
    {
        public string DefaultConnection { get; set; }
        public JWTSection JWTSection { get; set; }
    }
    public class JWTSection
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }

    }
}
