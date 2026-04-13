using IntegradorDominio.Inferencia;

namespace IntegradorAplicacao.ArquivosController.Csv.ExportarCsv.Factory
{
    public class CsvExportadorFactory
    {
        public static ICsvExportador<T> Criar<T>()
        {
            if (typeof(T) == typeof(List<ResultadoInferencia>))
                return (ICsvExportador<T>)new ExportarCsvResultadoInferência();

            throw new NotImplementedException($"Nenhum exportador para {typeof(T)}");
        }
    }
}
