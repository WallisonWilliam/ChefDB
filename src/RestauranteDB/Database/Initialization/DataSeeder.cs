using System;
using System.Data.SqlClient;

namespace RestauranteDB.Database
{
    public class DataSeeder
    {
        private readonly DatabaseConnection db;

        public DataSeeder(DatabaseConnection dbConnection)
        {
            db = dbConnection;
        }

        // Metodo para inserir dados iniciais evitando duplicacoes
        public void InserirDadosIniciais()
        {
            using (SqlConnection conn = (SqlConnection)db.GetDatabaseConnection())
            {
                conn.Open();

                // Limpa os dados existentes nas tabelas para evitar duplicacao
                using (SqlCommand clearCmd = conn.CreateCommand())
                {
                    clearCmd.CommandText = "DELETE FROM Venda; DELETE FROM Usos; DELETE FROM Ingredientes; DELETE FROM Fornecedor; DELETE FROM Cliente;";
                    clearCmd.ExecuteNonQuery();
                }

                InserirClientes(conn);
                InserirPratos(conn);
                InserirFornecedores(conn);
                InserirIngredientes(conn);
                InserirUsos(conn);
                InserirVendas(conn);

                Console.WriteLine("10 elementos cadastrados em cada tabela!");
            }
        }

        private void InserirClientes(SqlConnection conn)
        {
            string[] nomesClientes = { "Joao Silva", "Maria Oliveira", "Pedro Souza", "Ana Lima", "Carlos Pereira", "Mariana Castro", "Lucas Mendes", "Fernanda Rocha", "Paulo Alves", "Julia Costa" };
            char[] sexoClientes = { 'm', 'f', 'm', 'f', 'm', 'f', 'm', 'f', 'm', 'f' };

            for (int i = 1; i <= 10; i++)
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Cliente (id, nome, sexo, idade, nascimento, pontos) VALUES (@id, @nome, @sexo, @idade, @nascimento, @pontos)";
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Parameters.AddWithValue("@nome", nomesClientes[i - 1]);
                    cmd.Parameters.AddWithValue("@sexo", sexoClientes[i - 1]);
                    cmd.Parameters.AddWithValue("@idade", 20 + i);
                    cmd.Parameters.AddWithValue("@nascimento", DateTime.Now.AddYears(-20 - i));
                    cmd.Parameters.AddWithValue("@pontos", 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InserirPratos(SqlConnection conn)
        {
            string[] nomesPratos = { "Feijoada Completa", "Picanha Grelhada", "Moqueca de Peixe", "Hamburguer Artesanal", "Pizza Marguerita", "Combinado Sushi", "Tacos Mexicanos", "Salada Caesar", "Lasanha Bolonhesa", "Espaguete ao Alho e Oleo" };
            string[] descricaoPratos = {
                "Feijoada com carne seca, linguica, costela e acompanhamentos.",
                "Picanha grelhada ao ponto, acompanhada de arroz e batatas fritas.",
                "Moqueca de peixe com leite de coco, pimentoes e dende.",
                "Hamburguer artesanal com creme gorgonzola, geleia de pimenta, onion rings e bacon.",
                "Pizza marguerita com molho de tomate, mussarela e manjericao fresco.",
                "Combinado de sushi com 10 pecas de sushi e 8 de sashimi.",
                "Tacos com recheio de carne, alface, queijo e molho picante.",
                "Salada Caesar com frango grelhado, croutons e parmesao.",
                "Lasanha a bolonhesa com camadas de carne, queijo e molho de tomate.",
                "Espaguete ao alho e oleo com toque de pimenta e parmesao."
            };
            decimal[] valorPratos = { 35.0m, 60.0m, 50.0m, 25.0m, 28.0m, 45.0m, 22.0m, 18.0m, 32.0m, 20.0m };

            for (int i = 1; i <= 10; i++)
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    IF NOT EXISTS (SELECT * FROM Prato WHERE id = @id)
                    BEGIN
                        INSERT INTO Prato (id, nome, descricao, valor, disponibilidade) VALUES (@id, @nome, @descricao, @valor, @disponibilidade)
                    END";
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Parameters.AddWithValue("@nome", nomesPratos[i - 1]);
                    cmd.Parameters.AddWithValue("@descricao", descricaoPratos[i - 1]);
                    cmd.Parameters.AddWithValue("@valor", valorPratos[i - 1]);
                    cmd.Parameters.AddWithValue("@disponibilidade", true);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InserirFornecedores(SqlConnection conn)
        {
            string[] nomesFornecedores = { "Apolinario Gourmet", "Jhey Natural", "Cavalcanti Carnes", "Apolinaria Alimentos", "JheyJhey Queijaria", "Jhey Bebidas", "Apolinario Organicos", "Cavalcanti Graos", "Jhey Gelados", "Apolinario Especiarias" };
            string[] estadosFornecedores = { "SP", "RJ", "BA", "SP", "MG", "SP", "CE", "PR", "SC", "RS" };

            for (int i = 1; i <= 10; i++)
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Fornecedor (id, nome, estado_origem) VALUES (@id, @nome, @estado)";
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Parameters.AddWithValue("@nome", nomesFornecedores[i - 1]);
                    cmd.Parameters.AddWithValue("@estado", estadosFornecedores[i - 1]);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InserirIngredientes(SqlConnection conn)
        {
            string[] nomesIngredientes = { "Carne Seca", "Linguica", "Costela", "Arroz", "Batata", "Peixe", "Leite de Coco", "Pimentao", "Dende", "Gorgonzola", "Pimenta", "Onion Rings", "Bacon", "Tomate", "Mussarela", "Manjericao", "Carne", "Alface", "Queijo", "Frango", "Croutons", "Parmesao", "Molho de Tomate", "Farinha de Trigo", "Alho", "Oleo", "Pimenta do Reino" };
            string[] observacaoIngredientes = {
                "Ideal para feijoada.",
                "Linguica para feijoada.",
                "Costela bovina para feijoada.",
                "Arroz para acompanhar pratos.",
                "Batata para acompanhamento.",
                "Peixe fresco para moqueca.",
                "Leite de coco para moqueca.",
                "Pimentao para tempero.",
                "Oleo de dende.",
                "Queijo gorgonzola.",
                "Pimenta para geleia.",
                "Onion rings para hamburguer.",
                "Bacon crocante.",
                "Tomate fresco para pizza.",
                "Mussarela para pizza.",
                "Manjericao fresco.",
                "Carne moida para tacos.",
                "Alface fresca.",
                "Queijo ralado.",
                "Frango grelhado para salada.",
                "Croutons crocantes.",
                "Parmesao ralado.",
                "Molho de tomate para lasanha.",
                "Farinha de trigo para massas.",
                "Alho para espaguete.",
                "Oleo para fritura.",
                "Pimenta do reino para tempero."
            };

            for (int i = 1; i <= nomesIngredientes.Length; i++)
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Ingredientes (id, nome, data_fabricacao, data_validade, quantidade, observacao) VALUES (@id, @nome, @data_fabricacao, @data_validade, @quantidade, @observacao)";
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Parameters.AddWithValue("@nome", nomesIngredientes[i - 1]);
                    cmd.Parameters.AddWithValue("@data_fabricacao", DateTime.Now.AddMonths(-2));
                    cmd.Parameters.AddWithValue("@data_validade", DateTime.Now.AddMonths(4));
                    cmd.Parameters.AddWithValue("@quantidade", 50);
                    cmd.Parameters.AddWithValue("@observacao", observacaoIngredientes[i - 1]);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InserirUsos(SqlConnection conn)
        {
            int[][] pratoIngredientes = new int[][]
            {
                new int[] { 1, 2, 3, 4 },             // Feijoada Completa: Carne Seca, Linguica, Costela, Arroz
                new int[] { 4, 5 },                   // Picanha Grelhada: Arroz, Batata
                new int[] { 6, 7, 8, 9 },             // Moqueca de Peixe: Peixe, Leite de Coco, Pimentao, Dende
                new int[] { 10, 11, 12, 13 },         // Hamburguer Artesanal: Gorgonzola, Pimenta, Onion Rings, Bacon
                new int[] { 14, 15, 16 },             // Pizza Marguerita: Tomate, Mussarela, Manjericao
                new int[] { 17, 18, 19 },             // Combinado Sushi: Carne, Alface, Queijo
                new int[] { 17, 18, 19 },             // Tacos Mexicanos: Carne, Alface, Queijo
                new int[] { 20, 21, 22 },             // Salada Caesar: Frango, Croutons, Parmesao
                new int[] { 23, 24, 15 },             // Lasanha Bolonhesa: Molho de Tomate, Farinha de Trigo, Mussarela
                new int[] { 4, 25, 26, 27 }           // Espaguete ao Alho e Oleo: Arroz, Alho, Oleo, Pimenta do Reino
            };

            for (int pratoId = 1; pratoId <= 10; pratoId++)
            {
                foreach (int ingredienteId in pratoIngredientes[pratoId - 1])
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Usos (id_prato, id_ingrediente) VALUES (@id_prato, @id_ingrediente)";
                        cmd.Parameters.AddWithValue("@id_prato", pratoId);
                        cmd.Parameters.AddWithValue("@id_ingrediente", ingredienteId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            Console.WriteLine("Usos registrados com sucesso!");
        }

        private void InserirVendas(SqlConnection conn)
        {
            Random random = new Random();

            for (int i = 1; i <= 10; i++)
            {
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Venda (id, id_cliente, id_prato, quantidade, dia, hora, valor) VALUES (@id, @id_cliente, @id_prato, @quantidade, @dia, @hora, @valor)";
                    cmd.Parameters.AddWithValue("@id", i);
                    cmd.Parameters.AddWithValue("@id_cliente", i);
                    cmd.Parameters.AddWithValue("@id_prato", i);
                    int quantidade = random.Next(1, 5);
                    cmd.Parameters.AddWithValue("@quantidade", quantidade);
                    cmd.Parameters.AddWithValue("@dia", DateTime.Now.Date);
                    cmd.Parameters.AddWithValue("@hora", DateTime.Now.TimeOfDay);
                    cmd.Parameters.AddWithValue("@valor", quantidade * 25.0m);
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Vendas registradas com sucesso!");
        }
    }
}
