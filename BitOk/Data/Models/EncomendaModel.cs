namespace BitOk.Data.Models
{
    public class EncomendaModel
    {
        public int idEncomenda { get; set; } 
        public DateTime Data_Inicio { get; set; }
        public DateTime? Data_Fim { get; set; }
        public int Estado_idEstado { get; set; } 
        public int Utilizador_idUtilizador { get; set; }
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
