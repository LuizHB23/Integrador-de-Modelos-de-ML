using IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson.ConverteJson;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.Infraestrutura.Conversores.ConversorJson
{
    public interface IConversorJson
    {
        Task<T> CarregarJsonAsync<T>(string caminho) where T : class;
        Task ConverteJsonAsync<T>(T objeto) where T : class;
    }
}
