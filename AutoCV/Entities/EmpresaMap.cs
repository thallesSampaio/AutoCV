using CsvHelper.Configuration;

namespace AutoCV.Entities
{
    public class EmpresaMap : ClassMap<Empresa>
    {
        public EmpresaMap()
        {
            Map(e => e.CnpjRaiz).Index(0);
            Map(e => e.CnpjFilial).Index(1);
            Map(e => e.CnpjDv).Index(2);
            Map(e => e.Nome).Index(4);        // ou .Name("Nome")
            Map(e => e.CnaePrincipal).Index(11);
            Map(e => e.CnaesSecundarios).Index(12);
            Map(e => e.Uf).Index(19);
            Map(e => e.Municipio).Index(20);
            Map(e => e.Telefone).Index(22);
            Map(e => e.Email).Index(27);
            // Campos complexos como CnaePrincipal e CnaesSecundarios geralmente são tratados à parte
        }
    }
}
