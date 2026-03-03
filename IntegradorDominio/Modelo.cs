namespace IntegradorDominio
{
    public class Modelo
    {
        private string nome;
        private string tipo;
        private string caminhoPasta;

        public Modelo(string nome, string tipo, string caminhoPasta)
        {
            this.nome = nome;
            this.tipo = tipo;
            this.caminhoPasta = caminhoPasta;
        }
    }
}
