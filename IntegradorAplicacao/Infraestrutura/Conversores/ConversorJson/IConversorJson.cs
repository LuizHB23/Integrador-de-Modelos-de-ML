using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson
{
    public interface IConversorJson
    {
        Task ConverteJsonAsync<T>(T objeto, string nomeModelo) where T : class;
        Task<T> CarregarJsonAsync<T>(string nomeModelo) where T : class;
    }
}
