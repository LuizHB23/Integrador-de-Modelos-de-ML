using IntegradorAplicacao.DTO;
using IntegradorDominio.Models.Configuracao;
using IntegradorDominio.Models.Configuracao.Interfaces;
using IntegradorDominio.Models.ModeloEtapas;
using IntegradorViewModel.ControleUsuario;
using System.Collections.ObjectModel;

namespace IntegradorViewModel.Shared.Factory
{
    public interface IPipelineExecutorFactory<TIn, TOut> where TOut : IPipelineConfiguracao
    {
        static abstract List<TOut> Criar(ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsLista, string nomeModelo);
    }

    public class FuncaoDTOFactory : IPipelineExecutorFactory<FuncaoDTO, PipelineTratamentoConfiguracao>
    {
        public static List<PipelineTratamentoConfiguracao> Criar(ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsLista, string nomeModelo)
        {
            var pipelineNovo = new Dictionary<int, Pipeline>();

            foreach (var card in cardsLista)
            {
                var pipeline = new Pipeline()
                {
                    NomeFuncao = card.FuncaoItem.NomeFuncao,
                    Codigo = card.FuncaoItem.Codigo
                };

                pipelineNovo.Add(card.Posicao, pipeline);
            }

            var pipelineTratamentoConfiguracao = new List<PipelineTratamentoConfiguracao>()
            {
                new PipelineTratamentoConfiguracao(nomeModelo, "1.0", pipelineNovo)
            };

            return pipelineTratamentoConfiguracao;
        }
    }

    public class SaidaDTOFactory : IPipelineExecutorFactory<SaidaDTO, PipelineSaidaInferenciaConfiguracao>
    {
        public static List<PipelineSaidaInferenciaConfiguracao> Criar(ObservableCollection<ConfiguracaoCardFuncaoViewModel> cardsLista, string nomeModelo)
        {
            var pipelineNovo = new Dictionary<int, Pipeline>();

            foreach (var card in cardsLista)
            {
                var pipeline = new Pipeline()
                {
                    NomeFuncao = card.FuncaoItem.NomeFuncao,
                    Codigo = card.FuncaoItem.Codigo
                };

                pipelineNovo.Add(card.Posicao, pipeline);
            }

            var pipelineTratamentoConfiguracao = new List<PipelineSaidaInferenciaConfiguracao>()
            {
                new PipelineSaidaInferenciaConfiguracao(nomeModelo, "1.0", pipelineNovo)
            };

            return pipelineTratamentoConfiguracao;
        }
    }
}
