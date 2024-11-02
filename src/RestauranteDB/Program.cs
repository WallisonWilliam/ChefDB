using System;
using RestauranteDB.Database;

namespace RestauranteDB
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseSetup setup = new DatabaseSetup();
            DataSeeder seeder = new DataSeeder();
            TriggerManager triggers = new TriggerManager();

            setup.DestruirBancoDeDados();
            setup.CriarBancoDeDados();
            setup.CriarTabelas();
            seeder.InserirDadosIniciais();
            triggers.CriarTriggers();

            
            Console.WriteLine("Sistema do Restaurante iniciado com sucesso!");
        }
    }
}
