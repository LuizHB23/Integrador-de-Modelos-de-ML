using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace IntegradorViewModel.Shared.Manager.GerenciadorCards
{
    public class CardsConfigurarSchemaManager : CardsManager<ConfiguracaoCardSchemaViewModel>
    {
        public CardsConfigurarSchemaManager(ObservableCollection<ConfiguracaoCardSchemaViewModel> cardsLista, ObservableCollection<int> posicoesLista) : base(cardsLista, posicoesLista) { }

        public void AdicinarColuna(SchemaItemViewModel schemaItem, Action<ConfiguracaoCardSchemaViewModel> actionExcluir, Action<ConfiguracaoCardSchemaViewModel, int> actionTrocarPosicao)
        {
            if (schemaItem is null)
            {
                return;
            }

            var cardSchema = new ConfiguracaoCardSchemaViewModel(schemaItem, actionExcluir, actionTrocarPosicao);
            _cardsLista.Add(cardSchema);
            _posicoesLista.Add(schemaItem.Posicao);
            AtualizaPosicoes();
        }

        public void CarregarSchema(IDialogService _dialogService, IConverteJson<Dictionary<int, SchemaDTO>> _converter)
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
                var schemaItem = new SchemaItemViewModel(card.Key, card.Value.NomeColuna, card.Value.Finalidade, card.Value.Tipo, card.Value.Categorico);
                var cardSchema = new ConfiguracaoCardSchemaViewModel(schemaItem, RemoverCard, OrganizaPosicao);
                _cardsLista.Add(cardSchema);
                _posicoesLista.Add(card.Key);
            }

            AtualizaPosicoes();
        }

        public void PreparaParaJson(IConverteJson<Dictionary<int, SchemaDTO>> _converter, string nomeModelo)
        {
            var schemaNovo = new Dictionary<int, SchemaDTO>();

            foreach (var card in _cardsLista)
            {
                var schema = new SchemaDTO(card.NomeColuna, card.Finalidade, card.Tipo, card.Categorico) { NomeModelo = nomeModelo };
                schemaNovo.Add(card.Posicao, schema);
            }

            _converter.ConverteJson(schemaNovo);
        }

        public override void AtualizaPosicoes() => base.AtualizaPosicoes();

        public override void OrganizaPosicao(ConfiguracaoCardSchemaViewModel card, int posicaoNova) => base.OrganizaPosicao(card, posicaoNova);

        public override void RemoverCard(ConfiguracaoCardSchemaViewModel card) => base.RemoverCard(card);
    }
}
