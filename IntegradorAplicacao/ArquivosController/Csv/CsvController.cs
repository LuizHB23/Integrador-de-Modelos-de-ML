using IntegradorAplicacao.ArquivosController.Csv.ExportarCsv;
using IntegradorAplicacao.ArquivosController.Csv.ExportarCsv.Factory;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.ArquivosController.Csv
{
    public class CsvController
    {
        private Encoding _encoding;
        private char _pontuacaoDecimal;
        private char _delimitador;

        private string[]? _cabecalho;
        private List<string>[]? _colunas;

        public string[]? Cabecalho => _cabecalho;
        public List<string>[]? Colunas => _colunas;

        public CsvController()
        {
           _delimitador = ',';
           _pontuacaoDecimal = '.';
           _encoding = Encoding.UTF8;
        }

        public CsvController(char delimitador, char pontuacaoDecimal, Encoding encoding)
        {
            _encoding = encoding;
            _delimitador = delimitador;
            _pontuacaoDecimal = pontuacaoDecimal;
        }

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

            await Task.CompletedTask; // mantém assinatura async sem warning
        }

        public void EscreveArquivo<T>(T dados)
        {
            var exportador = CsvExportadorFactory.Criar<T>();
            exportador.ExportarCsv(dados);
        }

        private IEnumerable<string[]> LerArquivo(string caminho)
        {
            using var reader = new StreamReader(caminho, _encoding, true);

            var campo = new StringBuilder();
            var linha = new List<string>();

            bool dentroAspas = false;
            bool campoEntreAspas = false;

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
                        campoEntreAspas = true;
                    }
                }
                else if (c == _delimitador && !dentroAspas)
                {
                    linha.Add(ProcessarCampo(campo.ToString(), campoEntreAspas));
                    campo.Clear();
                    campoEntreAspas = false;
                }
                else if ((c == '\n' || c == '\r') && !dentroAspas)
                {
                    if (c == '\r' && reader.Peek() == '\n')
                        reader.Read();

                    linha.Add(ProcessarCampo(campo.ToString(), campoEntreAspas));
                    campo.Clear();

                    yield return linha.ToArray();
                    linha.Clear();

                    campoEntreAspas = false;
                }
                else
                {
                    campo.Append(c);
                }
            }

            // última linha
            if (campo.Length > 0 || linha.Count > 0)
            {
                linha.Add(ProcessarCampo(campo.ToString(), campoEntreAspas));
                yield return linha.ToArray();
            }
        }

        private string ProcessarCampo(string campo, bool estavaEntreAspas)
        {
            var valor = estavaEntreAspas ? campo : campo.Trim();

            // normalização de número
            if (!estavaEntreAspas && PodeSerNumero(valor))
            {
                if (_pontuacaoDecimal != '.')
                    valor = valor.Replace(_pontuacaoDecimal, '.');
            }

            return valor;
        }

        private bool PodeSerNumero(string valor)
        {
            // heurística simples e performática
            return valor.Any(char.IsDigit);
        }

    }

}
