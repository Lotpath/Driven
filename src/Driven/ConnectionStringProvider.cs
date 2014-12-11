using System.Configuration;

namespace Driven
{
    public class ConnectionStringProvider
    {
        public ConnectionStringProvider()
        {
            Master = ConfigurationManager.ConnectionStrings["Master"].ConnectionString;
            Store = ConfigurationManager.ConnectionStrings["Store"].ConnectionString;
        }

        public string Master { get; set; }
        public string Store { get; set; }
    }
}