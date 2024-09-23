namespace GenericToolsAPI.Models
{
    public class PontoInteresseRequest
    {
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string TipoPOI { get; set; } // Exemplo: "restaurante"
    }
}
