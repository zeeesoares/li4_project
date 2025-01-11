namespace BitOk.Data.Models
{
    public class EncomendaModel
    {
        public int Id { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int EstadoId { get; set; }
        public int UtilizadorId { get; set; }
        public Estado Estado { get; set; } 
        public string GetStatusColor()
        {
            return Estado?.Nome switch
            {
                "Completa" => "green",
                "Preparação" => "yellow",
                "Espera" => "red",
                _ => "blue"
            };
        }
    }

    public class Estado
    {
        public int Id { get; set; }
        public string Nome { get; set; }

    }
}
