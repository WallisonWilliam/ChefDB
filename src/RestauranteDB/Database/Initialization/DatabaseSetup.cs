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

        public void DestruirBancoDeDados()
        {
            using (SqlConnection conn = (SqlConnection)db.GetConnection())
            {
                conn.Open();

                // Deleta o banco de dados se ele existir
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "IF EXISTS (SELECT * FROM sys.databases WHERE name = 'RestauranteDB') DROP DATABASE RestauranteDB;";
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Banco de dados 'RestauranteDB' destruído com sucesso!");
            }
        }

        public void RemoverLogins()
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                // Comandos SQL para remover logins de servidor
                string sqlCommands = @"
                IF EXISTS (SELECT * FROM sys.server_principals WHERE name = 'Administrador')
                    DROP LOGIN Administrador;

                IF EXISTS (SELECT * FROM sys.server_principals WHERE name = 'Gerente')
                    DROP LOGIN Gerente;

                IF EXISTS (SELECT * FROM sys.server_principals WHERE name = 'Funcionario')
                    DROP LOGIN Funcionario;
                ";

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sqlCommands;
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Logins removidos com sucesso.");
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
        public void CriarViews()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();

                // View de Clientes e Pontuação Acumulada por Gastos
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "IF OBJECT_ID('ClientePontuacao', 'V') IS NOT NULL DROP VIEW ClientePontuacao;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                    CREATE VIEW ClientePontuacao AS
                    SELECT 
                        c.id AS ClienteID,
                        c.nome AS NomeCliente,
                        SUM(v.valor / 10) AS PontosAcumulados
                    FROM 
                        Cliente c
                    JOIN 
                        Venda v ON c.id = v.id_cliente
                    GROUP BY 
                        c.id, c.nome;";
                    cmd.ExecuteNonQuery();
                }

                // View de Pratos Mais Vendidos (sem ORDER BY)
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "IF OBJECT_ID('PratosMaisVendidos', 'V') IS NOT NULL DROP VIEW PratosMaisVendidos;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                    CREATE VIEW PratosMaisVendidos AS
                    SELECT 
                        p.id AS PratoID,
                        p.nome AS NomePrato,
                        SUM(v.quantidade) AS TotalVendas
                    FROM 
                        Prato p
                    JOIN 
                        Venda v ON p.id = v.id_prato
                    GROUP BY 
                        p.id, p.nome;";
                    cmd.ExecuteNonQuery();
                }

                // View de Ingredientes e Pratos Associados
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "IF OBJECT_ID('IngredientesPratos', 'V') IS NOT NULL DROP VIEW IngredientesPratos;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                    CREATE VIEW IngredientesPratos AS
                    SELECT 
                        i.id AS IngredienteID,
                        i.nome AS NomeIngrediente,
                        p.nome AS NomePrato
                    FROM 
                        Ingredientes i
                    JOIN 
                        Usos u ON i.id = u.id_ingrediente
                    JOIN 
                        Prato p ON u.id_prato = p.id
                    GROUP BY 
                        i.id, i.nome, p.nome;";
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Views criadas com sucesso.");
            }
        }
    }
}

