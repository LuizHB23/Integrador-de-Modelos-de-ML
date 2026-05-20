using IntegradorDominio.AST;
using IntegradorDominio.Models.DataFrameModel;

namespace IntegradorAplicacao.Aplicacao.PipelineAplicacao.ExecutorAplicacao
{
    public class BuilderExecutor
    {
        private Dictionary<string, object?> _objetosUtilizados;
        private List<ComandoMetodoPipeline> _listaComandos;
        private Queue<EtapaExecucao> _etapasExecutor;
        private string _dataFrameRetorno;

        public BuilderExecutor(Dictionary<string, object?> objetosUtilizados)
        {
            _objetosUtilizados = objetosUtilizados;
            _etapasExecutor = new();
            _listaComandos = new();
            _dataFrameRetorno = string.Empty;
        }

        public DataFrame ExecutarMetodo(DataFrame dataFrame)
        {
            DataFrame? dataFrameOrigem;

            while (_etapasExecutor.Count > 0)
            {
                var etapas = _etapasExecutor.Dequeue();

                dataFrameOrigem = (DataFrame)_objetosUtilizados[etapas.DataFrameOrigem]!;

                var dataFrameDestino = etapas.Executor!.Executar(dataFrameOrigem!);

                if(dataFrameDestino is DataFrame df)
                {
                    df.NomeContexto = etapas.DataFrameDestino;
                }

                _objetosUtilizados[etapas.DataFrameDestino] = dataFrameDestino;
                etapas = null;
            }

            var dataFrameRetorno = (DataFrame)_objetosUtilizados[_dataFrameRetorno]!;

            _listaComandos.Clear();

            return dataFrameRetorno!;
        }

        public void ConstroiMetodo(MetodoPipeline metodoPipeline)
        {
            foreach (var comando in metodoPipeline.Comandos)
            {
                if (comando is AtribuicaoMetodoPipeline atribuicao)
                {
                    ConstroiAtribuicao(atribuicao);
                    _listaComandos.Add(atribuicao);
                }
                else if(comando is RetornoMetodoPipeline retorno)
                {
                    if (!_objetosUtilizados.ContainsKey(retorno.Variavel))
                    {
                        throw new Exception($"Erro de Sintaxe: A variável '{retorno.Variavel}' não foi declarada.");
                    }

                    _dataFrameRetorno = retorno.Variavel;
                }
            }
        }

        public void ConstroiAtribuicao(AtribuicaoMetodoPipeline atribuicao)
        {
            if (!_objetosUtilizados.ContainsKey(atribuicao.ChamadaMetodo.ObjetoInicial))
            {
                throw new Exception($"Erro de Sintaxe: A variável '{atribuicao.ChamadaMetodo.ObjetoInicial}' não foi declarada.");
            }

            if (!_objetosUtilizados.ContainsKey(atribuicao.Variavel))
            {
                _objetosUtilizados[atribuicao.Variavel] = null;
            }

            var featureExecutor = new FeatureExecutor();

            foreach(var metodo in atribuicao.ChamadaMetodo.Metodos)
            {
                featureExecutor.PassaDicionarioObjetos(_objetosUtilizados);
                featureExecutor.CriarExecutorDinamico(metodo);
            }

            var expressaoExecutor = new EtapaExecucao(atribuicao.Variavel, atribuicao.ChamadaMetodo.ObjetoInicial, featureExecutor);

            _etapasExecutor.Enqueue(expressaoExecutor);
        }
    }
}
