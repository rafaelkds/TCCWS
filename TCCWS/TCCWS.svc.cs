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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TCCWS" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select TCCWS.svc or TCCWS.svc.cs at the Solution Explorer and start debugging.
    public class TCCWS : ITCCWS
    {    
        public Atualizacao Sincronizar(List<string> atualizacoes, DateTime ultimaAtualizacao, string identificacao)
        {
            Sincronizacao sincronizacao = new Sincronizacao();
            return sincronizacao.Sincronizar(atualizacoes, ultimaAtualizacao, identificacao);
        }
    }
}
