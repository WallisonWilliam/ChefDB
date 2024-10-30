using System;
using RestauranteDB.Database;

namespace RestauranteDB
{
    class Program
    {
        static void Main(string[] args)
        {
            RestauranteSystem sistema = new RestauranteSystem();
            sistema.CriarBancoDeDados();
            sistema.CriarTabelas();
            sistema.InserirDadosIniciais();
            sistema.CriarTriggers();  
                                     

            Console.WriteLine("Sistema do Restaurante iniciado com sucesso!");
        }
    }
}
