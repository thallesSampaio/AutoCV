namespace AutoCV.Entities
{
    public class Empresa
    {

        public string CnpjRaiz { get; set; }
        public string CnpjFilial { get; set; }
        public string CnpjDv { get; set; }

        public string Cnpj => $"{CnpjRaiz}{CnpjFilial}{CnpjDv}";
        public string Nome { get; set; }

        // Relação com CNAE principal (1-para-1)
        public string CnaePrincipal { get; set; }

        // Relação com CNAEs secundários (1-para-muitos)
        public string CnaesSecundarios { get; set; }

        public string Uf { get; set; }
        public string Municipio { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }

        public bool StatusEnvio { get; set; }
        // public DateTime? DataEnvioEmail { get; set; } // Pode ser nulo caso ainda não tenha enviado
    }
}