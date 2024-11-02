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

        // Metodo para inserir dados iniciais evitando duplicações
        public void InserirDadosIniciais()
        {
            using (SqlConnection conn = (SqlConnection)db.GetDatabaseConnection())
            {
                conn.Open();

                // Limpa os dados existentes nas tabelas para evitar duplicação
                using (SqlCommand clearCmd = conn.CreateCommand())
                {
                    clearCmd.CommandText = "DELETE FROM Venda; DELETE FROM Usos; DELETE FROM Ingredientes; DELETE FROM Fornecedor; DELETE FROM Prato; DELETE FROM Cliente;";
                    clearCmd.ExecuteNonQuery();
                }

                // Inserindo Clientes
                string[] nomesClientes = { "João Silva", "Maria Oliveira", "Pedro Souza", "Ana Lima", "Carlos Pereira", "Mariana Castro", "Lucas Mendes", "Fernanda Rocha", "Paulo Alves", "Júlia Costa" };
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

                // Inserindo Pratos
                string[] nomesPratos = { "Feijoada Completa", "Picanha Grelhada", "Moqueca de Peixe", "Hambúrguer Artesanal", "Pizza Marguerita", "Combinado Sushi", "Tacos Mexicanos", "Salada Caesar", "Lasanha Bolonhesa", "Espaguete ao Alho e Óleo" };
                string[] descricaoPratos = {
                    "Feijoada com carne seca, linguiça, costela e acompanhamentos.",
                    "Picanha grelhada ao ponto, acompanhada de arroz e batatas fritas.",
                    "Moqueca de peixe com leite de coco, pimentões e dendê.",
                    "Hambúrguer artesanal com creme gorgonzola, geleia de pimenta, onion rings e bacon.",
                    "Pizza marguerita com molho de tomate, mussarela e manjericão fresco.",
                    "Combinado de sushi com 10 peças de sushi e 8 de sashimi.",
                    "Tacos com recheio de carne, alface, queijo e molho picante.",
                    "Salada Caesar com frango grelhado, croutons e parmesão.",
                    "Lasanha à bolonhesa com camadas de carne, queijo e molho de tomate.",
                    "Espaguete ao alho e óleo com toque de pimenta e parmesão."
                };
                decimal[] valorPratos = { 35.0m, 60.0m, 50.0m, 25.0m, 28.0m, 45.0m, 22.0m, 18.0m, 32.0m, 20.0m };

                for (int i = 1; i <= 10; i++)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO Prato (id, nome, descricao, valor, disponibilidade) VALUES (@id, @nome, @descricao, @valor, @disponibilidade)";
                        cmd.Parameters.AddWithValue("@id", i);
                        cmd.Parameters.AddWithValue("@nome", nomesPratos[i - 1]);
                        cmd.Parameters.AddWithValue("@descricao", descricaoPratos[i - 1]);
                        cmd.Parameters.AddWithValue("@valor", valorPratos[i - 1]);
                        cmd.Parameters.AddWithValue("@disponibilidade", true);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Inserindo Fornecedores
                string[] nomesFornecedores = { "Apolinario Gourmet", "Jhey Natural", "Cavalcanti Carnes", "Apolinaria Alimentos", "JheyJhey Queijaria", "Jhey Bebidas", "Apolinário Orgânicos", "Cavalcanti Grãos", "Jhey Gelados", "Apolinario Especiarias" };
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

                // Inserindo Ingredientes
                string[] nomesIngredientes = { "Arroz", "Feijão", "Carne Bovina", "Peito de Frango", "Batata", "Mussarela", "Tomate", "Alface", "Cebola", "Farinha de Trigo" };
                string[] observacaoIngredientes = {
                    "Grão longo, ideal para pratos diversos.",
                    "Feijão carioca selecionado, de alta qualidade.",
                    "Carne bovina para grelhar e assados.",
                    "Peito de frango desossado e sem pele.",
                    "Batatas frescas, ideais para fritura e assados.",
                    "Queijo mussarela em fatias.",
                    "Tomate orgânico, fresco.",
                    "Alface crocante, ideal para saladas.",
                    "Cebola roxa, mais suave e adocicada.",
                    "Farinha de trigo especial para massas."
                };

                for (int i = 1; i <= 10; i++)
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

                Console.WriteLine("10 elementos cadastrados em cada tabela!");
            }
        }
    }
}
