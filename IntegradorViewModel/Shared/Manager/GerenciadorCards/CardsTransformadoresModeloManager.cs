using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.ModeloEtapas;
using IntegradorViewModel.ControleUsuario.ConfiguracaoCard;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Shared.Manager.GerenciadorCards
{
    public class CardsTransformadoresModeloManager : CardsManager<ConfiguracaoCardTransformadorViewModel>
    {
        public CardsTransformadoresModeloManager(ObservableCollection<ConfiguracaoCardTransformadorViewModel> cardsLista, ObservableCollection<int> posicoesLista, ModeloDTO modelo) : base(cardsLista, posicoesLista, modelo) { }

        public void AdicinarColuna(TransformadorItemViewModel transformadorItem, Func<ConfiguracaoCardTransformadorViewModel, Task> funcExcluir, Func<ConfiguracaoCardTransformadorViewModel, int, Task> funcTrocarPosicao)
        {
            if (transformadorItem is null)
            {
                return;
            }

            var cardSchema = new ConfiguracaoCardTransformadorViewModel(transformadorItem, funcExcluir, funcTrocarPosicao);
            _cardsLista.Add(cardSchema);
            _posicoesLista.Add(transformadorItem.Posicao);
            AtualizaPosicoes();
        }

        public async Task CarregarTransformador(IDialogService _dialogService, IConversorJson conversor)
        {
            var _caminhoJson = _dialogService.GetCaminhoArquivo();

            if (string.IsNullOrWhiteSpace(_caminhoJson))
            {
                return;
            }

            _cardsLista.Clear();
            _posicoesLista.Clear();

            var transformador = await conversor.CarregarJsonAsync<Dictionary<int, TransformadorDTO>>(_caminhoJson);

            foreach (var card in transformador)
            {
                var schemaItem = new TransformadorItemViewModel(card.Key, card.Value.NomeTransformador, card.Value.CaminhoTransformador);
                var cardSchema = new ConfiguracaoCardTransformadorViewModel(schemaItem, RemoverCard, OrganizaPosicao);
                _cardsLista.Add(cardSchema);
                _posicoesLista.Add(card.Key);
            }

            AtualizaPosicoes();
        }

        public async Task PreparaParaJson(IConversorJson conversor, string nomeModelo)
        {
            var transformadorNovo = new Dictionary<int, Transformador>();

            foreach (var card in _cardsLista)
            {
                var transformador = new Transformador()
                {
                    NomeTransformador = card.NomeTransformador, 
                    CaminhoTransformador = card.CaminhoProvisorio
                };
                transformadorNovo.Add(card.Posicao, transformador);
            }

            var listaTransformadoresConfiguracao = new List<TransformadorConfiguracao>()
            {
                new TransformadorConfiguracao(nomeModelo, "1.0", transformadorNovo)
            };

            await conversor.ConverteJsonAsync(listaTransformadoresConfiguracao, nomeModelo);
        }

        public override void AtualizaPosicoes() => base.AtualizaPosicoes();

        public override async Task OrganizaPosicao(ConfiguracaoCardTransformadorViewModel card, int posicaoNova) => await base.OrganizaPosicao(card, posicaoNova);

        public override async Task RemoverCard(ConfiguracaoCardTransformadorViewModel card) => await base.RemoverCard(card);
    }
}