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
            NpgsqlCommand command = new NpgsqlCommand("SELECT nome FROM clientes");
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

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

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

        public List<Cliente> GetClientes()
        {
            NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM clientes ORDER BY nome");
            DataSet ds = BancoDeDados.Query(command);
            if (ds == null) return null;
            DataTableReader dtr = ds.CreateDataReader();
            List<Cliente> clientes = new List<Cliente>();

            while (dtr.Read())
            {
                clientes.Add(new Cliente()
                {
                    Id = dtr.GetInt32(0),
                    Nome = dtr.GetString(1),
                    Cpf = dtr.GetString(2),
                    Rua = dtr.GetString(3),
                    Numero = dtr.GetString(4),
                    Bairro = dtr.GetString(5),
                    Cidade = dtr.GetInt32(6),
                    Cep = dtr.IsDBNull(7) ? "" : dtr.GetString(7),
                    Complemento = dtr.IsDBNull(8) ? "" : dtr.GetString(8),
                    Telefone = dtr.IsDBNull(9) ? "" : dtr.GetString(9),
                    Email = dtr.IsDBNull(10) ? "" : dtr.GetString(10)
                });
            }
            return clientes;
        }
        
    }
}
