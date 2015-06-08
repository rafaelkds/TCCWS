using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace TCCWS
{
    public class Sincronizacao
    {
        public Atualizacao Sincronizar(List<string> atualizacoes, DateTime ultimaAtualizacao, string identificacao)
        {
            if (BancoDeDados.BeginTransaction())
            {
                foreach (string sql in atualizacoes)
                {
                    NpgsqlCommand comando = new NpgsqlCommand(sql);
                    BancoDeDados.NonQuery(comando);
                }

                Atualizacao atualizacao = new Atualizacao();
                bool maxIds = false;

                #region Identificacao
                NpgsqlCommand command = new NpgsqlCommand("SELECT id FROM celular WHERE identificacao = '" + identificacao + "'");
                DataSet ds = BancoDeDados.Query(command);
                if (ds == null) return null;
                DataTableReader dtr = ds.CreateDataReader();

                if (dtr.Read())
                {
                    atualizacao.idCelular = dtr.GetInt32(0);

                    if (ultimaAtualizacao.Ticks == 0)
                        maxIds = true;
                }
                else
                {
                    command = new NpgsqlCommand("INSERT INTO celular(identificacao) VALUES ('" + identificacao + "')");
                    BancoDeDados.NonQuery(command);
                    command = new NpgsqlCommand("SELECT id FROM celular WHERE identificacao = '" + identificacao + "'");
                    ds = BancoDeDados.Query(command);
                    if (ds == null) return null;
                    dtr = ds.CreateDataReader();
                    dtr.Read();
                    atualizacao.idCelular = dtr.GetInt32(0);
                }
                #endregion

                #region Cliente
                command = new NpgsqlCommand(@"SELECT id, nome, cpf, rua, numero, bairro, cidade, uf, cep, complemento, telefone, email, ativo, alteracao 
FROM cliente WHERE alteracao > @alt ORDER BY Id");
                command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
                ds = BancoDeDados.Query(command);
                if (ds == null) return null;
                dtr = ds.CreateDataReader();
                List<ClienteWS> clientes = new List<ClienteWS>();

                while (dtr.Read())
                {
                    clientes.Add(new ClienteWS()
                    {
                        Id = dtr.GetString(0),
                        Nome = dtr.GetString(1),
                        Cpf = dtr.GetString(2),
                        Rua = dtr.GetString(3),
                        Numero = dtr.GetString(4),
                        Bairro = dtr.GetString(5),
                        Cidade = dtr.GetString(6),
                        Uf = dtr.GetString(7),
                        Cep = (dtr.IsDBNull(8) || string.IsNullOrWhiteSpace(dtr.GetString(8))) ? null : dtr.GetString(8),
                        Complemento = (dtr.IsDBNull(9) || string.IsNullOrWhiteSpace(dtr.GetString(9))) ? null : dtr.GetString(9),
                        Telefone = (dtr.IsDBNull(10) || string.IsNullOrWhiteSpace(dtr.GetString(10))) ? null : dtr.GetString(10),
                        Email = (dtr.IsDBNull(11) || string.IsNullOrWhiteSpace(dtr.GetString(11))) ? null : dtr.GetString(11),
                        Ativo = dtr.GetBoolean(12)
                    });
                    if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(13))
                    {
                        atualizacao.dtAtualizado = dtr.GetDateTime(13);
                    }
                    if (maxIds)
                    {
                        string[] aux = dtr.GetString(0).Split('/');
                        int id = Convert.ToInt32(aux[1]);
                        if (atualizacao.idCelular == Convert.ToInt32(aux[0]) && (atualizacao.maxIdCliente == null || atualizacao.maxIdCliente < id))
                            atualizacao.maxIdCliente = id;
                    }
                }

                atualizacao.clientes = clientes;
                #endregion

                #region Produto
                command = new NpgsqlCommand(@"SELECT id, nome, estoque, valor, ativo, alteracao 
FROM produto WHERE alteracao > @alt ORDER BY Id");
                command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
                ds = BancoDeDados.Query(command);
                if (ds == null) return null;
                dtr = ds.CreateDataReader();
                List<ProdutoWS> produtos = new List<ProdutoWS>();

                while (dtr.Read())
                {
                    produtos.Add(new ProdutoWS()
                    {
                        Id = dtr.GetInt32(0),
                        Nome = dtr.GetString(1),
                        Estoque = dtr.GetDecimal(2),
                        Valor = dtr.GetDecimal(3),
                        Ativo = dtr.GetBoolean(4)
                    });
                    if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(5))
                    {
                        atualizacao.dtAtualizado = dtr.GetDateTime(5);
                    }
                }

                atualizacao.produtos = produtos;
                #endregion

                #region Pedido
                command = new NpgsqlCommand(@"SELECT id, numero, id_cliente, id_vendedor, valor, data_emissao, data_pagamento, observacoes, alteracao 
FROM pedido WHERE alteracao > @alt ORDER BY Id");
                command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
                ds = BancoDeDados.Query(command);
                if (ds == null) return null;
                dtr = ds.CreateDataReader();
                List<PedidoWS> pedidos = new List<PedidoWS>();

                while (dtr.Read())
                {
                    pedidos.Add(new PedidoWS()
                    {
                        Id = dtr.GetString(0),
                        Numero = dtr.GetString(1),
                        IdCliente = dtr.GetString(2),
                        IdVendedor = dtr.GetInt32(3),
                        Valor = dtr.GetDecimal(4),
                        DataEmissao = dtr.GetDateTime(5),
                        DataPagamento = dtr.GetDateTime(6),
                        Observacoes = dtr.GetString(7)
                    });
                    if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(8))
                    {
                        atualizacao.dtAtualizado = dtr.GetDateTime(8);
                    }
                    if (maxIds)
                    {
                        string[] aux = dtr.GetString(0).Split('/');
                        int id = Convert.ToInt32(aux[1]);
                        if (atualizacao.idCelular == Convert.ToInt32(aux[0]) && (atualizacao.maxIdPedido == null || atualizacao.maxIdPedido < id))
                            atualizacao.maxIdPedido = id;
                    }
                }

                atualizacao.pedidos = pedidos;
                #endregion

                #region Produtos Pedido
                command = new NpgsqlCommand(@"SELECT id, id_pedido, id_produto, valor, quantidade, quantidade_entregue, alteracao 
FROM produto_pedido WHERE alteracao > @alt ORDER BY Id");
                command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
                ds = BancoDeDados.Query(command);
                if (ds == null) return null;
                dtr = ds.CreateDataReader();
                List<ProdutoPedidoWS> produtospedido = new List<ProdutoPedidoWS>();

                while (dtr.Read())
                {
                    produtospedido.Add(new ProdutoPedidoWS()
                    {
                        Id = dtr.GetString(0),
                        IdPedido = dtr.GetString(1),
                        IdProduto = dtr.GetInt32(2),
                        Valor = dtr.GetDecimal(3),
                        Quantidade = dtr.GetDecimal(4),
                        QuantidadeEntregue = dtr.GetDecimal(5)
                    });
                    if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(6))
                    {
                        atualizacao.dtAtualizado = dtr.GetDateTime(6);
                    }
                    if (maxIds)
                    {
                        string[] aux = dtr.GetString(0).Split('/');
                        int id = Convert.ToInt32(aux[1]);
                        if (atualizacao.idCelular == Convert.ToInt32(aux[0]) && (atualizacao.maxIdProdutoPedido == null || atualizacao.maxIdProdutoPedido < id))
                            atualizacao.maxIdProdutoPedido = id;
                    }
                }

                atualizacao.produtospedido = produtospedido;
                #endregion

                #region Receber
                command = new NpgsqlCommand(@"SELECT id, id_pedido, ordem, valor, vencimento, pagamento, alteracao 
FROM receber WHERE alteracao > @alt ORDER BY Id");
                command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
                ds = BancoDeDados.Query(command);
                if (ds == null) return null;
                dtr = ds.CreateDataReader();
                List<ReceberWS> receber = new List<ReceberWS>();

                while (dtr.Read())
                {
                    receber.Add(new ReceberWS()
                    {
                        Id = dtr.GetString(0),
                        IdPedido = dtr.GetString(1),
                        Ordem = dtr.GetInt32(2),
                        Valor = dtr.GetDecimal(3),
                        Vencimento = dtr.GetDateTime(4),
                        Pagamento = dtr.GetDateTime(5),
                    });
                    if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(6))
                    {
                        atualizacao.dtAtualizado = dtr.GetDateTime(6);
                    }
                    if (maxIds)
                    {
                        string[] aux = dtr.GetString(0).Split('/');
                        int id = Convert.ToInt32(aux[1]);
                        if (atualizacao.idCelular == Convert.ToInt32(aux[0]) && (atualizacao.maxIdReceber == null || atualizacao.maxIdReceber < id))
                            atualizacao.maxIdReceber = id;
                    }
                }

                atualizacao.receber = receber;
                #endregion

                #region Anotacao
                command = new NpgsqlCommand(@"SELECT id, id_pedido, data, data_ultima_alteracao, texto, alteracao 
FROM anotacao WHERE alteracao > @alt ORDER BY Id");
                command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
                ds = BancoDeDados.Query(command);
                if (ds == null) return null;
                dtr = ds.CreateDataReader();
                List<AnotacaoWS> anotacoes = new List<AnotacaoWS>();

                while (dtr.Read())
                {
                    anotacoes.Add(new AnotacaoWS()
                    {
                        Id = dtr.GetString(0),
                        IdPedido = dtr.GetString(1),
                        Data = dtr.GetDateTime(2),
                        DataUltimaAlteracao = dtr.GetDateTime(3),
                        Texto = dtr.GetString(4)
                    });
                    if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(5))
                    {
                        atualizacao.dtAtualizado = dtr.GetDateTime(5);
                    }
                    if (maxIds)
                    {
                        string[] aux = dtr.GetString(0).Split('/');
                        int id = Convert.ToInt32(aux[1]);
                        if (atualizacao.idCelular == Convert.ToInt32(aux[0]) && (atualizacao.maxIdAnotacao == null || atualizacao.maxIdAnotacao < id))
                            atualizacao.maxIdAnotacao = id;
                    }
                }

                atualizacao.anotacoes = anotacoes;
                #endregion

                #region Vendedor
                command = new NpgsqlCommand(@"SELECT id, nome, alteracao 
FROM vendedor WHERE alteracao > @alt ORDER BY Id");
                command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
                ds = BancoDeDados.Query(command);
                if (ds == null) return null;
                dtr = ds.CreateDataReader();
                List<VendedorWS> vendedores = new List<VendedorWS>();

                while (dtr.Read())
                {
                    vendedores.Add(new VendedorWS()
                    {
                        Id = dtr.GetInt32(0),
                        Nome = dtr.GetString(1)
                    });
                    if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(2))
                    {
                        atualizacao.dtAtualizado = dtr.GetDateTime(2);
                    }
                }

                atualizacao.vendedores = vendedores;
                #endregion

                if (BancoDeDados.CommitTransaction())
                    return atualizacao;
                else
                    BancoDeDados.RollbackTransaction();
            }
            return null;
        }
    }
}