
using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Shared.Manager.GerenciadorCards
{
    public class CardsPipelineModeloManager : CardsManager<ConfiguracaoCardFuncaoViewModel>
    {
        public CardsPipelineModeloManager(ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsLista, ObservableCollection<int> posicoesLista) : base(cardsLista, posicoesLista) { }

        public void AdicinarColuna(FuncaoItemViewModel funcaoItem, Action<ConfiguracaoCardFuncaoViewModel> actionExcluir, Action<ConfiguracaoCardFuncaoViewModel, int> actionTrocarPosicao)
        {
            if (funcaoItem is null)
            {
                return;
            }

            var cardFuncao = new ConfiguracaoCardFuncaoViewModel(funcaoItem, actionExcluir, actionTrocarPosicao);
            _cardsLista.Add(new ConfiguracaoCardFuncaoViewModel(funcaoItem, actionExcluir, actionTrocarPosicao));
            _posicoesLista.Add(cardFuncao.Posicao);
            AtualizaPosicoes();
        }

        public void CarregarSchema(IDialogService _dialogService, IConverteJson<Dictionary<int, FuncaoDTO>> _converter)
        {
            var _caminhoJson = _dialogService.GetCaminhoArquivo();

            if (string.IsNullOrWhiteSpace(_caminhoJson))
            {
                return;
            }

            _cardsLista.Clear();
            _posicoesLista.Clear();

            var schema = _converter.CarregarJson(_caminhoJson);

            foreach (var card in schema)
            {
                var funcaoItem = new FuncaoItemViewModel(card.Key, card.Value.NomeFuncao, card.Value.Codigo);
                var cardFuncao = new ConfiguracaoCardFuncaoViewModel(funcaoItem, RemoverColuna, OrganizaPosicao);
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

        public override void RemoverColuna(ConfiguracaoCardFuncaoViewModel card) => base.RemoverColuna(card);
    }
}
