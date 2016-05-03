using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facade;

namespace Model
{
    public partial class Controller:IHaveID
    {
        public int ID
        {
            get { return this.Id; }
        }
    }

    public partial class Models
    {
        private static Dictionary<string,string>  _connectinsStrings=new Dictionary<string, string>();

        static Models()
        {
            _connectinsStrings["vlad"]= @"Data Source=UMILLY;Initial Catalog=house;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;App=EntityFramework";
        }
        public Models(string connectionString) : base(BuildEntityConnectionStringFromAppSettings(connectionString))
        {
        }

        public static string BuildEntityConnectionStringFromAppSettings(string nameOfConnectionString)
        {
            var shortConnectionString = _connectinsStrings[nameOfConnectionString];

            // Specify the provider name, server and database. 
            string providerName = "System.Data.SqlClient";

            // Initialize the connection string builder for the 
            // underlying provider taking the short connection string.
            SqlConnectionStringBuilder sqlBuilder =
                new SqlConnectionStringBuilder(shortConnectionString);

            // Set the properties for the data source.
            sqlBuilder.IntegratedSecurity = true;

            // Build the SqlConnection connection string. 
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
                new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata =
                String.Format(
                    "res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl",
                    "ApplicationDB");
            return entityBuilder.ToString();
        }
    }
}
