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
            UserPermissions users = new UserPermissions(dbConnection);
            StoredProcedures procedures = new StoredProcedures(dbConnection); // Instancia a classe StoredProcedures

            //setup.RemoverLogins();             
            //dsetup.DestruirBancoDeDados();       
            setup.CriarBancoDeDados();          
            setup.CriarTabelas();               
            seeder.InserirDadosIniciais();      
            setup.CriarViews();                 
            triggers.CriarTriggers();          
            users.SetupUsersAndPermissions();   

            Console.WriteLine("Sistema do Restaurante iniciado com sucesso!");
        }
    }
}
