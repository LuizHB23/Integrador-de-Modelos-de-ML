using CommunityToolkit.Mvvm.ComponentModel;
using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorEnum;
using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson;
using IntegradorDominio.Models.Configuracao;
using IntegradorViewModel.JanelaModelo;
using IntegradorViewModel.Shared.Context;
using IntegradorViewModel.Shared.Interfaces;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

namespace IntegradorViewModel.Pages.ConfiguracaoModelo
{
    public partial class ConfiguracaoModeloViewModel : ObservableObject
    {
        [ObservableProperty]
        private INavigationService _navigation;

        [ObservableProperty]
        private DataView? _schemaPreview;

        [ObservableProperty]
        private ModeloEmUsoConfiguracao _modelo;

        [ObservableProperty]
        public ObservableCollection<TransformadorDTO> _transformadores;

        [ObservableProperty]
        public Dictionary<string, string> _pipeline;

        private IContext <ModeloDTO> _context;
        private IPathProvider _provider;
        private IConversorJson _conversor;

        public ConfiguracaoModeloViewModel(INavigationService navigation, IPathProvider provider, IContext<ModeloDTO> context, IConversorJson conversor)
        {
            Navigation = navigation;

            _context = context;
            _provider = provider;
            _conversor = conversor;
        }

        public async Task InicializarAsync()
        {
            Modelo = await _conversor.CarregarJsonAsync<ModeloEmUsoConfiguracao>(_context.RecebeMensagem().NomeModelo);
            Modelo.CaminhoPasta = Path.GetFileName(Modelo.CaminhoPasta);

            var taskPipeline = CarregarPipeline();
            var taskSchema = CarregarSchema();
            var taskTransformadores = CarregarTransformadores();

            await Task.WhenAll(taskPipeline, taskSchema, taskTransformadores);

            Pipeline = await taskPipeline;
            SchemaPreview = await taskSchema;
            Transformadores = await taskTransformadores;
        }

        private async Task<DataView?> CarregarSchema()
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Coluna", typeof(string));
            dataTable.Columns.Add("Finalidade", typeof(string));
            dataTable.Columns.Add("Tipo", typeof(string));
            dataTable.Columns.Add("Categorico", typeof(bool));

            string nomeModelo = Modelo.NomeModelo;
            string caminhoSchema = _provider.GetCaminhoSchemaConfig(nomeModelo);

            if (!File.Exists(caminhoSchema))
            {
                return dataTable.DefaultView;
            }

            var schema = (await _conversor.CarregarJsonAsync<List<SchemaConfiguracao>>(nomeModelo)).First(s => s.Versao == Modelo.SchemaVersao);

            string nomeColuna;
            string finalidade;
            string tipo;
            bool categorico;

            foreach (var item in schema.Colunas)
            {
                nomeColuna = item.Value.NomeColuna;
                finalidade = item.Value.Finalidade;
                tipo = item.Value.Tipo;
                categorico = item.Value.Categorico;

                dataTable.Rows.Add(nomeColuna, finalidade, tipo, categorico);
            }

            return dataTable.DefaultView;
        }

        private async Task<Dictionary<string, string>> CarregarPipeline()
        {
            Dictionary<string, string> dicionarioPipeline = new();

            string nomeModelo = Modelo.NomeModelo;
            string caminhoPipeline = _provider.GetCaminhoPipelineConfig(nomeModelo);

            if (!File.Exists(caminhoPipeline))
            {
                return dicionarioPipeline;
            }

            var pipeline = (await _conversor.CarregarJsonAsync<List<PipelineTratamentoConfiguracao>>(nomeModelo)).First(p => p.Versao == Modelo.PipelineVersao);

            string codigo = string.Empty;

            foreach (var item in pipeline.ScriptCodigo)
            {
                codigo = $"{item.Value.NomeFuncao}()" + "\n{";

                foreach (var linha in item.Value.Codigo)
                {

                    codigo += $"\n{linha}\n";
                }
                codigo += "}";

                dicionarioPipeline.Add(item.Value.NomeFuncao, codigo);
            }

            return dicionarioPipeline;
        }

        private async Task<ObservableCollection<TransformadorDTO>> CarregarTransformadores()
        {
            ObservableCollection<TransformadorDTO> listaTransformadores = new();

            string nomeModelo = Modelo.NomeModelo;
            string caminhoTransformadores = _provider.GetCaminhoTransformadorConfig(nomeModelo);

            if (!File.Exists(caminhoTransformadores))
            {
                return listaTransformadores;
            }

            var transformadores = (await _conversor.CarregarJsonAsync<List<TransformadorConfiguracao>>(nomeModelo)).First(t => t.Versao == Modelo.TransformadoresVersao);

            foreach (var item in transformadores.Transformadores)
            {
                var nome = item.Value.NomeTransformador;
                var arquivo = !string.IsNullOrEmpty(item.Value.CaminhoTransformador)
                              ? Path.GetFileName(item.Value.CaminhoTransformador)
                              : string.Empty;

                Debug.WriteLine(item.Value.NomeTransformador);

                listaTransformadores.Add(new TransformadorDTO(nome, arquivo));
            }

            return listaTransformadores;
        }
    }
}
