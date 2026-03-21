using IntegradorAplicacao.DTO;

namespace IntegradorViewModel.Context
{
    public class NomeModeloContext : IContext<ModeloDTO>
    {
        private ModeloDTO _mensagem = new ModeloDTO(string.Empty, string.Empty, string.Empty);

        public void EnviaMensagem(ModeloDTO mensagem) => _mensagem = mensagem;
        public ModeloDTO RecebeMensagem() => _mensagem;
    }
}
