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

            setup.CriarBancoDeDados();
            setup.CriarTabelas();
            seeder.InserirDadosIniciais();
            triggers.CriarTriggers();

            //sistema.DestruirBancoDeDados();
            Console.WriteLine("Sistema do Restaurante iniciado com sucesso!");
        }
    }
}
