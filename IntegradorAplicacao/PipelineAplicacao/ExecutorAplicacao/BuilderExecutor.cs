using IntegradorAplicacao.ConversorJson;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorDominio.AST;
using IntegradorDominio.DataFrameModel;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao
{
    public class BuilderExecutor
    {
        private readonly IConverteJson<Dictionary<int, FuncaoDTO>> _conversor;
        private readonly ParserAst _parser;

        private Dictionary<string, DataFrame> _dataFramesUtilizados;
        private List<ComandoMetodoPipeline> _listaComandos;
        private List<EtapaExecucao> _etapasExecutor;
        private string _dataFrameRetorno;

        public BuilderExecutor(IConverteJson<Dictionary<int, FuncaoDTO>> conversor)
        {
            _parser = new ParserAst();
            _conversor = conversor;

            _etapasExecutor = new();
            _dataFramesUtilizados = new();
            _listaComandos = new();
            _dataFrameRetorno = string.Empty;
        }

        public DataFrame ExecutarTudo(DataFrame dataFrame)
        {
            _dataFramesUtilizados["df"] = dataFrame;

            DataFrame? dataFrameOrigem = null;

            foreach (var etapas in _etapasExecutor)
            {
                dataFrameOrigem = _dataFramesUtilizados[etapas.DataFrameOrigem];

                _dataFramesUtilizados[etapas.DataFrameDestino] = etapas.Executor!.Executar(dataFrameOrigem!);
            }

            var dataFrameRetorno = _dataFramesUtilizados[_dataFrameRetorno!];

            _dataFramesUtilizados.Clear();
            _etapasExecutor.Clear();
            _listaComandos.Clear();

            return dataFrameRetorno;
        }

        public void ConstroiMetodo(DataFrame dataFrame, string caminhoFuncao)
        {
            _dataFramesUtilizados["df"] = dataFrame;

            var modeloPipeline = RecuperaMetodoPipeline(caminhoFuncao);

            foreach (var comando in modeloPipeline.Comandos)
            {
                if (comando is AtribuicaoMetodoPipeline atribuicao)
                {
                    ConstroiAtribuicao(atribuicao);
                    _listaComandos.Add(atribuicao);
                }
                else if(comando is RetornoMetodoPipeline retorno)
                {
                    if (!_dataFramesUtilizados.ContainsKey(retorno.Variavel))
                    {
                        throw new Exception($"Erro de Sintaxe: A variável '{retorno.Variavel}' não foi declarada.");
                    }

                    _dataFrameRetorno = retorno.Variavel;
                }
            }
        }

        public void ConstroiAtribuicao(AtribuicaoMetodoPipeline atribuicao)
        {
            if (!_dataFramesUtilizados.ContainsKey(atribuicao.ChamadaMetodo.ObjetoInicial))
            {
                throw new Exception($"Erro de Sintaxe: A variável '{atribuicao.ChamadaMetodo.ObjetoInicial}' não foi declarada.");
            }

            if (!_dataFramesUtilizados.ContainsKey(atribuicao.Variavel))
            {
                _dataFramesUtilizados[atribuicao.Variavel] = new DataFrame();
            }

            var featureExecutor = new FeatureExecutor();

            foreach(var metodo in atribuicao.ChamadaMetodo.Metodos)
            {
                featureExecutor.CriarExecutorDinamico(metodo);
            }

            var expressaoExecutor = new EtapaExecucao(atribuicao.Variavel, atribuicao.ChamadaMetodo.ObjetoInicial, featureExecutor);

            _etapasExecutor.Add(expressaoExecutor);
        }

        private MetodoPipeline RecuperaMetodoPipeline(string caminhoFuncao)
        {
            var codigosJson = _conversor.CarregarJson(caminhoFuncao);
            var metodoNomeCorpo = new Dictionary<string, List<string>>();

            foreach (var elemento in codigosJson)
            {
                metodoNomeCorpo.Add(elemento.Value.NomeFuncao, elemento.Value.Codigo);
            }

            return _parser.Parse(metodoNomeCorpo);
        }
    }
}
