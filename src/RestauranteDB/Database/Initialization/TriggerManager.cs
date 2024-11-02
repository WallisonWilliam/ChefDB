using System;
using System.Data.SqlClient;

namespace RestauranteDB.Database {
    
    public class TriggerManager
	{
        private readonly DatabaseConnection db = new DatabaseConnection();

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