using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.ConversorJSON;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ControleUsuario.ConfiguracaoCard;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IntegradorViewModel.Shared.Manager.GerenciadorCards
{
    public class CardsTransformadoresModeloManager : CardsManager<ConfiguracaoCardTransformadorViewModel>
    {
        public CardsTransformadoresModeloManager(ObservableCollection<ConfiguracaoCardTransformadorViewModel> cardsLista, ObservableCollection<int> posicoesLista) : base(cardsLista, posicoesLista) { }

        public void AdicinarColuna(TransformadorItemViewModel transformadorItem, Action<ConfiguracaoCardTransformadorViewModel> actionExcluir, Action<ConfiguracaoCardTransformadorViewModel, int> actionTrocarPosicao)
        {
            if (transformadorItem is null)
            {
                return;
            }

            var cardSchema = new ConfiguracaoCardTransformadorViewModel(transformadorItem, actionExcluir, actionTrocarPosicao);
            _cardsLista.Add(cardSchema);
            _posicoesLista.Add(transformadorItem.Posicao);
            AtualizaPosicoes();
        }

        public void CarregarTransformador(IDialogService _dialogService, IConverteJson<Dictionary<int, TransformadorDTO>> _converter)
        {
            var _caminhoJson = _dialogService.GetCaminhoArquivo();

            if (string.IsNullOrWhiteSpace(_caminhoJson))
            {
                return;
            }

            _cardsLista.Clear();
            _posicoesLista.Clear();

            var transformador = _converter.CarregarJson(_caminhoJson);

            foreach (var card in transformador)
            {
                var schemaItem = new TransformadorItemViewModel(card.Key, card.Value.NomeTransformador, card.Value.CaminhoTransformador);
                var cardSchema = new ConfiguracaoCardTransformadorViewModel(schemaItem, RemoverCard, OrganizaPosicao);
                _cardsLista.Add(cardSchema);
                _posicoesLista.Add(card.Key);
            }

            AtualizaPosicoes();
        }

        public void PreparaParaJson(IConverteJson<Dictionary<int, TransformadorDTO>> _converter, string nomeModelo)
        {
            var transformadorNovo = new Dictionary<int, TransformadorDTO>();

            foreach (var card in _cardsLista)
            {
                var transformador = new TransformadorDTO(card.NomeTransformador, card.CaminhoProvisorio) { NomeModelo = nomeModelo };
                transformadorNovo.Add(card.Posicao, transformador);
            }

            _converter.ConverteJson(transformadorNovo);
        }

        public override void AtualizaPosicoes() => base.AtualizaPosicoes();

        public override void OrganizaPosicao(ConfiguracaoCardTransformadorViewModel card, int posicaoNova) => base.OrganizaPosicao(card, posicaoNova);

        public override void RemoverCard(ConfiguracaoCardTransformadorViewModel card) => base.RemoverCard(card);
    }
}