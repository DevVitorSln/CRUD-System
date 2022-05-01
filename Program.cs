using ProgramaCadastroLogin.Enums;
using ProgramaCadastroLogin.Funcoes;
using System.Text.Json;

namespace SistemaCRUD
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                MenuFuncoesUsuario.ImprimirMensagem("Carregando...");
                SegurancaSistema.CarregarConfiguracao();

                OpcoesIniciaisUsuario enumOpcoes = OpcoesIniciaisUsuario.Fechar;

                do
                {
                    bool validaLoginUsuario = FuncoesUsuario.ValidaLoginUsuario();

                    if (validaLoginUsuario)
                    {
                        enumOpcoes = MenuFuncoesUsuario.ReceberOpcaoUsuario();

                        switch (enumOpcoes)
                        {
                            case OpcoesIniciaisUsuario.CadastrarUsuario:
                                FuncoesUsuario.CadastrarUsuario();
                                break;

                            case OpcoesIniciaisUsuario.Login:
                                FuncoesUsuario.Login();
                                break;

                            case OpcoesIniciaisUsuario.Fechar:
                                return;

                            default:
                                MenuFuncoesUsuario.ImprimirErro("Opção Indisponível,Informe uma outra opção");
                                break;
                        }
                    }
                    else
                    {
                        MenuFuncoesUsuario.MenuUuarioLogado();
                        OpcoesUsuarioLogado enumOpcoesUsuarioLogado = MenuFuncoesUsuario.ReceberOpcaoUsuarioLogado();

                        switch (enumOpcoesUsuarioLogado)
                        {
                            case OpcoesUsuarioLogado.CadastrarDocumento:
                                FuncoesUsuario.CadastrarDocumento();
                                break;

                            case OpcoesUsuarioLogado.ListarDocumentos:
                                FuncoesUsuario.ListarAtributosDocumentosUsuario();
                                break;

                            case OpcoesUsuarioLogado.ListarDocumentosAssinados:
                                FuncoesUsuario.ListarDocumentosAssinados();
                                break;

                            case OpcoesUsuarioLogado.AssinarDocumento:
                                FuncoesUsuario.AssinarDocumento();
                                break;

                            case OpcoesUsuarioLogado.DeletarDocumento:
                                FuncoesUsuario.DeletarDocumento();
                                break;

                            case OpcoesUsuarioLogado.VerificarAssinatura:
                                FuncoesUsuario.VerificarAssinatura();
                                break;


                            case OpcoesUsuarioLogado.AtualizarSenha:
                                FuncoesUsuario.AlterarSenha();
                                break;


                            case OpcoesUsuarioLogado.DeletarConta:
                                FuncoesUsuario.Remover();
                                break;

                            case OpcoesUsuarioLogado.Sair:
                                FuncoesUsuario.Logout();
                                break;

                            default:
                                MenuFuncoesUsuario.ImprimirErro("Informe uma opção válida, tente novamente.");
                                MenuFuncoesUsuario.Continuar();
                                break;
                        }
                    }

                } while (enumOpcoes != OpcoesIniciaisUsuario.Fechar);
            }
            catch (ArgumentNullException ex)
            {
                MenuFuncoesUsuario.ImprimirErro(ex.Message);
            }
            catch (ArgumentException ex)
            {
                MenuFuncoesUsuario.ImprimirErro(ex.Message);
            }
            catch (JsonException ex)
            {
                MenuFuncoesUsuario.ImprimirErro(ex.Message);
            }
            catch (Exception ex)
            {
                MenuFuncoesUsuario.ImprimirErro("Ocorreu um erro na aplicação.");
            }
            finally
            {
                try
                {
                    BancoSistema.Dispose();
                    MenuFuncoesUsuario.Continuar("\nAperte uma tecla para finalizar a aplicação.");
                }
                catch (Exception)
                {
                    MenuFuncoesUsuario.Continuar("\nAperte uma tecla para finalizar a aplicação.");
                }
            }
        }
    }
}