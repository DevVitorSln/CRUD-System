using ProgramaCadastroLogin.Enums;
using System.Text.RegularExpressions;

namespace ProgramaCadastroLogin.Funcoes
{
    public static class MenuFuncoesUsuario
    {

        private static readonly string _menu;

        private static string _menuLogin;

        private const string _messagemSelecionOpcao = "\nInforme uma opção: ";

        static MenuFuncoesUsuario()
        {
            _menu = "[OPÇÕES]";

            foreach (var value in Enum.GetValues(typeof(OpcoesIniciaisUsuario)))
            {
                string enumName = Enum.GetName(typeof(OpcoesIniciaisUsuario), value);

                var names = Regex.Split(enumName, @"(?<!^)(?=[A-Z]|[0-9])");

                enumName = string.Empty;

                foreach (var name in names)
                {
                    enumName += $"{name} ";
                }

                _menu += $"\n{(int)value}.{enumName}";
            }
        }
        public static void MenuUuarioLogado()
        {
            _menuLogin = "[OPÇÕES]";

            foreach (var value in Enum.GetValues(typeof(OpcoesUsuarioLogado)))
            {
                string enumName = Enum.GetName(typeof(OpcoesUsuarioLogado), value);

                var names = Regex.Split(enumName, @"(?<!^)(?=[A-Z]|[0-9])");

                enumName = string.Empty;

                foreach (var name in names)
                {
                    enumName += $"{name} ";
                }

                _menuLogin += $"\n{(int)value}.{enumName}";
            }
        }

        public static OpcoesIniciaisUsuario ReceberOpcaoUsuario()
        {
            Console.Clear();
            ImprimirMensagem($"{_menu}\n{_messagemSelecionOpcao}");

            string opcao = MenuFuncoesUsuario.ReceberValorInserido();
            if (string.IsNullOrWhiteSpace(opcao))
            {
                throw new ArgumentNullException("\n[ERROR]: Informe uma opção.");
            }

            int nroOpcao;

            var opcaoIntValida = Int32.TryParse(opcao, out nroOpcao);

            if (opcaoIntValida)
            {
                bool enumValido = Enum.IsDefined(typeof(OpcoesIniciaisUsuario), nroOpcao);

                if (enumValido)
                    return (OpcoesIniciaisUsuario)nroOpcao;
            }

            throw new ArgumentException("\nInforme uma opção válida.");
        }
        public static OpcoesUsuarioLogado ReceberOpcaoUsuarioLogado()
        {
            Console.Clear();
            MenuFuncoesUsuario.ImprimirMensagem($"{_menuLogin}\n{_messagemSelecionOpcao}");

            string opcao = MenuFuncoesUsuario.ReceberValorInserido();

            int nroOpcao;

            bool opcaoIntValida = Int32.TryParse(opcao, out nroOpcao);

            OpcoesUsuarioLogado enumOpcoesUsuarioLogado = (OpcoesUsuarioLogado)nroOpcao;

            return enumOpcoesUsuarioLogado;
        }
        public static string ReceberValorInserido()
        {
            return Console.ReadLine();
        }

        public static void ImprimirErro(string mensagem)
        {
            Console.WriteLine($"\n[ERROR]{mensagem}");
        }
        public static void ImprimirMensagem(string mensagem)
        {
            Console.Write($"{mensagem}");
        }
        public static void Continuar(string mensagem = null)
        {
            if (mensagem == null)
            {
                ImprimirMensagem("\nAperte uma tecla para voltar ao Menu...");
            }
            else
            {
                ImprimirMensagem($"{mensagem}");
            }

            Console.ReadKey();
        }


    }
}