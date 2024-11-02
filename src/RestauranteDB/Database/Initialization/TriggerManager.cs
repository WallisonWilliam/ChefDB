using System;
using System.Data.SqlClient;

namespace RestauranteDB.Database
{
    public class TriggerManager
    {
        private readonly DatabaseConnection db;

        public TriggerManager(DatabaseConnection dbConnection)
        {
            db = dbConnection;
        }

        public void CriarTriggers()
        {
            CriarTriggerPontos();
            /*
            CriarTriggerIngredientesVencidos();
            CriarTriggerCompraIndisponivel();
            CriarTriggerVendaProduto();
            */
        }

        // Trigger para pontos de cliente e recria se ja existir
        public void CriarTriggerPontos()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();

                // Exclui o trigger se ele ja existir
                using (SqlCommand dropCmd = conn.CreateCommand())
                {
                    dropCmd.CommandText = @"IF OBJECT_ID('trg_CalculatePoints', 'TR') IS NOT NULL DROP TRIGGER trg_CalculatePoints;";
                    dropCmd.ExecuteNonQuery();
                }

                // Cria o trigger que atualiza os pontos do cliente
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    CREATE TRIGGER trg_CalculatePoints
                    ON Venda
                    AFTER INSERT
                    AS
                    BEGIN
                        UPDATE Cliente
                        SET pontos = pontos + (INSERTED.valor / 10)
                        FROM Cliente
                        INNER JOIN INSERTED ON Cliente.id = INSERTED.id_cliente;
                    END;";
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Trigger de pontos criada com sucesso!");
            }
        }
    }
}
