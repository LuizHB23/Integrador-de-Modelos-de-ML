namespace IntegradorDominio.FeatureEngineering.MapearLinhas.CasosLinha
{
    public class IfMap : NodeMap
    {
        public string Condicao { get; set; }
        public List<NodeMap> Corpo { get; set; }
        public List<NodeMap>? Else { get; set; }  // opcional

        public IfMap(string condicao, List<NodeMap> corpo, List<NodeMap>? elseCorpo = null)
        {
            Condicao = condicao;
            Corpo = corpo;
            Else = elseCorpo;
        }
    }
}