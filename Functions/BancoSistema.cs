using LiteDB;
using ProgramaCadastroLogin.Enums;
using ProgramaCadastroLogin.Models;

namespace ProgramaCadastroLogin.Funcoes
{
    public class BancoSistema
    {
        private static LiteDatabase _conexaoDb;

        static BancoSistema()
        {
            Config config = SegurancaSistema.Config;

            if (config == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                _conexaoDb = new LiteDatabase($"Filename={config.CaminhoBanco};Password={config.SenhaBanco};Collation={new Collation("pt-BR/None")}");
            }
        }

        public static void Dispose()
        {
            _conexaoDb.Dispose();
        }

        #region .:Usuarios:
        public static IEnumerable<Usuario> CarregarUsuarios(Usuario usuario)
        {
            IEnumerable<Usuario> listaUsuarios = _conexaoDb.GetCollection<Usuario>().Find(usr => usr.Nome != usuario.Nome);

            return listaUsuarios;
        }

        public static Usuario RetornarUsuario(string nome, string senha = null)
        {
            Usuario usuario;

            if (senha == null)
            {
                usuario = _conexaoDb.GetCollection<Usuario>().FindOne(usr => usr.Nome.Equals(nome, StringComparison.InvariantCulture));
            }
            else
            {
                usuario = _conexaoDb.GetCollection<Usuario>().FindOne(usr => usr.Nome.Equals(nome, StringComparison.InvariantCulture) && usr.Senha.Equals(senha, StringComparison.InvariantCulture));
            }

            return usuario;
        }
        public static Usuario RetornarUsuarioAssinante(Documento documento)
        {
            Usuario usuario;

            usuario = _conexaoDb.GetCollection<Usuario>().FindOne(usr => usr.Nome.Equals(documento.Assinador, StringComparison.InvariantCulture));

            return usuario;
        }

        public static void InserirUsuario(Usuario usuario)
        {
            _conexaoDb.GetCollection<Usuario>().Insert(usuario);
        }

        public static void AtualizarSenhaUsuario(Usuario usuario, string senha)
        {
            usuario.Senha = senha;
            _conexaoDb.GetCollection<Usuario>().Update(usuario);
        }

        public static void DeletarUsuario(Usuario usuario)
        {
            _conexaoDb.GetCollection<Usuario>().Delete(usuario.Id);
        }
        #endregion

        #region .:Documentos:
        public static IEnumerable<Documento> BuscarDocumentos(Usuario usuario)
        {
            var listaDocumentos = _conexaoDb.GetCollection<Documento>().Find(usr => usr.IdUsuarioDocumento == usuario.Id && usr.Status != StatusDocumento.Removido);

            return listaDocumentos;
        }
        public static IEnumerable<Documento> BuscarDocumentos(Usuario usuario, StatusDocumento statusDocumento)
        {
            IEnumerable<Documento> listaDocumentos;

            if (statusDocumento == StatusDocumento.Assinado)
            {
                listaDocumentos = _conexaoDb.GetCollection<Documento>().Find(usr => usr.IdUsuarioDocumento == usuario.Id && usr.Status == StatusDocumento.Assinado);
            }
            else if (statusDocumento == StatusDocumento.Pendente)
            {
                listaDocumentos = _conexaoDb.GetCollection<Documento>().Find(usr => usr.IdUsuarioDocumento == usuario.Id && usr.Status == StatusDocumento.Pendente);
            }
            else
            {
                listaDocumentos = _conexaoDb.GetCollection<Documento>().Find(usr => usr.IdUsuarioDocumento == usuario.Id && usr.Status == StatusDocumento.Removido);
            }

            return listaDocumentos;
        }
        public static IEnumerable<Documento> BuscarDocumentosVinculadosAoUsuario(Usuario usuario, StatusDocumento statusDocumento)
        {
            IEnumerable<Documento> listaDocumentos;

            if (statusDocumento == StatusDocumento.Pendente)
            {
                listaDocumentos = _conexaoDb.GetCollection<Documento>().Find(usr => usr.Assinador == usuario.Nome && usr.Status == StatusDocumento.Pendente);
            }
            else if (statusDocumento == StatusDocumento.Assinado)
            {
                listaDocumentos = _conexaoDb.GetCollection<Documento>().Find(usr => usr.Assinador == usuario.Nome && usr.Status == StatusDocumento.Assinado);
            }
            else
            {
                listaDocumentos = _conexaoDb.GetCollection<Documento>().Find(usr => usr.Assinador == usuario.Nome && usr.Status == StatusDocumento.Removido);
            }

            return listaDocumentos;
        }

        public static Documento RetornarDocumentoVinculadosAoUsuario(Usuario usuario, int idDocumento)
        {
            var documento = _conexaoDb.GetCollection<Documento>().FindOne(usr => usr.Assinador == usuario.Nome && usr.IdDocumento == idDocumento);

            return documento;
        }
        public static Documento RetornarDocumento(Usuario usuario, StatusDocumento statusDocumento, string idDocumento = null, string nomeDocumento = null)
        {
            Documento documento = new Documento();
            int intIdDocumento;

            var opcaoIntValida = Int32.TryParse(idDocumento, out intIdDocumento);

            if (statusDocumento == StatusDocumento.Assinado)
            {
                if (opcaoIntValida)
                {
                    documento = _conexaoDb.GetCollection<Documento>().FindOne(usr => usr.IdUsuarioDocumento == usuario.Id && usr.IdDocumento == intIdDocumento && usr.Status == StatusDocumento.Assinado);
                }
                else
                {
                    throw new ArgumentException("\nNão foi possível converter o idDocumento para Int.");
                }
               
            }
            else if (statusDocumento == StatusDocumento.Pendente)
            {
                documento = _conexaoDb.GetCollection<Documento>().FindOne(usr => usr.IdUsuarioDocumento == usuario.Id && usr.IdDocumento == intIdDocumento && usr.Status == StatusDocumento.Pendente);
            }
            else
            {
                documento = _conexaoDb.GetCollection<Documento>().FindOne(usr => usr.IdUsuarioDocumento == usuario.Id && usr.Nome == nomeDocumento);
            }

            return documento;
        }

        public static int RetornarQuantidadeDocumento()
        {
            int documento = _conexaoDb.GetCollection<Documento>().Count();

            return documento;
        }
        public static int RetornarQuantidadeDocumentoUsuario(Usuario usuario)
        {
            int documento = _conexaoDb.GetCollection<Documento>().Count(usr => usr.IdUsuarioDocumento == usuario.Id && usr.Status != StatusDocumento.Removido);

            return documento;
        }

        public static bool VerificarDocumentosVinculadosAoUsuario(Usuario usuario, int idDocumento, IEnumerable<Documento> listaDocumentos)
        {
            bool verifica = listaDocumentos.Any(usr => usr.Assinador == usuario.Nome && usr.IdDocumento == idDocumento);

            return verifica;
        }
        public static bool VerificarDocumento(Usuario usuario, StatusDocumento statusDocumento, IEnumerable<Documento> listaDocumentos, string idDocumento = null, string nomeDocumento = null)
        {
            bool verifica;

            int intIdDocumento;

            var opcaoIntValida = Int32.TryParse(idDocumento, out intIdDocumento);

            if (nomeDocumento == null && statusDocumento == StatusDocumento.Assinado)
            {    
                if (opcaoIntValida)
                {
                    verifica = listaDocumentos.Any(usr => usr.IdUsuarioDocumento == usuario.Id && usr.IdDocumento == intIdDocumento && usr.Status == StatusDocumento.Assinado);
                }
                else
                {
                    throw new ArgumentException("\nNão foi possível converter o idDocumento para Int.");
                }

            }
            else if (nomeDocumento == null && statusDocumento == StatusDocumento.Pendente)
            {
                verifica = listaDocumentos.Any(usr => usr.IdUsuarioDocumento == usuario.Id && usr.IdDocumento == intIdDocumento && usr.Status == StatusDocumento.Pendente);
            }
            else
            {
                verifica = listaDocumentos.Any(usr => usr.IdUsuarioDocumento == usuario.Id && usr.Nome == nomeDocumento);
            }

            return verifica;
        }

        public static void InserirDocumento(Documento documento)
        {
            _conexaoDb.GetCollection<Documento>().Insert(documento);
        }

        public static void AtualizarDocumento(Documento documento, dynamic conteudo, string atributo)
        {
            if (atributo == "ChavePublica")
            {
                documento.ChavePublica = conteudo;
                _conexaoDb.GetCollection<Documento>().Update(documento._id, documento);
            }
            else if (atributo == "Assinatura")
            {
                documento.Assinatura = conteudo;
                _conexaoDb.GetCollection<Documento>().Update(documento._id, documento);
            }
            else if (atributo == "Status")
            {
                documento.Status = conteudo;
                _conexaoDb.GetCollection<Documento>().Update(documento._id, documento);
            }
        }

        public static void DeletarDocumentosUsuario(Usuario usuario)
        {
            var listadocumento = _conexaoDb.GetCollection<Documento>().Find(usr => usr.IdUsuarioDocumento == usuario.Id && usr.Status == StatusDocumento.Pendente);

            foreach (var item in listadocumento)
            {
                item.Status = StatusDocumento.Removido;
                _conexaoDb.GetCollection<Documento>().Update(item._id, item);
            }
        }
        public static void DeletarDocumentosVinculadosAoUsuario(Usuario usuario)
        {
            IEnumerable<Documento> listaDocumentos = _conexaoDb.GetCollection<Documento>().Find(usr => usr.Assinador == usuario.Nome && usr.Status == StatusDocumento.Pendente);

            foreach (var item in listaDocumentos)
            {
                item.Status = StatusDocumento.Removido;
                _conexaoDb.GetCollection<Documento>().Update(item._id, item);
            }
        }
        #endregion
    }
}