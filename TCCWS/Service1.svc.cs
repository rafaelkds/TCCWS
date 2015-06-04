using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading;

namespace TCCWS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
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
                atualizacao.id = dtr.GetInt32(0);

                if(ultimaAtualizacao.Ticks == 0)
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
                atualizacao.id = dtr.GetInt32(0);
            }
            #endregion    

            #region Cliente
            command = new NpgsqlCommand(@"SELECT * FROM cliente WHERE alteracao > @alt ORDER BY Id");
            command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
            ds = BancoDeDados.Query(command);
            if (ds == null) return null;
            dtr = ds.CreateDataReader();
            List<ClienteWS> clientes = new List<ClienteWS>();

            while (dtr.Read())
            {
                clientes.Add(new ClienteWS()
                {
                    Id = dtr.GetString(11),
                    Nome = dtr.GetString(0),
                    Cpf = dtr.GetString(1),
                    Rua = dtr.GetString(2),
                    Numero = dtr.GetString(3),
                    Bairro = dtr.GetString(4),
                    Cidade = dtr.GetInt32(5),
                    Cep = (dtr.IsDBNull(6) || string.IsNullOrWhiteSpace(dtr.GetString(6))) ? null : dtr.GetString(6),
                    Complemento = (dtr.IsDBNull(7) || string.IsNullOrWhiteSpace(dtr.GetString(7))) ? null : dtr.GetString(7),
                    Telefone = (dtr.IsDBNull(8) || string.IsNullOrWhiteSpace(dtr.GetString(8))) ? null : dtr.GetString(8),
                    Email = (dtr.IsDBNull(9) || string.IsNullOrWhiteSpace(dtr.GetString(9))) ? null : dtr.GetString(9)
                });
                if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(10))
                {
                    atualizacao.dtAtualizado = dtr.GetDateTime(10);
                }
                if (maxIds)
                {
                    string[] aux = dtr.GetString(11).Split('/');
                    int id = Convert.ToInt32(aux[1]);
                    if (atualizacao.id == Convert.ToInt32(aux[0]) && (atualizacao.maxIdCliente == null || atualizacao.maxIdCliente < id))
                        atualizacao.maxIdCliente = id;
                }
            }

            atualizacao.clientes = clientes;
            #endregion

            #region Produto
            command = new NpgsqlCommand(@"SELECT * FROM produto WHERE alteracao > @alt ORDER BY Id");
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
                    Ativo = dtr.GetBoolean(3),
                    Valor = dtr.GetDecimal(4)
                });
                if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(5))
                {
                    atualizacao.dtAtualizado = dtr.GetDateTime(5);
                }
            }

            atualizacao.produtos = produtos;
            #endregion

            #region Pedido
            command = new NpgsqlCommand(@"SELECT * FROM pedido WHERE alteracao > @alt ORDER BY Id");
            command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
            ds = BancoDeDados.Query(command);
            if (ds == null) return null;
            dtr = ds.CreateDataReader();
            List<PedidoWS> pedidos = new List<PedidoWS>();

            while (dtr.Read())
            {
                pedidos.Add(new PedidoWS()
                {
                    Id = dtr.GetString(6),
                    IdCliente = dtr.GetString(8),
                    IdVendedor = dtr.GetInt32(1),
                    Numero = dtr.GetString(0),
                    Valor = dtr.GetDecimal(2),
                    DataEmissao = dtr.GetDateTime(3),
                    DataPago = dtr.GetDateTime(4),
                    Observacoes = dtr.GetString(5)
                });
                if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(7))
                {
                    atualizacao.dtAtualizado = dtr.GetDateTime(7);
                }
                if (maxIds)
                {
                    string[] aux = dtr.GetString(6).Split('/');
                    int id = Convert.ToInt32(aux[1]);
                    if (atualizacao.id == Convert.ToInt32(aux[0]) && (atualizacao.maxIdPedido == null || atualizacao.maxIdPedido < id))
                        atualizacao.maxIdPedido = id;
                }
            }

            atualizacao.pedidos = pedidos;
            #endregion

            #region Produtos Pedido
            command = new NpgsqlCommand(@"SELECT * FROM produto_pedido WHERE alteracao > @alt ORDER BY Id");
            command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
            ds = BancoDeDados.Query(command);
            if (ds == null) return null;
            dtr = ds.CreateDataReader();
            List<ProdutoPedidoWS> produtospedido = new List<ProdutoPedidoWS>();

            while (dtr.Read())
            {
                produtospedido.Add(new ProdutoPedidoWS()
                {
                    Id = dtr.GetString(3),
                    IdPedido = dtr.GetString(4),
                    IdProduto = dtr.GetInt32(0),
                    Valor = dtr.GetDecimal(1),
                    Quantidade = dtr.GetDecimal(2)
                });
                if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(5))
                {
                    atualizacao.dtAtualizado = dtr.GetDateTime(5);
                }
                if (maxIds)
                {
                    string[] aux = dtr.GetString(3).Split('/');
                    int id = Convert.ToInt32(aux[1]);
                    if (atualizacao.id == Convert.ToInt32(aux[0]) && (atualizacao.maxIdProdutoPedido == null || atualizacao.maxIdProdutoPedido < id))
                        atualizacao.maxIdProdutoPedido = id;
                }
            }

            atualizacao.produtospedido = produtospedido;
            #endregion

            #region Receber
            command = new NpgsqlCommand(@"SELECT * FROM receber WHERE alteracao > @alt ORDER BY Id");
            command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
            ds = BancoDeDados.Query(command);
            if (ds == null) return null;
            dtr = ds.CreateDataReader();
            List<ReceberWS> receber = new List<ReceberWS>();

            while (dtr.Read())
            {
                receber.Add(new ReceberWS()
                {
                    Id = dtr.GetString(6),
                    IdPedido = dtr.GetString(0),
                    Ordem = dtr.GetInt32(1),
                    Valor = dtr.GetDecimal(2),
                    Vencimento = dtr.GetDateTime(3),
                    Pagamento = dtr.GetDateTime(4),
                });
                if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(5))
                {
                    atualizacao.dtAtualizado = dtr.GetDateTime(5);
                }
                if (maxIds)
                {
                    string[] aux = dtr.GetString(6).Split('/');
                    int id = Convert.ToInt32(aux[1]);
                    if (atualizacao.id == Convert.ToInt32(aux[0]) && (atualizacao.maxIdReceber == null || atualizacao.maxIdReceber < id))
                        atualizacao.maxIdReceber = id;
                }
            }

            atualizacao.receber = receber;
            #endregion

            #region Anotacao
            command = new NpgsqlCommand(@"SELECT * FROM anotacao WHERE alteracao > @alt ORDER BY Id");
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
                    if (atualizacao.id == Convert.ToInt32(aux[0]) && (atualizacao.maxIdAnotacao == null || atualizacao.maxIdAnotacao < id))
                        atualizacao.maxIdAnotacao = id;
                }
            }

            atualizacao.anotacoes = anotacoes;
            #endregion

                BancoDeDados.CommitTransaction();
                return atualizacao;
            }
            return null;
        }
    }
}
