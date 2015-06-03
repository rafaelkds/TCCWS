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
        Atualizacao Sincronizar(List<string> atualizacoes, DateTime ultimaAtualizacao, string identificacao);
    }


    [DataContract]
    public class Atualizacao
    {
        [DataMember]
        public DateTime dtAtualizado { get; set; }
        [DataMember]
        public List<ClienteWS> clientes { get; set; }
        [DataMember]
        public List<ProdutoWS> produtos { get; set; }
        [DataMember]
        public List<PedidoWS> pedidos { get; set; }
        [DataMember]
        public List<ProdutoPedidoWS> produtospedido { get; set; }
        [DataMember]
        public List<ReceberWS> receber { get; set; }
        [DataMember]
        public List<AnotacaoWS> anotacoes { get; set; }
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public int? maxIdCliente { get; set; }
        [DataMember]
        public int? maxIdPedido { get; set; }
        [DataMember]
        public int? maxIdProdutoPedido { get; set; }
        [DataMember]
        public int? maxIdReceber { get; set; }
        [DataMember]
        public int? maxIdAnotacao { get; set; }
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

    [DataContract]
    public class ProdutoWS
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public decimal Estoque { get; set; }
        [DataMember]
        public bool Ativo { get; set; }
        [DataMember]
        public decimal Valor { get; set; }
    }

    [DataContract]
    public class PedidoWS
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Numero { get; set; }
        [DataMember]
        public string IdCliente { get; set; }
        [DataMember]
        public int IdVendedor { get; set; }
        [DataMember]
        public decimal Valor { get; set; }
        [DataMember]
        public DateTime DataEmissao { get; set; }
        [DataMember]
        public DateTime DataPago { get; set; }
        [DataMember]
        public string Observacoes { get; set; }
    }

    [DataContract]
    public class ProdutoPedidoWS
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string IdPedido { get; set; }
        [DataMember]
        public int IdProduto { get; set; }
        [DataMember]
        public decimal Valor { get; set; }
        [DataMember]
        public decimal Quantidade { get; set; }
    }

    [DataContract]
    public class ReceberWS
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string IdPedido { get; set; }
        [DataMember]
        public int Ordem { get; set; }
        [DataMember]
        public decimal Valor { get; set; }
        [DataMember]
        public DateTime Vencimento { get; set; }
        [DataMember]
        public DateTime Pagamento { get; set; }
    }

    [DataContract]
    public class AnotacaoWS
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string IdPedido { get; set; }
        [DataMember]
        public DateTime Data { get; set; }
        [DataMember]
        public DateTime DataUltimaAlteracao { get; set; }
        [DataMember]
        public string Texto { get; set; }
    }
}
