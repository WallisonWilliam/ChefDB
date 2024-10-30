using System;
using System.Data.SqlClient;

namespace RestauranteDB.Database
{
    public class RestauranteSystem
    {
        private DatabaseConnection db = new DatabaseConnection();

        // Metodo para criar o banco de dados se não existir
        public void CriarBancoDeDados()
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RestauranteDB') CREATE DATABASE RestauranteDB;", conn);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Banco de dados 'RestauranteDB' criado com sucesso ou já existente.");
            }
        }

        // Metodo para destruir o banco de dados
        public void DestruirBancoDeDados()
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("IF EXISTS (SELECT * FROM sys.databases WHERE name = 'RestauranteDB') DROP DATABASE RestauranteDB;", conn);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Banco de dados 'RestauranteDB' destruído com sucesso!");
            }
        }

        // Metodo para criar as tabelas no banco de dados
        public void CriarTabelas()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();
                string createTables = @"
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

        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Ingrediente' AND xtype='U')
        CREATE TABLE Ingrediente (
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
            FOREIGN KEY (id_ingrediente) REFERENCES Ingrediente(id)
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

                SqlCommand cmd = new SqlCommand(createTables, conn);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Tabelas criadas com sucesso ou já existentes.");
            }
        }


        // Metodo para inserir dados iniciais evitando duplicacoes
        public void InserirDadosIniciais()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();
                // Limpa os dados existentes nas tabelas para evitar duplicacao
                SqlCommand clearCmd = new SqlCommand("DELETE FROM Venda; DELETE FROM Usos; DELETE FROM Ingrediente; DELETE FROM Fornecedor; DELETE FROM Prato; DELETE FROM Cliente;", conn);
                clearCmd.ExecuteNonQuery();

                for (int i = 1; i <= 10; i++)
                {
                    // Clientes
                    SqlCommand cmd = new SqlCommand("INSERT INTO Cliente (id, nome, sexo, idade, nascimento, pontos) VALUES (@id, @nome, @sexo, @idade, @nascimento, @pontos)", conn);
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Parameters.AddWithValue("@nome", $"Cliente{i}");
                    cmd.Parameters.AddWithValue("@sexo", i % 2 == 0 ? "m" : "f");
                    cmd.Parameters.AddWithValue("@idade", 20 + i);
                    cmd.Parameters.AddWithValue("@nascimento", DateTime.Now.AddYears(-20 - i));
                    cmd.Parameters.AddWithValue("@pontos", 0);
                    cmd.ExecuteNonQuery();

                    // Pratos
                    cmd = new SqlCommand("INSERT INTO Prato (id, nome, descricao, valor, disponibilidade) VALUES (@id, @nome, @descricao, @valor, @disponibilidade)", conn);
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Parameters.AddWithValue("@nome", $"Prato{i}");
                    cmd.Parameters.AddWithValue("@descricao", $"Descrição do Prato {i}");
                    cmd.Parameters.AddWithValue("@valor", 20.0m + i);
                    cmd.Parameters.AddWithValue("@disponibilidade", true);
                    cmd.ExecuteNonQuery();

                    // Fornecedores
                    cmd = new SqlCommand("INSERT INTO Fornecedor (id, nome, estado_origem) VALUES (@id, @nome, @estado)", conn);
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Parameters.AddWithValue("@nome", $"Fornecedor{i}");
                    cmd.Parameters.AddWithValue("@estado", "SP");
                    cmd.ExecuteNonQuery();

                    // Ingredientes
                    cmd = new SqlCommand("INSERT INTO Ingrediente (id, nome, data_fabricacao, data_validade, quantidade, observacao) VALUES (@id, @nome, @data_fabricacao, @data_validade, @quantidade, @observacao)", conn);
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Parameters.AddWithValue("@nome", $"Ingrediente{i}");
                    cmd.Parameters.AddWithValue("@data_fabricacao", DateTime.Now.AddMonths(-1));
                    cmd.Parameters.AddWithValue("@data_validade", DateTime.Now.AddMonths(1));
                    cmd.Parameters.AddWithValue("@quantidade", 100);
                    cmd.Parameters.AddWithValue("@observacao", $"Observação {i}");
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("10 elementos cadastrados em cada tabela!");
            }
        }

        public void CriarTriggers()
        {
            CriarTriggerPontos();/*
            CriarTriggerIngredientesVencidos();
            CriarTriggerCompraIndisponivel();
            CriarTriggerVendaProduto();*/
        }

        // Trigger para pontos de cliente e recria se ja existir
        public void CriarTriggerPontos()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();

                // Exclui o trigger se ele ja existir
                string dropTriggerSQL = @"IF OBJECT_ID('trg_CalculatePoints', 'TR') IS NOT NULL 
                                      DROP TRIGGER trg_CalculatePoints;";
                SqlCommand dropCmd = new SqlCommand(dropTriggerSQL, conn);
                dropCmd.ExecuteNonQuery();

                // Cria o trigger novamente
                string triggerSQL = @"
            CREATE TRIGGER trg_CalculatePoints
            ON Venda
            AFTER INSERT
            AS
            BEGIN
                UPDATE Cliente
                SET pontos = pontos + (INSERTED.valor / 10)
                FROM Cliente, INSERTED
                WHERE Cliente.id = INSERTED.id_cliente;
            END;
            ";
                SqlCommand cmd = new SqlCommand(triggerSQL, conn);
                cmd.ExecuteNonQuery();

                Console.WriteLine("Trigger de pontos criada com sucesso!");
            }
        }
    }
}
