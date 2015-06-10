using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCCWS
{
    public class BancoDeDados
    {
        private NpgsqlConnection conn;
        private NpgsqlConnection Conn
        {
            get
            {
                if (conn == null || conn.State == ConnectionState.Closed)
                {
                    conn = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["teste"].ConnectionString);
                    conn.Open();
                }
                return conn;
            }
        }

        private NpgsqlTransaction transaction;
        public bool BeginTransaction()
        {
            if (transaction == null)
            {
                transaction = Conn.BeginTransaction();
                return true;
            }
            return false;
        }

        public bool CommitTransaction()
        {
            if (transaction != null)
            {
                transaction.Commit();
                transaction = null;
                Conn.Close();
                return true;
            }
            return false;
        }

        public bool RollbackTransaction()
        {
            if (transaction != null)
            {
                transaction.Rollback();
                transaction = null;
                Conn.Close();
                return true;
            }
            return false;
        }

        public DataSet Query(NpgsqlCommand command)
        {
            try
            {
                command.Connection = Conn;
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(command);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            catch (Exception e)
            {
            }
            return null;
        }

        public int NonQuery(NpgsqlCommand command)
        {
            try
            {
                command.Connection = Conn;
                int i = command.ExecuteNonQuery();
                return i;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}
