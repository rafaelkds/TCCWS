using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace TCCWS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        Atualizacao Sincronizar(List<string> atualizacoes, DateTime ultimaAtualizacao);

        [OperationContract]
        List<string> GetData();

        
    }


    [DataContract]
    public class Atualizacao
    {
        [DataMember]
        public DateTime dtAtualizado { get; set; }
        [DataMember]
        public List<ClienteWS> clientes { get; set; }
    }

    [DataContract]
    public class ClienteWS
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public string Cpf { get; set; }
        [DataMember]
        public string Rua { get; set; }
        [DataMember]
        public string Numero { get; set; }
        [DataMember]
        public string Bairro { get; set; }
        [DataMember]
        public int Cidade { get; set; }
        [DataMember]
        public string Cep { get; set; }
        [DataMember]
        public string Complemento { get; set; }
        [DataMember]
        public string Telefone { get; set; }
        [DataMember]
        public string Email { get; set; }
    }
}
