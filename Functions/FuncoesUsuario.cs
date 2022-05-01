using ProgramaCadastroLogin.Enums;
using ProgramaCadastroLogin.Models;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ProgramaCadastroLogin.Funcoes
{
    public static class FuncoesUsuario
    {
        private static Usuario _usuario = null;

        #region .:Usuarios:
        public static string ListarNomesUsuarios()
        {
            MenuFuncoesUsuario.ImprimirMensagem("\n||USUÁRIOS||\n\n");

            var listaDocumentos = BancoSistema.CarregarUsuarios(_usuario);

            string retornoListaUsuarios = null;

            foreach (var item in listaDocumentos)
            {
                retornoListaUsuarios += $"{item.Nome}\n";
            }
            if (retornoListaUsuarios == null)
            {
              
                return null;
            }
            else
            {
                return retornoListaUsuarios;
            }
        }

        public static void CadastrarUsuario()
        {
            MenuFuncoesUsuario.ImprimirMensagem("\n|| CADASTRO DE USUÁRIO ||\n\nNome: ");

            string nome = MenuFuncoesUsuario.ReceberValorInserido();

            if (string.IsNullOrWhiteSpace(nome))
            {
                MenuFuncoesUsuario.ImprimirErro("O nome do usuário deve ser informado, tente novamente.");
                MenuFuncoesUsuario.Continuar();

                return;
            }
            else
            {
                var verifica = BancoSistema.RetornarUsuario(nome);

                if (verifica != null)
                {
                    MenuFuncoesUsuario.ImprimirErro("Usuário já cadastrado, tente novamente.");
                    MenuFuncoesUsuario.Continuar();

                    return;
                }
                else
                {
                    MenuFuncoesUsuario.ImprimirMensagem("\nSenha: ");
                    var senha = SegurancaSistema.MascararSenha();
                    string rawPassword = new NetworkCredential(string.Empty, senha).Password;

                    if (string.IsNullOrWhiteSpace(rawPassword))
                    {
                        MenuFuncoesUsuario.ImprimirErro("A senha do usuário deve ser informada, tente novamente.");
                        MenuFuncoesUsuario.Continuar();

                        return;
                    }
                    else
                    {
                        bool forcaSenhaCaracterEspecial = rawPassword.All(item => char.IsLetterOrDigit(item));
                        bool forcaSenhaLetraMaiscula = rawPassword.Any(item => char.IsUpper(item));
                        bool forcaSenhaNumero = rawPassword.Any(item => char.IsDigit(item));

                        if (forcaSenhaCaracterEspecial || !forcaSenhaLetraMaiscula || !forcaSenhaNumero || rawPassword.Length < 8)
                        {
                            MenuFuncoesUsuario.ImprimirErro("A senha deve ter no minímo 8 caracteres e pelo menos um(a):\n-Número.\n-Letra Maiúscula.\n-Caracter Especial.\n\nTente Novamente.");
                            MenuFuncoesUsuario.Continuar();

                            return;
                        }


                        {
                            MenuFuncoesUsuario.ImprimirMensagem("\nConfirme a Senha: ");
                            var confirmaSenha = SegurancaSistema.MascararSenha();
                            string rawPasswordConfirm = new NetworkCredential(string.Empty, confirmaSenha).Password;

                            if (string.IsNullOrWhiteSpace(rawPasswordConfirm))
                            {
                                MenuFuncoesUsuario.ImprimirErro("Confirme a senha para realizar o cadastro, tente novamente.");
                                MenuFuncoesUsuario.Continuar();

                                return;
                            }
                            else
                            {
                                if (rawPassword != rawPasswordConfirm)
                                {
                                    MenuFuncoesUsuario.ImprimirErro("Senhas não coincidem, tente novamente.");
                                    MenuFuncoesUsuario.Continuar();

                                    return;
                                }
                                else
                                {
                                    string hashSenha = SegurancaSistema.GerarHash(rawPasswordConfirm);

                                    RSA parDeChaves = SegurancaSistema.GeraParDeChaves();

                                    byte[] chavePublica = parDeChaves.ExportRSAPublicKey();
                                    byte[] chavePrivada = parDeChaves.ExportRSAPrivateKey();

                                    string chavePublicaB64Encode = Convert.ToBase64String(chavePublica);
                                    string chavePrivadaB64Encode = Convert.ToBase64String(chavePrivada);

                                    var md5 = MD5.Create();
                                    var hash = md5.ComputeHash(Encoding.Default.GetBytes(nome));
                                    Guid id = new Guid(hash);

                                    Usuario usuario = new Usuario(nome, hashSenha, chavePublicaB64Encode, chavePrivadaB64Encode, id);
                                    BancoSistema.InserirUsuario(usuario);

                                    MenuFuncoesUsuario.ImprimirMensagem("\nUsuário cadastrado com sucesso!");
                                    MenuFuncoesUsuario.Continuar();

                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Login()
        {
            MenuFuncoesUsuario.ImprimirMensagem("\n|| LOGIN ||\n\nNome: ");

            string nome = MenuFuncoesUsuario.ReceberValorInserido();

            if (string.IsNullOrWhiteSpace(nome))
            {
                MenuFuncoesUsuario.ImprimirErro("O nome do usuário deve ser informado, tente novamente.");
                MenuFuncoesUsuario.Continuar();

                return;
            }
            else
            {
                string mensagemSenha = "\nSenha: ";
                MenuFuncoesUsuario.ImprimirMensagem(mensagemSenha);

                var senha = SegurancaSistema.MascararSenha();
                string rawPassword = new NetworkCredential(string.Empty, senha).Password;

                if (string.IsNullOrWhiteSpace(rawPassword))
                {
                    MenuFuncoesUsuario.ImprimirErro("A senha do usuário deve ser informada, tente novamente.");
                    MenuFuncoesUsuario.Continuar();
                }
                else
                {
                    var hashSenha = SegurancaSistema.GerarHash(rawPassword);
                    var validarUsuario = BancoSistema.RetornarUsuario(nome, hashSenha);

                    if (validarUsuario != null)
                    {
                        _usuario = BancoSistema.RetornarUsuario(nome);
                        MenuFuncoesUsuario.ImprimirMensagem("\nLogin efetuado com sucesso!");
                        MenuFuncoesUsuario.Continuar("\nAperte uma tecla para iniciar sessão.");

                        return;
                    }
                    else
                    {
                        MenuFuncoesUsuario.ImprimirErro("Credenciais inválidas, tente novamente.");
                        MenuFuncoesUsuario.Continuar();

                        return;
                    }
                }
            }
        }
        public static void Logout()
        {
            if (_usuario != null)
                _usuario = null;
        }
        public static bool ValidaLoginUsuario()
        {
            return _usuario == null;
        }

        public static void AlterarSenha()
        {
            MenuFuncoesUsuario.ImprimirMensagem("\n|| ATUALIZAÇÃO DE SENHA ||\n\nInforme senha atual: ");

            var senhaAtual = SegurancaSistema.MascararSenha();
            string rawPassword = new NetworkCredential(string.Empty, senhaAtual).Password;

            if (string.IsNullOrWhiteSpace(rawPassword))
            {
                MenuFuncoesUsuario.ImprimirErro("A senha atual do usuário deve ser informada, tente novamente.");
                MenuFuncoesUsuario.Continuar();

                return;
            }

            string hashSenha = SegurancaSistema.GerarHash(rawPassword);

            if (hashSenha == _usuario.Senha)
            {

                MenuFuncoesUsuario.ImprimirMensagem("\nInforme a nova Senha: ");

                var senhaAux = SegurancaSistema.MascararSenha();
                string rawPasswordAux = new NetworkCredential(string.Empty, senhaAux).Password;

                if (string.IsNullOrWhiteSpace(rawPasswordAux))
                {
                    MenuFuncoesUsuario.ImprimirErro("A nova senha do usuário deve ser informada, tente novamente.");
                    MenuFuncoesUsuario.Continuar();

                    return;
                }
                else
                {
                    bool forcaSenhaCaracterEspecial = rawPassword.All(item => char.IsLetterOrDigit(item));
                    bool forcaSenhaLetraMaiscula = rawPassword.Any(item => char.IsUpper(item));
                    bool forcaSenhaNumero = rawPassword.Any(item => char.IsDigit(item));

                    if (forcaSenhaCaracterEspecial || !forcaSenhaLetraMaiscula || !forcaSenhaNumero || rawPassword.Length < 8)
                    {
                        MenuFuncoesUsuario.ImprimirErro("Minímo 8 caracteres e pelo menos um(a):\n-Número.\n-Letra Maiúscula.\n-Caracter Especial.\n\nTente Novamente.");
                        MenuFuncoesUsuario.Continuar();

                        return;
                    }
                    else
                    {
                        if (rawPasswordAux == rawPassword)
                        {
                            MenuFuncoesUsuario.ImprimirErro("A senha do usuário deve ser diferente da atual, tente novamente.");
                            MenuFuncoesUsuario.Continuar();

                            return;
                        }
                        else
                        {
                            MenuFuncoesUsuario.ImprimirMensagem("\n\nConfirme a nova senha: ");

                            var senhaAuxConfirm = SegurancaSistema.MascararSenha();
                            string rawPasswordAuxConfirm = new NetworkCredential(string.Empty, senhaAuxConfirm).Password;
     

                            if (string.IsNullOrWhiteSpace(rawPasswordAuxConfirm))
                            {
                                MenuFuncoesUsuario.ImprimirErro("Informe a confirmação da senha para realizar a atualização, tente novamente.");
                                MenuFuncoesUsuario.Continuar();

                                return;
                            }
                            else
                            {
                                if (rawPasswordAuxConfirm != rawPasswordAux)
                                {
                                    MenuFuncoesUsuario.ImprimirErro("Senhas não coincidem, tente novamente.");
                                    MenuFuncoesUsuario.Continuar();

                                    return;
                                }
                                else
                                {
                                    string hashSenhaAuxConfirm = SegurancaSistema.GerarHash(rawPasswordAuxConfirm);
                                    BancoSistema.AtualizarSenhaUsuario(_usuario, hashSenhaAuxConfirm);

                                    MenuFuncoesUsuario.ImprimirMensagem("\nSenha atualizada com sucesso!");
                                    MenuFuncoesUsuario.Continuar();

                                    return;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MenuFuncoesUsuario.ImprimirErro("Credenciais inválidas, tente novamente.");
                MenuFuncoesUsuario.Continuar();

                return;
            }
        }

        public static void Remover()
        {
            MenuFuncoesUsuario.ImprimirMensagem("\n|| DELETAR CONTA ||\n\nSenha atual: ");

            var senhaAtual = SegurancaSistema.MascararSenha();
            string rawPassword = new NetworkCredential(string.Empty, senhaAtual).Password;

            if (string.IsNullOrWhiteSpace(rawPassword))
            {
                MenuFuncoesUsuario.ImprimirErro("A senha do usuário deve ser informada, tente novamente.");
                MenuFuncoesUsuario.Continuar();

                return;
            }

            string hashSenha = SegurancaSistema.GerarHash(rawPassword);

            if (hashSenha == _usuario.Senha)
            {
                MenuFuncoesUsuario.ImprimirMensagem("\n[Confirmação]: Deseja  deletar a conta?\ntecle (S) para sim e (N) não: ");
                string teclaConfirm = MenuFuncoesUsuario.ReceberValorInserido();

                if (string.IsNullOrWhiteSpace(teclaConfirm))
                {
                    MenuFuncoesUsuario.ImprimirErro("Informe uma opção, tente novamente.");
                    MenuFuncoesUsuario.Continuar();

                    return;
                }
                else
                {
                    if (teclaConfirm == "S" || teclaConfirm == "s")
                    {
                        BancoSistema.DeletarDocumentosUsuario(_usuario);
                        BancoSistema.DeletarDocumentosVinculadosAoUsuario(_usuario);
                        BancoSistema.DeletarUsuario(_usuario);

                        MenuFuncoesUsuario.ImprimirMensagem("\nUsuário deletado com sucesso!");
                        MenuFuncoesUsuario.Continuar();

                        _usuario = null;
                    }
                    else if (teclaConfirm == "N" || teclaConfirm == "n")
                    {
                        MenuFuncoesUsuario.ImprimirMensagem("\nConfirmação negada, a conta não foi deletada.");
                        MenuFuncoesUsuario.Continuar();

                        return;
                    }
                    else
                    {
                        MenuFuncoesUsuario.ImprimirErro("Informe uma opção válida, tente novamente.");
                        MenuFuncoesUsuario.Continuar();

                        return;
                    }
                }
            }
            else
            {
                MenuFuncoesUsuario.ImprimirErro("Credenciais inválidas, tente novamente.");
                MenuFuncoesUsuario.Continuar();
                return;
            }

        }
        #endregion

        #region .:Documentos:
        public static void ListarDocumentosAssinados()
        {
            MenuFuncoesUsuario.ImprimirMensagem("\n\n|| DOCUMENTOS ||\n");

            var listaDocumentos = BancoSistema.BuscarDocumentosVinculadosAoUsuario(_usuario, StatusDocumento.Assinado);

            string retornoListaDocuemntos = null;

            foreach (var item in listaDocumentos)
            {
                retornoListaDocuemntos += $"\nNome: {item.Nome}\nDescrição: {item.Descricao}\n" +
                $"Conteudo(hash): {item.Conteudo}\nId do documento: {item.IdDocumento}\nAssinante: {item.Assinador}\nStatus: {item.Status}\n"; ;
            }

            if (retornoListaDocuemntos == null)
            {
                MenuFuncoesUsuario.ImprimirMensagem("\nNenhum documento foi assinado.");
                MenuFuncoesUsuario.Continuar();

                return;
            }
            else
            {
                MenuFuncoesUsuario.ImprimirMensagem(retornoListaDocuemntos);
                MenuFuncoesUsuario.Continuar();

                return;
            }
        }
        public static void ListarAtributosDocumentosUsuario()
        {
            MenuFuncoesUsuario.ImprimirMensagem("\n|| DOCUMENTOS ||\n");

            int retornaQuantidadeDocumento = BancoSistema.RetornarQuantidadeDocumentoUsuario(_usuario);

            if (retornaQuantidadeDocumento == 0)
            {
                MenuFuncoesUsuario.ImprimirMensagem("\nNenhum documento foi cadastrado.");
                MenuFuncoesUsuario.Continuar();

                return;
            }
            else
            {
                var listaDocumentos = BancoSistema.BuscarDocumentos(_usuario);

                string retornoListaDocuemntos = null;

                foreach (var item in listaDocumentos)
                {
                    retornoListaDocuemntos += $"\nNome: {item.Nome}\nDescrição: {item.Descricao}\n" +
                    $"Conteudo(hash): {item.Conteudo}\nId do documento: {item.IdDocumento}\nAssinante: {item.Assinador}\nStatus: {item.Status}\n";
                }

                MenuFuncoesUsuario.ImprimirMensagem(retornoListaDocuemntos);
                MenuFuncoesUsuario.Continuar();

                return;
            }
        }

        public static void CadastrarDocumento()
        {

            MenuFuncoesUsuario.ImprimirMensagem("\n|| CADASTRO DE DOCUMENTO||\n\nNome:");

            string nomeDocumento = MenuFuncoesUsuario.ReceberValorInserido();

            if (string.IsNullOrWhiteSpace(nomeDocumento))
            {
                MenuFuncoesUsuario.ImprimirErro("O nome do documento deve ser informado, tente novamente.");
                MenuFuncoesUsuario.Continuar();

                return;
            }
            else
            {
                var listaDocumentos = BancoSistema.BuscarDocumentos(_usuario);

                bool verifica = BancoSistema.VerificarDocumento(_usuario, StatusDocumento.Removido, listaDocumentos);

                if (verifica)
                {
                    MenuFuncoesUsuario.ImprimirErro("Documento já cadastrado, tente novamente.");
                    MenuFuncoesUsuario.Continuar();

                    return;
                }
                else
                {
                    MenuFuncoesUsuario.ImprimirMensagem("\nDescrição: ");
                    string descricao = MenuFuncoesUsuario.ReceberValorInserido();

                    if (string.IsNullOrWhiteSpace(descricao))
                    {
                        MenuFuncoesUsuario.ImprimirErro("Descrição deve ser informada, tente novamente.");
                        MenuFuncoesUsuario.Continuar();

                        return;
                    }
                    else
                    {
                        string listaUsuarios = ListarNomesUsuarios();

                        if (listaUsuarios == null)
                        {
                            MenuFuncoesUsuario.ImprimirMensagem("\nNenhum usuário disponível para assinar no momento.");
                            MenuFuncoesUsuario.Continuar();

                            return;
                        }
                        else
                        {
                            MenuFuncoesUsuario.ImprimirMensagem(listaUsuarios);

                            MenuFuncoesUsuario.ImprimirMensagem("\nDefina o usuário que terá permissão para verificar a assinatura neste documento: ");
                            string assinador = MenuFuncoesUsuario.ReceberValorInserido();

                            if (string.IsNullOrWhiteSpace(assinador))
                            {
                                MenuFuncoesUsuario.ImprimirErro("Informe um assinador, tente novamente.");
                                MenuFuncoesUsuario.Continuar();

                                return;
                            }
                            else
                            {
                                var usuarioAssinante = BancoSistema.RetornarUsuario(assinador);

                                if (usuarioAssinante == null)
                                {
                                    MenuFuncoesUsuario.ImprimirMensagem("\nUsuário não existe, tente novamente.");
                                    MenuFuncoesUsuario.Continuar();

                                    return;
                                }
                                else if (usuarioAssinante.Nome == _usuario.Nome)
                                {
                                    MenuFuncoesUsuario.ImprimirMensagem("\nInforme um nome que esteja na lista, tente novamente.");
                                    MenuFuncoesUsuario.Continuar();

                                    return;
                                }
                                else
                                {
                                    int Idocumento = BancoSistema.RetornarQuantidadeDocumento() + 1;

                                    string hashDecricao = SegurancaSistema.GerarHash(descricao, Idocumento.ToString());

                                    Documento documentoInserir = new Documento(nomeDocumento, descricao, hashDecricao, Idocumento, usuarioAssinante, _usuario, StatusDocumento.Pendente);

                                    BancoSistema.InserirDocumento(documentoInserir);

                                    MenuFuncoesUsuario.ImprimirMensagem("\nDocumento cadastrado com sucesso!");
                                    MenuFuncoesUsuario.Continuar();

                                    return;
                                }
                            }
                        }               
                    }
                }
            }
        }

        public static void DeletarDocumento()
        {
            MenuFuncoesUsuario.ImprimirMensagem("\n|| DOCUMENTOS ||\n");

            var retornoListaDocumentos = BancoSistema.BuscarDocumentos(_usuario, StatusDocumento.Pendente);

            string listaDocumentos = null;

            foreach (var item in retornoListaDocumentos)
            {
                listaDocumentos += $"\nNome: {item.Nome}\nDescrição: {item.Descricao}\n" +
                $"Conteudo(hash): {item.Conteudo}\nId do documento: {item.IdDocumento}\nAssinante: {item.Assinador}\nStatus: {item.Status}\n";
            }

            if (listaDocumentos == null)
            {
                MenuFuncoesUsuario.ImprimirMensagem("\nNenhum documento pendente a ser deletado.");
                MenuFuncoesUsuario.Continuar();

                return;
            }
            else
            {
                MenuFuncoesUsuario.ImprimirMensagem(listaDocumentos);

                MenuFuncoesUsuario.ImprimirMensagem("\nInforme o ID do documento que deseja ser deletado: ");
                string idDocumento = MenuFuncoesUsuario.ReceberValorInserido();

                if (string.IsNullOrWhiteSpace(idDocumento))
                {
                    MenuFuncoesUsuario.ImprimirErro("O ID do documento deve ser informado, tente novamente.");
                    MenuFuncoesUsuario.Continuar();

                    return;
                }
                else
                {
                    bool verificaDocumento = BancoSistema.VerificarDocumento(_usuario, StatusDocumento.Pendente, retornoListaDocumentos, idDocumento);

                    if (verificaDocumento)
                    {
                        var documento = BancoSistema.RetornarDocumento(_usuario, StatusDocumento.Pendente, idDocumento);

                        BancoSistema.AtualizarDocumento(documento, StatusDocumento.Removido, "Status");

                        MenuFuncoesUsuario.ImprimirMensagem("\nDocumento deletado com sucesso!");
                        MenuFuncoesUsuario.Continuar();

                        return;
                    }
                    else
                    {
                        MenuFuncoesUsuario.ImprimirErro("Documento não existe, tente novamente.");
                        MenuFuncoesUsuario.Continuar();

                        return;
                    }
                }
            }
        }

        public static void AssinarDocumento()
        {
            var retornolistaDocumentos = BancoSistema.BuscarDocumentosVinculadosAoUsuario(_usuario, StatusDocumento.Pendente);

            string listaDocumentos = null;

            foreach (var item in retornolistaDocumentos)
            {
                listaDocumentos += $"\nNome: {item.Nome}\nDescrição: {item.Descricao}\n" +
                $"Conteudo(hash): {item.Conteudo}\nId do documento: {item.IdDocumento}\nAssinante: {item.Assinador}\nStatus: {item.Status}\n";
            }
            if (listaDocumentos != null)
            {
                MenuFuncoesUsuario.ImprimirMensagem(listaDocumentos);

                MenuFuncoesUsuario.ImprimirMensagem("\nInforme o ID do documento a ser assinado: ");
                string idDocumento = MenuFuncoesUsuario.ReceberValorInserido();

                if (string.IsNullOrWhiteSpace(idDocumento))
                {
                    MenuFuncoesUsuario.ImprimirErro("O ID do documento deve ser informado, tente novamente.");
                    MenuFuncoesUsuario.Continuar();

                    return;
                }
                else
                {
                    int nroId;

                    var opcaoIntValida = Int32.TryParse(idDocumento, out nroId);

                    if (opcaoIntValida)
                    {
                        bool verificaDocumento = BancoSistema.VerificarDocumentosVinculadosAoUsuario(_usuario, nroId, retornolistaDocumentos);

                        if (verificaDocumento)
                        {
                            Documento documento = BancoSistema.RetornarDocumentoVinculadosAoUsuario(_usuario, nroId);

                            SegurancaSistema.GerarAssinatura(documento, _usuario.ChavePrivada, _usuario);
                        }
                        else
                        {
                            MenuFuncoesUsuario.ImprimirErro("Este documento não existe, tente novamente.");
                            MenuFuncoesUsuario.Continuar();

                            return;
                        }
                    }
                    else
                    {
                        MenuFuncoesUsuario.ImprimirErro("Informe uma opção válida, tente novamente.");
                        MenuFuncoesUsuario.Continuar();

                        return;
                    }

                }
            }
            else
            {
                MenuFuncoesUsuario.ImprimirMensagem("\nNenhum documento pendente de assinatura no momento.");
                MenuFuncoesUsuario.Continuar();

                return;
            }
        }
        public static void VerificarAssinatura()
        {
            var retornoListaDocumentos = BancoSistema.BuscarDocumentos(_usuario, StatusDocumento.Assinado);

            string listaDocumentos = null;

            foreach (var item in retornoListaDocumentos)
            {
                listaDocumentos += $"\nNome: {item.Nome}\nDescrição: {item.Descricao}\n" +
                $"Conteudo(hash): {item.Conteudo}\nId do documento: {item.IdDocumento}\nAssinante: {item.Assinador}\nStatus: {item.Status}\n";
            }
            if (listaDocumentos == null)
            {
                MenuFuncoesUsuario.ImprimirMensagem("\nNenhum documento foi assinado.");
                MenuFuncoesUsuario.Continuar();

                return;
            }
            else
            {
                MenuFuncoesUsuario.ImprimirMensagem($"{listaDocumentos}\nInforme o ID do documento a ser verificado: ");
                string idDocumento = MenuFuncoesUsuario.ReceberValorInserido();

                if (string.IsNullOrWhiteSpace(idDocumento))
                {
                    MenuFuncoesUsuario.ImprimirErro("O ID do documento deve ser informado, tente novamente.");
                    MenuFuncoesUsuario.Continuar();

                    return;
                }
                else
                {
                    bool verificarDocumento = BancoSistema.VerificarDocumento(_usuario, StatusDocumento.Assinado, retornoListaDocumentos, idDocumento);

                    if (verificarDocumento)
                    {
                        Documento documento = BancoSistema.RetornarDocumento(_usuario, StatusDocumento.Assinado, idDocumento);

                        SegurancaSistema.VerificarAssinatura(documento);
                    }
                    else
                    {
                        MenuFuncoesUsuario.ImprimirErro("Documento não encontrado, tente novamente.");
                        MenuFuncoesUsuario.Continuar();

                        return;
                    }
                }
            }
        }
        #endregion      
    }

}