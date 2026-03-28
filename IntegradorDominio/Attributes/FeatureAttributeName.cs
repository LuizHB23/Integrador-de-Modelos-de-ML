namespace IntegradorDominio.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FeatureNameAttribute : Attribute
    {
        public string Nome { get; }
        public FeatureNameAttribute(string nome) => Nome = nome;
    }
}
