using IntegradorAplicacao.DTO;
using IntegradorAplicacao.PipelineAplicacao.ParserPipeline;
using IntegradorDominio.DataFrameModel;
using IntegradorDominio.Inferencia;
using IntegradorViewModel.Shared.Interfaces;
using System.Data;

namespace IntegradorViewModel.ControleUsuario
{
    public partial class ConfiguracaoTextBoxViewModel
    {
        private ParserAst _parserAst;

        private readonly Action<DataView> _onDadosAlterados;

        private readonly IDialogService _dialogService;

        public ConfiguracaoTextBoxViewModel(IDialogService dialogService, ArquivoDadosDTO arquivoDados, Action<DataView> onDadosAlterados)
        {
            _dialogService = dialogService;
            _onDadosAlterados = onDadosAlterados;

            _parserAst = new();
        }

        public Dictionary<string, List<string>>? MandaCodigoMetodo(string scriptCodigo)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(scriptCodigo))
                {
                    return _parserAst.ParserCorpo(scriptCodigo);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage($"Código do método está errado: {ex.Message}", "Código Errado");
            }

            return null;

        }

        public DataTable DataFrameParaDataTable(DataFrame dataFrame)
        {
            var tabela = new DataTable();
            var colunas = dataFrame.Colunas;
            var quantidadeColunas = colunas.Count;
            var quantidadeLinhas = dataFrame.QuantidadeLinhas;

            // Adicionar colunas ao DataTable
            for (int j = 0; j < quantidadeColunas; j++)
            {
                tabela.Columns.Add(colunas[j].Nome, typeof(object));
            }

            // Criar a primeira linha com TIPAGEM
            var linhaTipagem = tabela.NewRow();
            for (int j = 0; j < quantidadeColunas; j++)
            {
                Type tipo = colunas[j].TipoDado;

                // Mapear nullable para tipo “base” que você quer mostrar
                string tipoExibicao = tipo switch
                {
                    Type t when t == typeof(float) || t == typeof(float?) => "Single",
                    Type t when t == typeof(string) => "String",
                    Type t when t == typeof(bool) || t == typeof(bool?) => "Boolean",
                    Type t when t == typeof(DateTime) || t == typeof(DateTime?) => "DateTime",
                    _ => "Object"
                };

                linhaTipagem[j] = tipoExibicao;
            }
            tabela.Rows.Add(linhaTipagem);

            // Adicionar dados do DataFrame
            for (int i = 0; i < quantidadeLinhas; i++)
            {
                var linhaDados = tabela.NewRow();

                for (int j = 0; j < quantidadeColunas; j++)
                {
                    linhaDados[j] = colunas[j].PegarValor(i) ?? DBNull.Value;
                }

                tabela.Rows.Add(linhaDados);
            }

            return tabela;
        }

        public void AtualizaTabela(DataFrame dataFrame)
        {
            var dataTable = DataFrameParaDataTable(dataFrame);

            _onDadosAlterados(dataTable.DefaultView);
        }

        public void EscreveScript(string featureName, List<string> listaPropriedades, string scriptCodigo)
        {
            if(string.IsNullOrWhiteSpace(scriptCodigo))
            {
                scriptCodigo ="SuaFuncao()\n{\nreturn df\n}";
            }

            var indeReturn = scriptCodigo.IndexOf("return");

            var codigo = $"df = df.{featureName}()";
            var indexParenteses = codigo.IndexOf("()");

            if (featureName == "Map")
            {

                if (indexParenteses != -1)
                {
                    codigo = codigo.Insert(indexParenteses + 1, "lambdax=[for{loop:\"\", line:\"\"}, if:{condition:\"\", line:\"\", else:{line:\"\"}}, line:\"\"]");
                }
            }
            else
            {
                var propriedades = string.Empty;

                foreach (var propriedade in listaPropriedades)
                {
                    if(propriedade != "Contexto")
                    {
                        if(string.IsNullOrWhiteSpace(propriedades))
                        {
                            propriedades = $"{propriedade}=";
                        }
                        else
                        {
                            propriedades += $", {propriedade}=";
                        }
                    }
                }

                codigo = codigo.Insert(indexParenteses + 1, propriedades);
            }


            scriptCodigo = scriptCodigo.Insert(indeReturn, $"{codigo}\n\n");
        }

        public void EsvaziaScript(string scriptCodigo) => scriptCodigo = string.Empty;
    }
}