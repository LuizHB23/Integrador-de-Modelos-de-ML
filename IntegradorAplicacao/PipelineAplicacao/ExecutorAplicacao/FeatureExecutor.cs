using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Executors;
using IntegradorDominio.AST;
using IntegradorDominio.Attributes;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.InterfacesSteps;
using System.Diagnostics;
using System.Reflection;


namespace IntegradorAplicacao.PipelineAplicacao.ExecutorAplicacao
{
    public class FeatureExecutor
    {
        private readonly Queue<IExecutorBase> _executores = new();
        Dictionary<string, object?>? _objetosUtilizados;

        private static readonly Lazy<Dictionary<string, Type>> _cacheExecutores =
            new(() =>
            {
                var dict = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

                var assembly = typeof(IExecutorBase).Assembly;

                var tipos = assembly.GetTypes()
                    .Where(t => typeof(IExecutorBase).IsAssignableFrom(t) && !t.IsAbstract);

                foreach (var tipo in tipos)
                {
                    var baseType = tipo.BaseType;

                    if (baseType == null || !baseType.IsGenericType)
                        continue;

                    var tipoFeature = baseType.GetGenericArguments()[0];

                    var attrs = tipoFeature.GetCustomAttributes<FeatureNameAttribute>();

                    foreach (var attr in attrs)
                    {
                        dict[attr.Nome] = tipo;
                    }
                }

                return dict;
            });

        public void AdicionarExecutor(IExecutorBase executor)
        {
            _executores.Enqueue(executor);
        }

        public object Executar(DataFrame dataFrame)
        {
            object? objetoNovo = dataFrame;
            var dataFrameAuxiliar = dataFrame;

            while (_executores.Count > 0)
            {
                var executorObjeto = _executores.Dequeue();

                if (objetoNovo is DataFrame)
                {
                    dataFrameAuxiliar = (DataFrame)objetoNovo;
                }
                else
                {
                    throw new Exception($"Você precisa de um DataFrame para esta operacao, pois a operação retornou um {objetoNovo}");
                }

                if (executorObjeto is IExecutorBase executor)
                {
                    if (executor == null)
                    {
                        throw new Exception("dataFrameExecutor está null");
                    }

                    if (dataFrameAuxiliar == null)
                    {
                        throw new Exception("dataFrameAuxiliar está null");
                    }

                    objetoNovo = executor.Executar(dataFrameAuxiliar);
                }

                executorObjeto = null;
            }

            return objetoNovo!;
        }

        public void CriarExecutorDinamico(MetodoChainPipeline metodoChain)
        {
            var executorType = EncontrarClassePorNome(metodoChain.Nome);

            if (executorType == null)
                throw new Exception($"Executor '{metodoChain.Nome}' não encontrado.");

            // Descobrir tipo da operação (T)
            var baseType = executorType.BaseType!;
            var tipoOperacao = baseType.GetGenericArguments()[0];

            // Criar instância da operação
            var operacao = Activator.CreateInstance(tipoOperacao);

            // Preencher propriedades
            foreach (var argumento in metodoChain.Argumentos)
            {
                if (argumento is null)
                {
                    continue;
                }

                var propriedade = tipoOperacao.GetProperty(argumento.Nome!);

                if (propriedade is null)
                {
                    continue;
                }

                var valorConvertido = Convert.ChangeType(argumento.Valor, propriedade.PropertyType);

                propriedade.SetValue(operacao, valorConvertido);
            }

            var contexto = tipoOperacao.GetProperty("Contexto");

            contexto!.SetValue(operacao, _objetosUtilizados);

            // Criar executor passando a operação
            var executor = (IExecutorBase)Activator.CreateInstance(executorType, operacao)!;

            _executores.Enqueue(executor);
        }

        private Type EncontrarClassePorNome(string nomeFuncao)
        {

            if (_cacheExecutores.Value.TryGetValue(nomeFuncao, out var tipo))
                return tipo;

            throw new Exception($"Executor para '{nomeFuncao}' não encontrado.");
        }

        public void PassaDicionarioObjetos(Dictionary<string, object?> objetosUtilizados) => _objetosUtilizados = objetosUtilizados;
    }
}