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
            CriarTriggerIngredientesVencidos();
            CriarTriggerCompraIndisponivel();
            CriarTriggerVendaProduto();
        }

        // Trigger para pontos de cliente e recria se ja existir
        public void CriarTriggerPontos()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();

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

        public void CriarTriggerIngredientesVencidos()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();

                using (SqlCommand dropCmd = conn.CreateCommand())
                {
                    dropCmd.CommandText = @"IF OBJECT_ID('trg_IngredientExpiration', 'TR') IS NOT NULL 
                                    DROP TRIGGER trg_IngredientExpiration;";
                    dropCmd.ExecuteNonQuery();
                }

                // Cria o trigger para marcar pratos como indisponiveis quando um ingrediente vence
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    CREATE TRIGGER trg_IngredientExpiration
                    ON Ingredientes
                    AFTER UPDATE, INSERT
                    AS
                    BEGIN
                        UPDATE Prato
                        SET disponibilidade = 0
                        WHERE id IN (
                            SELECT DISTINCT id_prato
                            FROM Usos
                            INNER JOIN Ingredientes ON Usos.id_ingrediente = Ingredientes.id
                            WHERE Ingredientes.data_validade < GETDATE()
                        );
                    END;";
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Trigger para marcar pratos como indisponíveis devido a ingredientes vencidos criado com sucesso!");
            }
        }
        public void CriarTriggerCompraIndisponivel()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();

                using (SqlCommand dropCmd = conn.CreateCommand())
                {
                    dropCmd.CommandText = @"IF OBJECT_ID('trg_BlockUnavailablePurchase', 'TR') IS NOT NULL 
                                    DROP TRIGGER trg_BlockUnavailablePurchase;";
                    dropCmd.ExecuteNonQuery();
                }

                // Cria o trigger para bloquear a compra de pratos indisponiveis
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    CREATE TRIGGER trg_BlockUnavailablePurchase
                    ON Venda
                    INSTEAD OF INSERT
                    AS
                    BEGIN
                        DECLARE @id_prato INT;
                        DECLARE @disponibilidade BIT;

                        SELECT @id_prato = id_prato FROM INSERTED;

                        SELECT @disponibilidade = disponibilidade FROM Prato WHERE id = @id_prato;

                        IF @disponibilidade = 1
                        BEGIN
                            INSERT INTO Venda (id, id_cliente, id_prato, quantidade, dia, hora, valor)
                            SELECT id, id_cliente, id_prato, quantidade, dia, hora, valor
                            FROM INSERTED;
                        END
                        ELSE
                        BEGIN
                            RAISERROR ('Não é possível realizar a compra. O prato selecionado está indisponível.', 16, 1);
                        END
                    END;";
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Trigger para impedir a compra de pratos indisponíveis criado com sucesso!");
            }
        }
        public void CriarTriggerVendaProduto()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();

                using (SqlCommand dropCmd = conn.CreateCommand())
                {
                    dropCmd.CommandText = @"IF OBJECT_ID('trg_ReduceIngredientQuantity', 'TR') IS NOT NULL 
                                    DROP TRIGGER trg_ReduceIngredientQuantity;";
                    dropCmd.ExecuteNonQuery();
                }

                // Trigger para reduzir a quantidade de ingredientes quando uma venda eh feita
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    CREATE TRIGGER trg_ReduceIngredientQuantity
                    ON Venda
                    AFTER INSERT
                    AS
                    BEGIN
                        DECLARE @id_prato INT, @quantidade INT;
                        SELECT @id_prato = id_prato, @quantidade = quantidade FROM INSERTED;

                        UPDATE Ingredientes
                        SET quantidade = quantidade - @quantidade
                        WHERE id IN (SELECT id_ingrediente FROM Usos WHERE id_prato = @id_prato)
                    END;";
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Trigger para reduzir a quantidade de ingredientes na venda criado com sucesso!");
            }
        }
    }
}
