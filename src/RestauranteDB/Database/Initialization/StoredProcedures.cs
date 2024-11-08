using System;
using System.Data.SqlClient;

namespace RestauranteDB.Database
{
    public class StoredProcedures
    {
        private readonly DatabaseConnection db;

        public StoredProcedures(DatabaseConnection dbConnection)
        {
            db = dbConnection;
        }

        public void CriarProceduresEFuncoes()
        {
            using (SqlConnection conn = db.GetDatabaseConnection())
            {
                conn.Open();

                // Procedure Reajuste
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "IF OBJECT_ID('Reajuste', 'P') IS NOT NULL DROP PROCEDURE Reajuste;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                    CREATE PROCEDURE Reajuste (@percentual DECIMAL(5, 2))
                    AS
                    BEGIN
                        UPDATE Prato
                        SET valor = valor * (1 + @percentual / 100);
                    END;";
                    cmd.ExecuteNonQuery();
                }

                // Procedure Sorteio com retorno do nome do cliente sorteado
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "IF OBJECT_ID('Sorteio', 'P') IS NOT NULL DROP PROCEDURE Sorteio;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                    CREATE PROCEDURE Sorteio
                    AS
                    BEGIN
                        DECLARE @clienteID INT, @clienteNome NVARCHAR(50);
                        SELECT TOP 1 @clienteID = id, @clienteNome = nome FROM Cliente ORDER BY NEWID();
                        UPDATE Cliente
                        SET pontos = pontos + 100
                        WHERE id = @clienteID;
                        SELECT @clienteNome AS ClienteSorteado;
                    END;";
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "IF OBJECT_ID('Estatisticas', 'P') IS NOT NULL DROP PROCEDURE Estatisticas;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                        CREATE PROCEDURE Estatisticas
                    AS
                    BEGIN
                        SELECT TOP 1 p.nome AS ProdutoMaisVendido, SUM(v.quantidade) AS TotalVendas, SUM(v.valor) AS ValorGanho
                        INTO #ProdutoMaisVendido
                        FROM Prato p
                        JOIN Venda v ON p.id = v.id_prato
                        GROUP BY p.id, p.nome
                        ORDER BY TotalVendas DESC;

                        SELECT TOP 1 p.nome AS ProdutoMenosVendido, SUM(v.quantidade) AS TotalVendas, SUM(v.valor) AS ValorGanho
                        INTO #ProdutoMenosVendido
                        FROM Prato p
                        JOIN Venda v ON p.id = v.id_prato
                        GROUP BY p.id, p.nome
                        ORDER BY TotalVendas ASC;

                        SELECT TOP 1 MONTH(v.dia) AS MesMaiorVenda
                        INTO #MesMaiorVendaMaisVendido
                        FROM Venda v
                        JOIN Prato p ON p.id = v.id_prato
                        WHERE p.nome = (SELECT ProdutoMaisVendido FROM #ProdutoMaisVendido)
                        GROUP BY MONTH(v.dia)
                        ORDER BY SUM(v.quantidade) DESC;

                        SELECT TOP 1 MONTH(v.dia) AS MesMenorVenda
                        INTO #MesMenorVendaMaisVendido
                        FROM Venda v
                        JOIN Prato p ON p.id = v.id_prato
                        WHERE p.nome = (SELECT ProdutoMaisVendido FROM #ProdutoMaisVendido)
                        GROUP BY MONTH(v.dia)
                        ORDER BY SUM(v.quantidade) ASC;

                        SELECT TOP 1 MONTH(v.dia) AS MesMaiorVenda
                        INTO #MesMaiorVendaMenosVendido
                        FROM Venda v
                        JOIN Prato p ON p.id = v.id_prato
                        WHERE p.nome = (SELECT ProdutoMenosVendido FROM #ProdutoMenosVendido)
                        GROUP BY MONTH(v.dia)
                        ORDER BY SUM(v.quantidade) DESC;

                        SELECT TOP 1 MONTH(v.dia) AS MesMenorVenda
                        INTO #MesMenorVendaMenosVendido
                        FROM Venda v
                        JOIN Prato p ON p.id = v.id_prato
                        WHERE p.nome = (SELECT ProdutoMenosVendido FROM #ProdutoMenosVendido)
                        GROUP BY MONTH(v.dia)
                        ORDER BY SUM(v.quantidade) ASC;

                        SELECT * FROM #ProdutoMaisVendido;
                        SELECT * FROM #ProdutoMenosVendido;
                        SELECT * FROM #MesMaiorVendaMaisVendido;
                        SELECT * FROM #MesMenorVendaMaisVendido;
                        SELECT * FROM #MesMaiorVendaMenosVendido;
                        SELECT * FROM #MesMenorVendaMenosVendido;
                    END;";

                    cmd.ExecuteNonQuery();
                }



                // Procedure Gastar_Pontos
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "IF OBJECT_ID('Gastar_Pontos', 'P') IS NOT NULL DROP PROCEDURE Gastar_Pontos;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                    CREATE PROCEDURE Gastar_Pontos (@clienteID INT, @pratoID INT)
                    AS
                    BEGIN
                        DECLARE @valorPrato DECIMAL(10, 2), @pontosCliente INT, @valorEmPontos INT;

                        SELECT @valorPrato = valor FROM Prato WHERE id = @pratoID;
                        SELECT @pontosCliente = pontos FROM Cliente WHERE id = @clienteID;

                        SET @valorEmPontos = CEILING(@valorPrato);

                        IF @pontosCliente >= @valorEmPontos
                        BEGIN
                            UPDATE Cliente
                            SET pontos = pontos - @valorEmPontos
                            WHERE id = @clienteID;
                        END;
                    END;";
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "IF OBJECT_ID('Calculo', 'FN') IS NOT NULL DROP FUNCTION Calculo;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                    CREATE FUNCTION Calculo (@valorCompra DECIMAL(10, 2))
                    RETURNS INT
                    AS
                    BEGIN
                        RETURN FLOOR(@valorCompra / 10);
                    END;";
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Procedures e função criadas com sucesso!");
            }
        }
    }
}
