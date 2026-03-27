using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.Interfaces
{
    public interface IExecutor
    {
        object Execute(object input);
    }
}
