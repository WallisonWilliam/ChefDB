using System;
using System.Data.SqlClient;

namespace RestauranteDB.Database
{
    public class DataSeeder
    {
        private readonly DatabaseConnection db = new DatabaseConnection();

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
    }
}
