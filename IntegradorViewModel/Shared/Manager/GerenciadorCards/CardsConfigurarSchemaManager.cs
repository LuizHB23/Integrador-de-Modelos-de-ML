using AutoMapper;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.ModeloEtapas;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Shared.Manager.GerenciadorCards
{
    public class CardsConfigurarSchemaManager : CardsManager<ConfiguracaoCardSchemaViewModel>
    {
        public CardsConfigurarSchemaManager(ObservableCollection<ConfiguracaoCardSchemaViewModel> cardsLista, ObservableCollection<int> posicoesLista, ModeloDTO modelo) : base(cardsLista, posicoesLista, modelo) { }

        public void AdicinarColuna(SchemaItemViewModel schemaItem, Func<ConfiguracaoCardSchemaViewModel, Task> funcExcluir, Func<ConfiguracaoCardSchemaViewModel, int, Task> funcTrocarPosicao)
        {
            if (schemaItem is null)
            {
                return;
            }

            var cardSchema = new ConfiguracaoCardSchemaViewModel(schemaItem, funcExcluir, funcTrocarPosicao);
            _cardsLista.Add(cardSchema);
            _posicoesLista.Add(schemaItem.Posicao);
            AtualizaPosicoes();
        }

        public async Task CarregarSchema(IDialogService _dialogService, IConversorJson conversor, IMapper mapper)
        {
            var _caminhoJson = _dialogService.GetCaminhoArquivo();

            if (string.IsNullOrWhiteSpace(_caminhoJson))
            {
                return;
            }

            _cardsLista.Clear();
            _posicoesLista.Clear();

            var schema = await conversor.CarregarJsonAsync<Dictionary<int, SchemaDTO>>(_caminhoJson);

            if(schema.Count == 0)
            {
                return;
            }

            foreach (var card in schema)
            {
                var schemaItem = new SchemaItemViewModel(card.Key, card.Value.NomeColuna, card.Value.Finalidade, card.Value.Tipo, card.Value.Categorico);
                var cardSchema = new ConfiguracaoCardSchemaViewModel(schemaItem, RemoverCard, OrganizaPosicao);
                _cardsLista.Add(cardSchema);
                _posicoesLista.Add(card.Key);
            }

            AtualizaPosicoes();

            string nomeModelo = _modelo.NomeModelo;

            SchemaConfiguracao schemaConfiguracao = new(nomeModelo, "1.0", mapper.Map<Dictionary<int, Schema>>(schema));

            await conversor.ConverteJsonAsync(schemaConfiguracao, nomeModelo);
        }

        public async Task PreparaParaJson(IConversorJson conversor, string nomeModelo)
        {
            var schemaNovo = new Dictionary<int, Schema>();




            //Parte do Versionador





            foreach (var card in _cardsLista)
            {
                var schema = new Schema()
                    {
                        NomeColuna = card.NomeColuna,
                        Finalidade = card.Finalidade, 
                        Tipo = card.Tipo, 
                        Categorico = card.Categorico
                    };

                schemaNovo.Add(card.Posicao, schema);
            }

            var schemaConfiguracao = new SchemaConfiguracao(nomeModelo, "1.0", schemaNovo);

            await conversor.ConverteJsonAsync(schemaConfiguracao, nomeModelo);
        }

        public override void AtualizaPosicoes() => base.AtualizaPosicoes();

        public override async Task OrganizaPosicao(ConfiguracaoCardSchemaViewModel card, int posicaoNova) => await base.OrganizaPosicao(card, posicaoNova);

        public override async Task RemoverCard(ConfiguracaoCardSchemaViewModel card) => await base.RemoverCard(card);
    }
}
