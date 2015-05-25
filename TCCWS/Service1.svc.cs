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
        public List<string> GetData()
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT nome FROM cliente");
            DataSet ds = BancoDeDados.Query(command);
            if (ds == null) return null;
            DataTableReader dtr = ds.CreateDataReader();
            List<string> retorno = new List<string>();
            while (dtr.Read())
            {
                retorno.Add(dtr.GetString(0));
            }
            return retorno;
        }

        public void InsertData()
        {

        }
        /*
        public int InsertCliente(Cliente cliente)
        {
            NpgsqlCommand command = new NpgsqlCommand();

            #region command parameters
            int i = -1;
            string campos = "";

            if (!string.IsNullOrEmpty(cliente.Nome))
            {
                i++;
                command.Parameters.Add(new NpgsqlParameter("v" + i, NpgsqlDbType.Varchar));
                command.Parameters[i].Value = cliente.Nome;
                campos += string.IsNullOrEmpty(campos) ? "nome" : ", nome";
            }

            if (!string.IsNullOrEmpty(cliente.Cpf))
            {
                i++;
                command.Parameters.Add(new NpgsqlParameter("v" + i, NpgsqlDbType.Varchar));
                command.Parameters[i].Value = cliente.Cpf;
                campos += string.IsNullOrEmpty(campos) ? "cpf" : ", cpf";
            }

            if (!string.IsNullOrEmpty(cliente.Rua))
            {
                i++;
                command.Parameters.Add(new NpgsqlParameter("v" + i, NpgsqlDbType.Varchar));
                command.Parameters[i].Value = cliente.Rua;
                campos += string.IsNullOrEmpty(campos) ? "rua" : ", rua";
            }

            if (!string.IsNullOrEmpty(cliente.Numero))
            {
                i++;
                command.Parameters.Add(new NpgsqlParameter("v" + i, NpgsqlDbType.Varchar));
                command.Parameters[i].Value = cliente.Numero;
                campos += string.IsNullOrEmpty(campos) ? "numero" : ", numero";
            }

            if (!string.IsNullOrEmpty(cliente.Bairro))
            {
                i++;
                command.Parameters.Add(new NpgsqlParameter("v" + i, NpgsqlDbType.Varchar));
                command.Parameters[i].Value = cliente.Bairro;
                campos += string.IsNullOrEmpty(campos) ? "bairro" : ", bairro";
            }

            if (cliente.Cidade > 0)
            {
                i++;
                command.Parameters.Add(new NpgsqlParameter("v" + i, NpgsqlDbType.Integer));
                command.Parameters[i].Value = cliente.Cidade;
                campos += string.IsNullOrEmpty(campos) ? "cidade" : ", cidade";
            }

            if (!string.IsNullOrEmpty(cliente.Cep))
            {
                i++;
                command.Parameters.Add(new NpgsqlParameter("v" + i, NpgsqlDbType.Varchar));
                command.Parameters[i].Value = cliente.Cep;
                campos += string.IsNullOrEmpty(campos) ? "cep" : ", cep";
            }

            if (!string.IsNullOrEmpty(cliente.Complemento))
            {
                i++;
                command.Parameters.Add(new NpgsqlParameter("v" + i, NpgsqlDbType.Varchar));
                command.Parameters[i].Value = cliente.Complemento;
                campos += string.IsNullOrEmpty(campos) ? "complemento" : ", complemento";
            }

            if (!string.IsNullOrEmpty(cliente.Telefone))
            {
                i++;
                command.Parameters.Add(new NpgsqlParameter("v" + i, NpgsqlDbType.Varchar));
                command.Parameters[i].Value = cliente.Telefone;
                campos += string.IsNullOrEmpty(campos) ? "telefone" : ", telefone";
            }

            if (!string.IsNullOrEmpty(cliente.Email))
            {
                i++;
                command.Parameters.Add(new NpgsqlParameter("v" + i, NpgsqlDbType.Varchar));
                command.Parameters[i].Value = cliente.Email;
                campos += string.IsNullOrEmpty(campos) ? "email" : ", email";
            }

            #endregion

            string variaveis = "";

            for (int v = 0; v <= i; v++)
            {
                if (v == 0)
                    variaveis = ":v0";
                else
                    variaveis += ", :v" + v;
            }

            command.CommandText = "INSERT INTO clientes("
                + campos + ")"
                + "VALUES (" + variaveis + ")";

            return BancoDeDados.NonQuery(command);
        }
        */
        


        public Atualizacao Sincronizar(List<string> atualizacoes, DateTime ultimaAtualizacao)
        {
            if (BancoDeDados.BeginTransaction())
            {
                foreach (string sql in atualizacoes)
                {
                    NpgsqlCommand comando = new NpgsqlCommand(sql);
                    BancoDeDados.NonQuery(comando);
                }
                BancoDeDados.CommitTransaction();
            }

            Atualizacao atualizacao = new Atualizacao();

            #region Cliente
            NpgsqlCommand command = new NpgsqlCommand(@"SELECT * FROM cliente WHERE alteracao > @alt ORDER BY Id");
            command.Parameters.Add("alt", NpgsqlDbType.Timestamp).Value = ultimaAtualizacao;
            DataSet ds = BancoDeDados.Query(command);
            if (ds == null) return null;
            DataTableReader dtr = ds.CreateDataReader();
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
                    Cep = dtr.IsDBNull(6) ? "" : dtr.GetString(6),
                    Complemento = dtr.IsDBNull(7) ? "" : dtr.GetString(7),
                    Telefone = dtr.IsDBNull(8) ? "" : dtr.GetString(8),
                    Email = dtr.IsDBNull(9) ? "" : dtr.GetString(9)
                });
                if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(10))
                {
                    atualizacao.dtAtualizado = dtr.GetDateTime(10);
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
                    Estoque = dtr.GetDecimal(2)
                });
                if (atualizacao.dtAtualizado == null || atualizacao.dtAtualizado < dtr.GetDateTime(3))
                {
                    atualizacao.dtAtualizado = dtr.GetDateTime(3);
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
            }

            atualizacao.produtospedido = produtospedido;
            #endregion

            return atualizacao;
        }
    }
}
