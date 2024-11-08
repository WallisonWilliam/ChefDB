using System;
using RestauranteDB.Database;
using System.Data.SqlClient;

namespace RestauranteDB
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseConnection dbConnection = new DatabaseConnection("Server=localhost\\SQLEXPRESS;Database=master;Integrated Security=True;");
            DatabaseSetup setup = new DatabaseSetup(dbConnection);
            DataSeeder seeder = new DataSeeder(dbConnection);
            TriggerManager triggers = new TriggerManager(dbConnection);
            UserPermissions users = new UserPermissions(dbConnection);
            StoredProcedures procedures = new StoredProcedures(dbConnection);

            //setup.RemoverLogins();             
            //setup.DestruirBancoDeDados();    
            setup.CriarBancoDeDados();
            setup.CriarTabelas();
            seeder.InserirDadosIniciais();
            setup.CriarViews();
            triggers.CriarTriggers();
            users.SetupUsersAndPermissions();
            procedures.CriarProceduresEFuncoes();

            Console.WriteLine("Sistema do Restaurante iniciado com sucesso!");

            bool exit = false;
            while (!exit)
            {
                try
                {
                    Console.WriteLine("\nBem-vindo! Escolha uma opção:");
                    Console.WriteLine("1. Fazer login");
                    Console.WriteLine("2. Sair");
                    Console.Write("Opção: ");
                    string opcao = Console.ReadLine();

                    if (opcao == "1")
                    {
                        bool isAuthenticated = false;
                        int loginAttempts = 0;

                        while (!isAuthenticated && loginAttempts < 5)
                        {
                            Console.WriteLine("\nDigite seu nome de usuário:");
                            string username = Console.ReadLine();
                            Console.WriteLine("Digite sua senha:");
                            string password = Console.ReadLine();

                            if (AuthenticateUser(username, password))
                            {
                                isAuthenticated = true;
                                switch (username.ToLower())
                                {
                                    case "administrador":
                                        Console.WriteLine("Você está logado como Administrador.");
                                        AdministradorMenu(dbConnection); 
                                        break;
                                    case "gerente":
                                        Console.WriteLine("Você está logado como Gerente.");
                                        GerenteMenu();
                                        break;
                                    case "funcionario":
                                        Console.WriteLine("Você está logado como Funcionário.");
                                        FuncionarioMenu();
                                        break;
                                    default:
                                        Console.WriteLine("Usuário não encontrado.");
                                        isAuthenticated = false;
                                        break;
                                }
                            }
                            else
                            {
                                loginAttempts++;
                                Console.WriteLine("Nome de usuário ou senha incorretos. Tente novamente.");
                                if (loginAttempts == 5)
                                {
                                    Console.WriteLine("Muitas tentativas incorretas. Voltando ao menu inicial...");
                                }
                            }
                        }
                    }
                    else if (opcao == "2")
                    {
                        exit = true;
                        Console.WriteLine("Saindo do sistema...");
                    }
                    else
                    {
                        Console.WriteLine("Opção inválida, tente novamente.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocorreu um erro: {ex.Message}. Tente novamente.");
                }
            }
        }

        static bool AuthenticateUser(string username, string password)
        {
            return (username == "Administrador" && password == "12345!") ||
                   (username == "Gerente" && password == "12345") ||
                   (username == "Funcionario" && password == "12345");
        }

        public static int Calculo(decimal valorCompra)
        {
            return (int)(valorCompra / 10);
        }

        static void ConsultarEstatisticas(DatabaseConnection dbConnection)
        {
            try
            {
                using (SqlConnection conn = dbConnection.GetDatabaseConnection())
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("Estatisticas", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine($"Produto mais vendido: {reader["ProdutoMaisVendido"]}, Total Vendas: {reader["TotalVendas"]}, " +
                                                  $"Valor Ganho: {reader["ValorGanho"]}");
                            }
                            if (reader.NextResult() && reader.Read())
                            {
                                Console.WriteLine($"Produto menos vendido: {reader["ProdutoMenosVendido"]}, Total Vendas: {reader["TotalVendas"]}, " +
                                                  $"Valor Ganho: {reader["ValorGanho"]}");
                            }
                            if (reader.NextResult() && reader.Read())
                            {
                                Console.WriteLine($"Mês de maior venda do produto mais vendido: {reader["MesMaiorVenda"]}");
                            }
                            if (reader.NextResult() && reader.Read())
                            {
                                Console.WriteLine($"Mês de menor venda do produto mais vendido: {reader["MesMenorVenda"]}");
                            }
                            if (reader.NextResult() && reader.Read())
                            {
                                Console.WriteLine($"Mês de maior venda do produto menos vendido: {reader["MesMaiorVenda"]}");
                            }
                            if (reader.NextResult() && reader.Read())
                            {
                                Console.WriteLine($"Mês de menor venda do produto menos vendido: {reader["MesMenorVenda"]}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}. Tente novamente.");
            }
        }


        static string RealizarSorteio(DatabaseConnection dbConnection)
        {
            using (SqlConnection conn = dbConnection.GetDatabaseConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("Sorteio", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    var result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : "Nenhum cliente sorteado.";
                }
            }
        }

        public static void RealizarReajuste(DatabaseConnection dbConnection, decimal percentual)
        {
            using (SqlConnection conn = dbConnection.GetDatabaseConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("Reajuste", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@percentual", percentual);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void GastarPontos(DatabaseConnection dbConnection, int clienteID, int pratoID)
        {
            using (SqlConnection conn = dbConnection.GetDatabaseConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("Gastar_Pontos", conn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@clienteID", clienteID);
                    cmd.Parameters.AddWithValue("@pratoID", pratoID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AdministradorMenu(DatabaseConnection dbConnection)
        {
            int opcao;
            do
            {
                Console.WriteLine("\nMenu do Administrador:");
                Console.WriteLine("1. Realizar Reajuste de Preços");
                Console.WriteLine("2. Realizar Sorteio de Pontos para um Cliente");
                Console.WriteLine("3. Consultar Estatísticas de Venda");
                Console.WriteLine("4. Gastar Pontos do Cliente");
                Console.WriteLine("5. Voltar ao login");

                if (int.TryParse(Console.ReadLine(), out opcao))
                {
                    switch (opcao)
                    {
                        case 1:
                            Console.Write("Digite o percentual de reajuste: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal percentual))
                            {
                                RealizarReajuste(dbConnection, percentual);
                                Console.WriteLine("Reajuste de preços aplicado com sucesso.");
                            }
                            break;
                        case 2:
                            string clienteSorteado = RealizarSorteio(dbConnection);
                            Console.WriteLine($"Cliente sorteado: {clienteSorteado}, que ganhou 100 pontos.");
                            break;
                        case 3:
                            ConsultarEstatisticas(dbConnection);
                            break;
                        case 4:
                            Console.Write("Digite o ID do cliente: ");
                            if (int.TryParse(Console.ReadLine(), out int clienteID))
                            {
                                Console.Write("Digite o ID do prato: ");
                                if (int.TryParse(Console.ReadLine(), out int pratoID))
                                {
                                    GastarPontos(dbConnection, clienteID, pratoID);
                                    Console.WriteLine("Pontos do cliente utilizados na compra.");
                                }
                            }
                            break;
                        case 5:
                            Console.WriteLine("Voltando ao login...");
                            break;
                        default:
                            Console.WriteLine("Opção inválida.");
                            break;
                    }
                }
            } while (opcao != 5);
        }

        static void GerenteMenu()
        {
            int opcao;
            do
            {
                Console.WriteLine("\nMenu do Gerente:");
                Console.WriteLine("1. Realizar Reajuste de Preços");
                Console.WriteLine("2. Consultar Estatísticas de Venda");
                Console.WriteLine("3. Voltar ao login");

                if (int.TryParse(Console.ReadLine(), out opcao))
                {
                    switch (opcao)
                    {
                        case 1:
                            Console.Write("Digite o percentual de reajuste: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal percentual))
                            {
                                Console.WriteLine("Reajuste de preços aplicado com sucesso.");
                            }
                            break;
                        case 2:
                            Console.WriteLine("Exibindo estatísticas de venda...");
                            break;
                        case 3:
                            Console.WriteLine("Voltando ao login...");
                            break;
                        default:
                            Console.WriteLine("Opção inválida.");
                            break;
                    }
                }
            } while (opcao != 3);
        }

        static void FuncionarioMenu()
        {
            int opcao;
            do
            {
                Console.WriteLine("\nMenu do Funcionário:");
                Console.WriteLine("1. Registrar Venda");
                Console.WriteLine("2. Voltar ao login");

                if (int.TryParse(Console.ReadLine(), out opcao))
                {
                    switch (opcao)
                    {
                        case 1:
                            Console.Write("Digite o ID do cliente: ");
                            if (int.TryParse(Console.ReadLine(), out int clienteID))
                            {
                                Console.Write("Digite o ID do prato: ");
                                if (int.TryParse(Console.ReadLine(), out int pratoID))
                                {
                                    Console.Write("Digite a quantidade: ");
                                    if (int.TryParse(Console.ReadLine(), out int quantidade))
                                    {
                                        decimal valorTotal = quantidade * 25.0m;
                                        int pontos = Calculo(valorTotal);
                                        Console.WriteLine($"Venda registrada com sucesso. Cliente ganhou {pontos} pontos.");
                                    }
                                }
                            }
                            break;
                        case 2:
                            Console.WriteLine("Voltando ao login...");
                            break;
                        default:
                            Console.WriteLine("Opção inválida.");
                            break;
                    }
                }
            } while (opcao != 2);
        }
    }
}
