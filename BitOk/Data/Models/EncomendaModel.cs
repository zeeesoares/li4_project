namespace BitOk.Data.Models
{
    public class EncomendaModel
    {
        public int idEncomenda { get; set; } // Representa o Id da encomenda na base de dados
        public DateTime Data_Inicio { get; set; } // Representa a data de início da encomenda
        public DateTime? Data_Fim { get; set; } // Representa a data de fim da encomenda (pode ser null)
        public int Estado_idEstado { get; set; } // Representa o Id do estado da encomenda
        public int Utilizador_idUtilizador { get; set; } // Representa o Id do utilizador associado à encomenda
        public Estado Estado { get; set; } // Representa o estado da encomenda (relacionamento com a tabela Estado)

        public string GetStatusColor()
        {
            return Estado?.Nome switch
            {
                "Completa" => "green", // Cor verde para o status "Completa"
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
