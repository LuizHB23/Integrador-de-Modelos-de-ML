using IntegradorAplicacao.Infraestrutura.ArquivosController.Csv.ExportarCsv;
using IntegradorDominio.Models.DataFrameModel;
using IntegradorDominio.Models.Inferencia;

namespace IntegradorAplicacao.Infraestrutura.ArquivosController.Csv.ExportarCsv.Factory
{
    public class CsvExportadorFactory
    {
        public static ICsvExportador<T> Criar<T>()
        {
            if (typeof(T) == typeof(DataFrame))
                return (ICsvExportador<T>) new ExportarCsvResultadoInferencia();

            if (typeof(T) == typeof(List<ErrosInferencia>))
                return (ICsvExportador<T>) new ExportarCsvErros();

            throw new NotImplementedException($"Nenhum exportador para {typeof(T)}");
        }
    }
}
