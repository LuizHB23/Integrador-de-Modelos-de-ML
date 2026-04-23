using IntegradorAplicacao.CaminhoProvider;
using IntegradorAplicacao.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace IntegradorAplicacao.ConversorJson.CardsJson
{
    public class PipelineJson : CardsJson<FuncaoDTO>
    {
        public PipelineJson(IPathProvider provider) : base(provider)
        {
            _json = "pipeline.json";
        }
    }
}
