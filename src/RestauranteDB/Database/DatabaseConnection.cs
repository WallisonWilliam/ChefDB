using System.Data.SqlClient;

namespace RestauranteDB.Database
{
    public class DatabaseConnection
    {
        private string connectionString = "Server=localhost\\SQLEXPRESS;Database=master;Integrated Security=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // Metodo para obter uma conexao especifica ao banco RestauranteDB
        public SqlConnection GetDatabaseConnection()
        {
            return new SqlConnection("Server=localhost\\SQLEXPRESS;Database=RestauranteDB;Integrated Security=True;");
        }
    }
}
