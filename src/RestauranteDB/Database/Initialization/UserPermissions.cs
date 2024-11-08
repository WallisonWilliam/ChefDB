using System;
using System.Data.SqlClient;

namespace RestauranteDB.Database
{
    public class UserPermissions
    {
        private readonly DatabaseConnection db;

        public UserPermissions(DatabaseConnection dbConnection)
        {
            db = dbConnection;
        }

        public void SetupUsersAndPermissions()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();

                string sqlCommands = @"
                        IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'Administrador')
                        BEGIN
                            CREATE LOGIN Administrador WITH PASSWORD = '12345!';
                            USE RestauranteDB;
                            CREATE USER Administrador FOR LOGIN Administrador;
                            EXEC sp_addrolemember 'db_owner', 'Administrador';
                        END;

                        IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'Gerente')
                        BEGIN
                            CREATE LOGIN Gerente WITH PASSWORD = '12345';
                            USE RestauranteDB;
                            CREATE USER Gerente FOR LOGIN Gerente;
                            GRANT SELECT, UPDATE, DELETE ON SCHEMA :: dbo TO Gerente;
                        END;

                        IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'Funcionario')
                        BEGIN
                            CREATE LOGIN Funcionario WITH PASSWORD = '12345';
                            USE RestauranteDB;
                            CREATE USER Funcionario FOR LOGIN Funcionario;
                            GRANT SELECT ON dbo.Venda TO Funcionario;
                            GRANT INSERT ON SCHEMA :: dbo TO Funcionario;
                        END;
                    ";

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommands;
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Usuarios e permissoes configurados com sucesso.");
            }
        }
    }
}
