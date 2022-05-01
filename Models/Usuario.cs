namespace ProgramaCadastroLogin.Models
{
    public class Usuario
    {
        public string Nome { get; set; }
        public string Senha { get; set; }
        public string ChavePrivada { get; set; }
        public string ChavePublica { get; set; }
        public Guid Id { get; set; }
        public Usuario(string nome, string senha, string chavePublica, string chavePrivada, Guid id)
        {
            Nome = nome;
            Senha = senha;
            ChavePublica = chavePublica;
            ChavePrivada = chavePrivada;
            Id = id;
        }
    }
}