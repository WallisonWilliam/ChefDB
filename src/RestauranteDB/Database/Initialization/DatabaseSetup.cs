using System;
using System.Data.SqlClient;

namespace RestauranteDB.Database
{
    public class DatabaseSetup
    {
        private readonly DatabaseConnection db;

        public DatabaseSetup(DatabaseConnection dbConnection)
        {
            db = dbConnection;
        }

        // Metodo para criar o banco de dados se nao existir
        public void CriarBancoDeDados()
        {
            using (SqlConnection conn = (SqlConnection)db.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RestauranteDB') CREATE DATABASE RestauranteDB;";
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Banco de dados 'RestauranteDB' criado com sucesso ou já existente.");
            }
        }

        // Metodo para destruir o banco de dados
        public void DestruirBancoDeDados()
        {
            using (SqlConnection conn = (SqlConnection)db.GetConnection())
            {
                conn.Open();

                // Força a desconexão de todos os usuários
                using (SqlCommand disconnectCmd = conn.CreateCommand())
                {
                    disconnectCmd.CommandText = "ALTER DATABASE RestauranteDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                    disconnectCmd.ExecuteNonQuery();
                }

                // Deleta o banco de dados se ele existir
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "IF EXISTS (SELECT * FROM sys.databases WHERE name = 'RestauranteDB') DROP DATABASE RestauranteDB;";
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Banco de dados 'RestauranteDB' destruído com sucesso!");
            }
        }

        public void CriarTabelas()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cliente' AND xtype='U')
                    CREATE TABLE Cliente (
                        id INT PRIMARY KEY, 
                        nome NVARCHAR(50) NOT NULL, 
                        sexo CHAR(1) CHECK (sexo IN ('m', 'f', 'o')), 
                        idade INT NOT NULL, 
                        nascimento DATE NOT NULL, 
                        pontos INT DEFAULT 0
                    );

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Prato' AND xtype='U')
                    CREATE TABLE Prato (
                        id INT PRIMARY KEY, 
                        nome NVARCHAR(50) NOT NULL, 
                        descricao NVARCHAR(200), 
                        valor DECIMAL(10, 2), 
                        disponibilidade BIT
                    );

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Fornecedor' AND xtype='U')
                    CREATE TABLE Fornecedor (
                        id INT PRIMARY KEY, 
                        nome NVARCHAR(50) NOT NULL, 
                        estado_origem NVARCHAR(50) NOT NULL
                    );

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Ingredientes' AND xtype='U')
                    CREATE TABLE Ingredientes (
                        id INT PRIMARY KEY, 
                        nome NVARCHAR(50) NOT NULL, 
                        data_fabricacao DATE NOT NULL, 
                        data_validade DATE NOT NULL, 
                        quantidade INT NOT NULL, 
                        observacao NVARCHAR(100) NULL
                    );

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Usos' AND xtype='U')
                    CREATE TABLE Usos (
                        id_prato INT, 
                        id_ingrediente INT, 
                        FOREIGN KEY (id_prato) REFERENCES Prato(id), 
                        FOREIGN KEY (id_ingrediente) REFERENCES Ingredientes(id)
                    );

                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Venda' AND xtype='U')
                    CREATE TABLE Venda (
                        id INT PRIMARY KEY, 
                        id_cliente INT, 
                        id_prato INT, 
                        quantidade INT, 
                        dia DATE, 
                        hora TIME, 
                        valor DECIMAL(10, 2),
                        FOREIGN KEY (id_cliente) REFERENCES Cliente(id), 
                        FOREIGN KEY (id_prato) REFERENCES Prato(id)
                    );";
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Tabelas criadas com sucesso ou já existentes.");
            }
        }
    }
}
