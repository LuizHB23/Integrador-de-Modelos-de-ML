
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace IntegradorViewModel.Shared.Manager.GerenciadorCards
{
    public class CardsPipelineModeloManager : CardsManager<ConfiguracaoCardFuncaoViewModel>
    {
        public CardsPipelineModeloManager(ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsLista, ObservableCollection<int> posicoesLista) : base(cardsLista, posicoesLista) { }

        public void AdicionarCard(FuncaoItemViewModel funcaoItem, Func<ConfiguracaoCardFuncaoViewModel, Task> actionExcluir, Action<ConfiguracaoCardFuncaoViewModel, int> actionTrocarPosicao, Action<ConfiguracaoCardFuncaoViewModel> actionConfigurarFuncao)
        {
            if (funcaoItem is null)
            {
                return;
            }

            var cardFuncao = new ConfiguracaoCardFuncaoViewModel(funcaoItem, actionExcluir, actionTrocarPosicao, actionConfigurarFuncao);
            _cardsLista.Add(cardFuncao);
            _posicoesLista.Add(cardFuncao.Posicao);
            AtualizaPosicoes();
        }

        public void CarregarPipeline(IDialogService _dialogService, IConverteJson<Dictionary<int, FuncaoDTO>> _converter, Action<ConfiguracaoCardFuncaoViewModel> actionConfigurarFuncao, Func<ConfiguracaoCardFuncaoViewModel, Task> functionRemover)
        {
            var _caminhoJson = _dialogService.GetCaminhoArquivo();

            if (string.IsNullOrWhiteSpace(_caminhoJson))
            {
                throw new Exception();
            }

            _cardsLista.Clear();
            _posicoesLista.Clear();

            var schema = _converter.CarregarJson(_caminhoJson);

            foreach (var card in schema)
            {
                var funcaoItem = new FuncaoItemViewModel(card.Key, card.Value.NomeFuncao, card.Value.Codigo);
                var cardFuncao = new ConfiguracaoCardFuncaoViewModel(funcaoItem, functionRemover, OrganizaPosicao, actionConfigurarFuncao);
                _cardsLista.Add(cardFuncao);
                _posicoesLista.Add(card.Key);
            }

            AtualizaPosicoes();
        }

        public void PreparaParaJson(IConverteJson<Dictionary<int, FuncaoDTO>> _converter, string nomeModelo)
        {
            var pipelineNovo = new Dictionary<int, FuncaoDTO>();

            foreach (var card in _cardsLista)
            {
                var funcao = new FuncaoDTO(card.FuncaoItem.NomeFuncao, card.FuncaoItem.Codigo, nomeModelo);
                pipelineNovo.Add(card.Posicao, funcao);
            }

            _converter.ConverteJson(pipelineNovo);
        }

        public override void AtualizaPosicoes() => base.AtualizaPosicoes();

        public override void OrganizaPosicao(ConfiguracaoCardFuncaoViewModel card, int posicaoNova) => base.OrganizaPosicao(card, posicaoNova);

        public override void RemoverCard(ConfiguracaoCardFuncaoViewModel card) => base.RemoverCard(card);
    }
}
