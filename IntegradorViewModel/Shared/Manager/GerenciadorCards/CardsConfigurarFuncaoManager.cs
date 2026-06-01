using IntegradorAplicacao.DTO;
using IntegradorAplicacao.DTO.Interfaces;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.Configuracao.Interfaces;
using IntegradorDominio.Models.ModeloEtapas;
using IntegradorViewModel.ControleUsuario;
using IntegradorViewModel.ItensViewModel;
using IntegradorViewModel.Shared.Factory;
using System.Collections.ObjectModel;
using System.IO.Pipelines;

namespace IntegradorViewModel.Shared.Manager.GerenciadorCards
{
    public class CardsConfigurarFuncaoManager<TIn, TOut> : 
        CardsManager<ConfiguracaoCardFuncaoViewModel> 
        where TIn : IPipelineDTO 
        where TOut : class, IPipelineConfiguracao
    {
        public CardsConfigurarFuncaoManager(ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsLista, ObservableCollection<int> posicoesLista, ModeloDTO modelo) : base(cardsLista, posicoesLista, modelo) { }

        public void AdicionarCard(FuncaoItemViewModel funcaoItem, Func<ConfiguracaoCardFuncaoViewModel, Task> functionExcluir, Func<ConfiguracaoCardFuncaoViewModel, int, Task> funcTrocarPosicao, Func<ConfiguracaoCardFuncaoViewModel, Task> functionConfigurarFuncao)
        {
            if (funcaoItem is null)
            {
                return;
            }

            var cardFuncao = new ConfiguracaoCardFuncaoViewModel(funcaoItem, functionExcluir, funcTrocarPosicao, functionConfigurarFuncao);
            _cardsLista.Add(cardFuncao);
            _posicoesLista.Add(cardFuncao.Posicao);
            AtualizaPosicoes();
        }

        public async Task CarregarPipeline(Func<ConfiguracaoCardFuncaoViewModel, Task> actionConfigurarFuncao, Func<ConfiguracaoCardFuncaoViewModel, Task> functionRemover, Dictionary<int, Pipeline> pipeline)
        {
            _cardsLista.Clear();
            _posicoesLista.Clear();

            foreach (var card in pipeline.OrderBy(x => x.Key))
            {
                var funcaoItem = new FuncaoItemViewModel(card.Key, card.Value.NomeFuncao, card.Value.Codigo);
                var cardFuncao = new ConfiguracaoCardFuncaoViewModel(funcaoItem, functionRemover, OrganizaPosicao, actionConfigurarFuncao);
                _cardsLista.Add(cardFuncao);
                _posicoesLista.Add(card.Key);
            }

            AtualizaPosicoes();
        }

        public async Task PreparaParaJson<F>(IConversorJson conversor, string nomeModelo) where F : IPipelineExecutorFactory<TIn, TOut>
        {
            var pipelineNovo = F.Criar(_cardsLista, nomeModelo);

            if(typeof(TOut) == typeof(PipelineSaidaInferenciaConfiguracao))
            {
                await conversor.ConverteJsonAsync(pipelineNovo, nomeModelo);
            }
            else
            {
                await conversor.ConverteJsonAsync(pipelineNovo.First(), nomeModelo);
            }

        }

        public override async Task RemoverCard(ConfiguracaoCardFuncaoViewModel card)
        {
            if(card.Posicao == _posicoesLista.Count)
            {
                await base.RemoverCard(card);
            }
            else
            {
                throw new Exception("Não é possível remover funções do meio do pipeline");
            }
        }

        public override void AtualizaPosicoes() => base.AtualizaPosicoes();

        public override async Task OrganizaPosicao(ConfiguracaoCardFuncaoViewModel card, int posicaoNova) => await base.OrganizaPosicao(card, posicaoNova);
    }
}
