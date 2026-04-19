using IntegradorAplicacao.DTO;
using System.Text;

namespace IntegradorViewModel.Shared.Context
{
    public class CarregarDadosContext : IContext<ArquivoDadosDTO>
    {
        private ArquivoDadosDTO _mensagem = new ArquivoDadosDTO(string.Empty, ' ', Encoding.UTF8, ' ', false);

        public void EnviaMensagem(ArquivoDadosDTO mensagem) => _mensagem = mensagem;
        public ArquivoDadosDTO RecebeMensagem() => _mensagem;
    }
}
