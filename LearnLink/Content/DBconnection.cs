using System;
using System.Configuration;

namespace LearnLink.Content
{
    public static class DBconnection
    {
        public static string connStr = Init();

        private static string Init()
        {
            var fromConfig = ConfigurationManager.ConnectionStrings["LearnLinkDb"]?.ConnectionString;
            var fromEnv = Environment.GetEnvironmentVariable("LEARNLINK_CONNSTR");
            var fallback = "Data Source=DAREDEVIL\\SQLEXPRESS; Initial Catalog=learnlink; Integrated Security=True; TrustServerCertificate=True";
            if (!string.IsNullOrWhiteSpace(fromConfig)) return fromConfig;
            if (!string.IsNullOrWhiteSpace(fromEnv)) return fromEnv;
            return fallback;
        }
    }
}
