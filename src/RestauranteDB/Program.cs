using System;
using RestauranteDB.Database;

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

            //setup.DestruirBancoDeDados();
            setup.CriarBancoDeDados();
            setup.CriarTabelas();
            seeder.InserirDadosIniciais();
            triggers.CriarTriggers();

            Console.WriteLine("Sistema do Restaurante iniciado com sucesso!");
        }
    }
}
