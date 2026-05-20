using IntegradorAplicacao.PipelineAplicacao.ExecutorPipeline.FeatureExecutor.MapearLinhas;
using IntegradorDominio.FeatureEngineering.MapearLinhas;
using IntegradorDominio.FeatureEngineering.MapearLinhas.CasosLinha;
using IntegradorDominio.Models.DataFrameModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace IntegradorTesteUnidade.AplicacaoTestes.PipelineAplicacaoTestes.ExecutorPipelineTestes.FeatureExecutorTestes.MapearLinhas
{
    public class MapExecutorTests
    {
        private DataFrame CriarDataFrame()
        {
            var df = new DataFrame { NomeContexto = "df" };

            df.AdicionarColuna("GastoTotal", new List<float?> { 100f, 200f, 300f });
            df.AdicionarColuna("GastoMensal", new List<float?> { 10f, 20f, 30f });

            return df;
        }

        private Map CriarMap(string lambdax, DataFrame df)
        {
            return new Map
            {
                lambdax = lambdax,
                Contexto = new Dictionary<string, object>
                {
                    { df.NomeContexto, df }
                }
            };
        }

        private float? Val(DataFrame df, string col, int i)
        {
            return df.PegarColuna<float?>(col).Dados[i];
        }

        [Fact]
        public void Map_LineSimples_DeveAtualizarColuna()
        {
            var df = CriarDataFrame();
            var map = CriarMap(@"[line:""GastoTotal = GastoTotal + 10""]", df);

            new MapExecutor(map).Executar(df);

            Assert.Equal(110f, Val(df, "GastoTotal", 0));
            Assert.Equal(210f, Val(df, "GastoTotal", 1));
            Assert.Equal(310f, Val(df, "GastoTotal", 2));
        }

        [Fact]
        public void Map_VariavelTemporaria_DeveFuncionar()
        {
            var df = CriarDataFrame();
            var map = CriarMap(@"[line:""x = GastoTotal * 2"", line:""GastoMensal = x""]", df);

            new MapExecutor(map).Executar(df);

            Assert.Equal(200f, Val(df, "GastoMensal", 0));
            Assert.Equal(400f, Val(df, "GastoMensal", 1));
            Assert.Equal(600f, Val(df, "GastoMensal", 2));
        }

        [Fact]
        public void Map_If_DeveAplicarCondicao()
        {
            var df = CriarDataFrame();
            var map = CriarMap(@"[if:{condition:""GastoTotal > 150"", line:""GastoTotal = GastoTotal + 10""}]", df);

            new MapExecutor(map).Executar(df);

            Assert.Equal(100f, Val(df, "GastoTotal", 0));
            Assert.Equal(210f, Val(df, "GastoTotal", 1));
            Assert.Equal(310f, Val(df, "GastoTotal", 2));
        }

        [Fact]
        public void Map_IfElse_DeveAplicarAmbosCaminhos()
        {
            var df = CriarDataFrame();
            var map = CriarMap(@"[if:{condition:""GastoTotal > 150"", line:""GastoTotal = GastoTotal + 10"", else:{line:""GastoTotal = 0""}}]", df);

            new MapExecutor(map).Executar(df);

            Assert.Equal(0f, Val(df, "GastoTotal", 0));
            Assert.Equal(210f, Val(df, "GastoTotal", 1));
            Assert.Equal(310f, Val(df, "GastoTotal", 2));
        }

        [Fact]
        public void Map_For_DeveExecutarLoop()
        {
            var df = CriarDataFrame();
            var map = CriarMap(@"[for:{loop:""i = 0; i < 3; i = i+1"", line:""GastoTotal = GastoTotal + i""}]", df);

            new MapExecutor(map).Executar(df);

            // 0 + 1 + 2 = 3
            Assert.Equal(103f, Val(df, "GastoTotal", 0));
            Assert.Equal(203f, Val(df, "GastoTotal", 1));
            Assert.Equal(303f, Val(df, "GastoTotal", 2));
        }

        [Fact]
        public void Map_Misto_DeveExecutarTudo()
        {
            var df = CriarDataFrame();

            var map = CriarMap(@"[line:""x = GastoTotal * 2"", if:{condition:""x > 200"", line:""GastoMensal = x"", else:{line:""GastoMensal = 0""}}, for:{loop:""i = 0; i < 2; i = i+1"", line:""GastoTotal = GastoTotal + i""}]", df);

            new MapExecutor(map).Executar(df);

            Assert.Equal(0f, Val(df, "GastoMensal", 0));
            Assert.Equal(400f, Val(df, "GastoMensal", 1));
            Assert.Equal(600f, Val(df, "GastoMensal", 2));

            // i = 0 + 1 = +1
            Assert.Equal(101f, Val(df, "GastoTotal", 0));
            Assert.Equal(201f, Val(df, "GastoTotal", 1));
            Assert.Equal(301f, Val(df, "GastoTotal", 2));
        }

        [Fact]
        public void Map_ExpressaoComplexa_DeveFuncionar()
        {
            var df = CriarDataFrame();
            var map = CriarMap(@"[line:""GastoTotal = (GastoTotal + 10) * 2""]", df);

            new MapExecutor(map).Executar(df);

            Assert.Equal(220f, Val(df, "GastoTotal", 0));
            Assert.Equal(420f, Val(df, "GastoTotal", 1));
            Assert.Equal(620f, Val(df, "GastoTotal", 2));
        }
    }
}
