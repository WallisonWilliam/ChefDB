# RestauranteDB

Este projeto é um sistema de banco de dados para um restaurante, desenvolvido em C# e SQL Server. Ele inclui operações para criação de banco de dados, tabelas, inserção de dados iniciais e configuração de triggers automáticos para gerenciar a pontuação dos clientes com base em compras.

## Funcionalidades

- Criação e exclusão do banco de dados `RestauranteDB`.
- Criação de tabelas para armazenar informações de clientes, pratos, fornecedores, ingredientes e vendas.
- Inserção de dados iniciais em cada tabela.
- Configuração de triggers para:
  - Atribuir pontos aos clientes com base em cada compra.
  - Validar a disponibilidade de ingredientes e pratos.
  - Reduzir o estoque de ingredientes com base nas vendas.
