using IntegradorAplicacao.DTO;

namespace IntegradorViewModel.Shared.Factory
{
    public interface IPipelineExecutorFactory<T>
    {
        static abstract T Criar(string nomeFuncao, List<string> codigo, string nomeModelo);
    }

    public class FuncaoDTOFactory : IPipelineExecutorFactory<FuncaoDTO>
    {
        public static FuncaoDTO Criar(string nomeFuncao, List<string> codigo, string nomeModelo)
            => new FuncaoDTO()
            {
                NomeFuncao = nomeFuncao,
                Codigo = codigo,
                NomeModelo = nomeModelo
            };
    }

    public class SaidaDTOFactory : IPipelineExecutorFactory<SaidaDTO>
    {
        public static SaidaDTO Criar(string nomeFuncao, List<string> codigo, string nomeModelo)
            => new SaidaDTO()
            {
                NomeFuncao = nomeFuncao,
                Codigo = codigo,
                NomeModelo = nomeModelo
            };
    }
}
