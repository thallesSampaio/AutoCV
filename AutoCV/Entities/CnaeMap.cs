using CsvHelper.Configuration;

namespace AutoCV.Entities
{
    public class CnaeMap : ClassMap<Cnae>
    {
        public CnaeMap() 
        {
            Map(c => c.Codigo).Index(0);
            Map(c => c.Descricao).Index(1);
        }
    }
}
