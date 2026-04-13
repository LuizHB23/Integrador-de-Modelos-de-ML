using IntegradorAplicacao.ArquivosController.Csv.ExportarCsv;
using IntegradorAplicacao.ArquivosController.Csv.ExportarCsv.Factory;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.ArquivosController.Csv
{
    public class CsvController
    {
        private string[] _cabecalho;
        private List<string>[] _colunas;

        public string[] Cabecalho => _cabecalho;
        public List<string>[] Colunas => _colunas;

        public async Task CarregarArquivoAsync(string caminho)
        {
            var enumerador = LerArquivo(caminho).GetEnumerator();

            if (!enumerador.MoveNext())
                throw new Exception("CSV vazio");

            _cabecalho = enumerador.Current;
            _colunas = _cabecalho.Select(_ => new List<string>()).ToArray();

            while (enumerador.MoveNext())
            {
                var linha = enumerador.Current;

                for (int i = 0; i < _colunas.Length; i++)
                {
                    if (i < linha.Length)
                        _colunas[i].Add(linha[i]);
                    else
                        _colunas[i].Add(string.Empty);
                }
            }
        }

        public void EscreveArquivo<T>(T dados)
        {
            var exportador = CsvExportadorFactory.Criar<T>();
            exportador.ExportarCsv(dados);
        }

        private IEnumerable<string[]> LerArquivo(string caminho)
        {
            using var reader = new StreamReader(caminho);

            var campo = new StringBuilder();
            var linha = new List<string>();
            bool dentroAspas = false;

            while (!reader.EndOfStream)
            {
                int ci = reader.Read();
                char c = (char)ci;

                if (c == '"')
                {
                    if (dentroAspas && reader.Peek() == '"')
                    {
                        // aspas escapada ("")
                        campo.Append('"');
                        reader.Read();
                    }
                    else
                    {
                        dentroAspas = !dentroAspas;
                    }
                }
                else if (c == ',' && !dentroAspas)
                {
                    linha.Add(campo.ToString());
                    campo.Clear();
                }
                else if ((c == '\n' || c == '\r') && !dentroAspas)
                {
                    if (c == '\r' && reader.Peek() == '\n')
                        reader.Read();

                    linha.Add(campo.ToString());
                    campo.Clear();

                    yield return linha.ToArray();
                    linha.Clear();
                }
                else
                {
                    campo.Append(c);
                }
            }

            // última linha
            if (campo.Length > 0 || linha.Count > 0)
            {
                linha.Add(campo.ToString());
                yield return linha.ToArray();
            }
        }

    }
}
