using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace IntegradorAplicacao.ConversorJson.CardsJson
{
    public class TransformadorJson : CardsJson<TransformadorDTO>
    {
        public TransformadorJson(IPathProvider provider) : base(provider) 
        {
            _json = "transformador.json";
        }
    }
}
