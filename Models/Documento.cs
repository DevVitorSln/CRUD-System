using LiteDB;
using ProgramaCadastroLogin.Enums;

namespace ProgramaCadastroLogin.Models
{
    public class Documento
    {
        public ObjectId _id { get; set; }
        public int IdDocumento { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Conteudo { get; set; }
        public Guid IdUsuarioDocumento { get; set; }
        public string Assinatura { get; set; }
        public string Assinador { get; set; }
        public string ChavePublica { get; set; }
        public StatusDocumento Status { get; set; }
        public Documento(string nome, string descricao, string conteudo, int idDocumento, Usuario assinador, Usuario usuario, StatusDocumento status)
        {
            Nome = nome;
            Descricao = descricao;
            Conteudo = conteudo;
            IdUsuarioDocumento = usuario.Id;
            IdDocumento = idDocumento;
            Assinador = assinador.Nome;
            Status = status;
        }

        public Documento()
        { }

    }
}