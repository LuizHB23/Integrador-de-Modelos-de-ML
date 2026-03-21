using IntegradorAplicacao.DTO;

namespace IntegradorViewModel.Context
{
    public class CarregarDadosContext : IContext<ArquivoDadosDTO>
    {
        private ArquivoDadosDTO _mensagem = new ArquivoDadosDTO(string.Empty, string.Empty, string.Empty, string.Empty, false);

        public void EnviaMensagem(ArquivoDadosDTO mensagem) => _mensagem = mensagem;
        public ArquivoDadosDTO RecebeMensagem() => _mensagem;
    }
}
