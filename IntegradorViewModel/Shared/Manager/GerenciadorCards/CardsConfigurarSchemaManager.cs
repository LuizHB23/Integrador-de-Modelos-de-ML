using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.ConversorJson;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;

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

        public void CarregarSchema(IDialogService _dialogService, ConversorJson conversor)
        {
            var _caminhoJson = _dialogService.GetCaminhoArquivo();

            if (string.IsNullOrWhiteSpace(_caminhoJson))
            {
                return;
            }

            _cardsLista.Clear();
            _posicoesLista.Clear();

            var schema = conversor.CarregarJson<Dictionary<int, SchemaDTO>>(_caminhoJson);

            foreach (var card in schema)
            {
                var schemaItem = new SchemaItemViewModel(card.Key, card.Value.NomeColuna, card.Value.Finalidade, card.Value.Tipo, card.Value.Categorico);
                var cardSchema = new ConfiguracaoCardSchemaViewModel(schemaItem, RemoverCard, OrganizaPosicao);
                _cardsLista.Add(cardSchema);
                _posicoesLista.Add(card.Key);
            }

            AtualizaPosicoes();
        }

        public void PreparaParaJson(ConversorJson conversor, string nomeModelo)
        {
            var schemaNovo = new Dictionary<int, SchemaDTO>();

            foreach (var card in _cardsLista)
            {
                var schema = new SchemaDTO(card.NomeColuna, card.Finalidade, card.Tipo, card.Categorico) { NomeModelo = nomeModelo };
                schemaNovo.Add(card.Posicao, schema);
            }

            conversor.ConverteJson(schemaNovo);
        }

        public override void AtualizaPosicoes() => base.AtualizaPosicoes();

        public override void OrganizaPosicao(ConfiguracaoCardSchemaViewModel card, int posicaoNova) => base.OrganizaPosicao(card, posicaoNova);

        public override void RemoverCard(ConfiguracaoCardSchemaViewModel card) => base.RemoverCard(card);
    }
}
