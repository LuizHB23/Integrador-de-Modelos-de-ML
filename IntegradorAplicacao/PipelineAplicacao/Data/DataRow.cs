using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.Data
{
    public class DataRow
    {
        private readonly Dictionary<string, object> _values = new();

        public IReadOnlyDictionary<string, object> Values => _values;

        public T Get<T>(string coluna)
        {
            if (!_values.TryGetValue(coluna, out var value))
                throw new KeyNotFoundException($"Coluna {coluna} não encontrada");

            if (value is T tValue)
                return tValue;

            return (T)Convert.ChangeType(value, typeof(T));
        }

        public void Set(string coluna, object value)
        {
            _values[coluna] = value;
        }
    }
}
