using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.ConversorJSON;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace IntegradorAplicacao.Infraestrutura.ConversorJSON.CardsJson
{
    public class TransformadorJson : CardsJson<TransformadorDTO>
    {
        public TransformadorJson(IPathProvider provider) : base(provider) 
        {
            _json = "transformador.json";
        }
    }
}
