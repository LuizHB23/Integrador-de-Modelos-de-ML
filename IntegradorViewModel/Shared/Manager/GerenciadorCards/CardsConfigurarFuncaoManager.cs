using IntegradorAplicacao.DTO.Interfaces;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Factory;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Shared.Manager.GerenciadorCards
{
    public class CardsConfigurarFuncaoManager<T> : CardsManager<ConfiguracaoCardFuncaoViewModel> where T : IPipelineExecutor
    {
        public CardsConfigurarFuncaoManager(ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsLista, ObservableCollection<int> posicoesLista) : base(cardsLista, posicoesLista) { }

        public void AdicionarCard(FuncaoItemViewModel funcaoItem, Func<ConfiguracaoCardFuncaoViewModel, Task> functionExcluir, Action<ConfiguracaoCardFuncaoViewModel, int> actionTrocarPosicao, Func<ConfiguracaoCardFuncaoViewModel, Task> functionConfigurarFuncao)
        {
            if (funcaoItem is null)
            {
                return;
            }

            var cardFuncao = new ConfiguracaoCardFuncaoViewModel(funcaoItem, functionExcluir, actionTrocarPosicao, functionConfigurarFuncao);
            _cardsLista.Add(cardFuncao);
            _posicoesLista.Add(cardFuncao.Posicao);
            AtualizaPosicoes();
        }

        public async Task CarregarPipeline(IConversorJson conversor, Func<ConfiguracaoCardFuncaoViewModel, Task> actionConfigurarFuncao, Func<ConfiguracaoCardFuncaoViewModel, Task> functionRemover,
            string caminhoJson)
        {
            _cardsLista.Clear();
            _posicoesLista.Clear();

            var schema = await conversor.CarregarJsonAsync<Dictionary<int, T>>(caminhoJson);

            foreach (var card in schema.OrderBy(x => x.Key))
            {
                var funcaoItem = new FuncaoItemViewModel(card.Key, card.Value.NomeFuncao, card.Value.Codigo);
                var cardFuncao = new ConfiguracaoCardFuncaoViewModel(funcaoItem, functionRemover, OrganizaPosicao, actionConfigurarFuncao);
                _cardsLista.Add(cardFuncao);
                _posicoesLista.Add(card.Key);
            }

            AtualizaPosicoes();
        }

        public async Task PreparaParaJson<F>(IConversorJson conversor, string nomeModelo) where F : IPipelineExecutorFactory<T>
        {
            var pipelineNovo = new Dictionary<int, T>();

            foreach (var card in _cardsLista)
            {
                var pipeline = F.Criar(card.FuncaoItem.NomeFuncao, card.FuncaoItem.Codigo, nomeModelo);
                pipelineNovo.Add(card.Posicao, pipeline);
            }

            await conversor.ConverteJsonAsync(pipelineNovo);
        }

        public override void RemoverCard(ConfiguracaoCardFuncaoViewModel card)
        {
            if(card.Posicao == _posicoesLista.Count)
            {
                base.RemoverCard(card);
            }
            else
            {
                throw new Exception("Não é possível remover funções do meio do pipeline");
            }
        }

        public override void AtualizaPosicoes() => base.AtualizaPosicoes();

        public override void OrganizaPosicao(ConfiguracaoCardFuncaoViewModel card, int posicaoNova) => base.OrganizaPosicao(card, posicaoNova);
    }
}
