using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorDominio.DataFrameModel
{
    public class DataFrame
    {
        private readonly List<ColunaBase> _colunas = new();
        private readonly Dictionary<string, int> _colunaIndex = new();

        public string NomeContexto { get; set; }
        public int QuantidadeLinhas { get => Colunas[0].Quantidade; }
        public List<ColunaBase> Colunas {  get => _colunas; }
        public Dictionary<string, int> ColunaIndex { get => _colunaIndex; }

        public void AdicionarColuna<T>(string nome, List<T?> dados)
        {
            if (_colunas.Count > 0 && dados.Count != QuantidadeLinhas)
                throw new Exception("Column size mismatch");

            var col = new Coluna<T?>(nome, dados);
            _colunaIndex[nome] = _colunas.Count;
            _colunas.Add(col);
        }

        public void RenomearColunas(string antigoNome, string novoNome)
        {
            if (!_colunaIndex.TryGetValue(antigoNome, out int index))
            {
                throw new Exception($"Coluna '{antigoNome}' não encontrada.");
            }

            // Atualiza o nome da coluna
            _colunas[index].GetType()
                .GetProperty("Nome")?
                .SetValue(_colunas[index], novoNome);

            // Atualiza o índice no dicionário
            _colunaIndex.Remove(antigoNome);
            _colunaIndex[novoNome] = index;
        }

        public Coluna<T?> PegarColuna<T>(string nome)
        {
            if (!_colunaIndex.TryGetValue(nome, out int index))
            {
                return null;
            }

            var coluna = _colunas[index];

            if (coluna is Coluna<T?> colunaTipada)
            {
                return colunaTipada;
            }

            return null;
        }

        public ColunaBase? PegarColunaBase(string nome)
        {
            if (!_colunaIndex.TryGetValue(nome, out int index))
                return null;

            return _colunas[index];
        }

        public void AlterarColuna<T>(string nome, List<T?> valor)
        {
            if (!_colunaIndex.TryGetValue(nome, out int index))
            {
                AdicionarColuna<T?>(nome, valor);
            }
            else
            { 
                _colunas[index] = new Coluna<T?>(nome, valor);
            }
        }

        public void AlterarColuna<T>(string nome, List<T?> valor, Type tipo)
        {
            var tipoLista = typeof(List<>).MakeGenericType(tipo);
            var listaConvertida = (System.Collections.IList)Activator.CreateInstance(tipoLista)!;

            foreach (var v in valor)
            {
                if (v == null)
                {
                    listaConvertida.Add(null);
                }
                else
                {
                    var tipoBase = Nullable.GetUnderlyingType(tipo) ?? tipo;
                    var convertido = Convert.ChangeType(v, tipoBase);
                    listaConvertida.Add(convertido);
                }
            }

            var tipoColuna = typeof(Coluna<>).MakeGenericType(tipo);
            var novaColuna = Activator.CreateInstance(tipoColuna, nome, listaConvertida);

            if (!_colunaIndex.TryGetValue(nome, out int index))
            {
                _colunas.Add((ColunaBase)novaColuna!);
                _colunaIndex[nome] = _colunas.Count - 1;
            }
            else
            {
                _colunas[index] = (ColunaBase)novaColuna!;
            }
        }
    }
}
