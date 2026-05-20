using IntegradorAplicacao.DTO;
using IntegradorAplicacao.Infraestrutura.CaminhoProvider;
using IntegradorAplicacao.Infraestrutura.ConversorJSON;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace IntegradorAplicacao.Infraestrutura.ConversorJSON.CardsJson
{
    public class PipelineJson : CardsJson<FuncaoDTO>
    {
        public PipelineJson(IPathProvider provider) : base(provider)
        {
            _json = "pipeline.json";
        }
    }
}
