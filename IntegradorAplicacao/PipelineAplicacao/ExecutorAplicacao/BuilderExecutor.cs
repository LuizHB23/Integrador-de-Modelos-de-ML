using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorDominio.AST;
using IntegradorDominio.DataFrameModel;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao
{
    public class BuilderExecutor
    {
        private Dictionary<string, object?> _objetosUtilizados;
        private List<ComandoMetodoPipeline> _listaComandos;
        private List<EtapaExecucao> _etapasExecutor;
        private string _dataFrameRetorno;

        public BuilderExecutor()
        {
            _etapasExecutor = new();
            _objetosUtilizados = new();
            _listaComandos = new();
            _dataFrameRetorno = string.Empty;
        }

        public DataFrame ExecutarMetodo(DataFrame dataFrame)
        {
            _objetosUtilizados["df"] = dataFrame;

            DataFrame? dataFrameOrigem;

            foreach (var etapas in _etapasExecutor)
            {
                dataFrameOrigem = (DataFrame)_objetosUtilizados[etapas.DataFrameOrigem]!;

                _objetosUtilizados[etapas.DataFrameDestino] = etapas.Executor!.Executar(dataFrameOrigem!);
            }

            var dataFrameRetorno = (DataFrame)_objetosUtilizados[_dataFrameRetorno]!;

            _objetosUtilizados.Clear();
            _etapasExecutor.Clear();
            _listaComandos.Clear();

            return dataFrameRetorno!;
        }

        public void ConstroiMetodo(MetodoPipeline metodoPipeline)
        {
            _objetosUtilizados["df"] = null;

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

            _etapasExecutor.Add(expressaoExecutor);
        }
    }
}
