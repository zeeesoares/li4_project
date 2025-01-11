namespace BitOk.Data.Models
{
    public class DesktopEncomendaModel
    {
        public int EncomendaId { get; set; }
        public int DesktopId { get; set; }
        public int QuantidadeProd { get; set; }
        public string Estado { get; set; }

        public EncomendaModel Encomenda { get; set; }
        public DesktopModel Desktop { get; set; }

        public string GetCurrentPieceName()
        {
            return Estado switch
            {
                "Espera" => "Nenhuma Peça",
                "Montar Caixa" => "Caixa",
                "Montar Motherboard" => "Motherboard",
                "Montar Componentes" => "Componentes",
                "Cable Management" => "Cabos",
                "Pronto" => "Pronto",
                _ => "Desconhecido"
            };
        }

        public string GetStatusColor()
        {
            return Estado switch
            {
                "Espera" => "yellow-500",
                "Montar Caixa" => "yellow-500",
                "Montar Motherboard" => "yellow-500",
                "Montar Componentes" => "yellow-500",
                "Cable Management" => "yellow-500",
                "Pronto" => "green-500",
                _ => "red-500"
            };
        }
    }
}
