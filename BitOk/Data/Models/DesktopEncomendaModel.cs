namespace BitOk.Data.Models
{
    public class DesktopEncomendaModel
    {
        public int Encomenda_idEncomenda { get; set; }
        public int Desktop_idDesktop { get; set; }
        public int Quantidade_Prod { get; set; }
        public string Estado { get; set; }
        public EncomendaModel? Encomenda { get; set; }
        public DesktopModel? Desktop { get; set; }

        public string GetCurrentPieceName()
        {
            return Estado switch
            {
                "Espera" => "Espera",
                "Montar CPU" => "CPU",
                "Montar RAM" => "RAM",
                "Montar Disco" => "Disco",
                "Montar Cooler" => "Cooler",
                "Montar Motherboard" => "Motherboard",
                "Montar GPU" => "GPU",
                "Montar Fonte de Alimentação" => "Fonte de Alimentação",
                "Montar Caixa" => "Caixa",
                "Pronto" => "Pronto",
                _ => "Desconhecido"
            };
        }


        public string GetStatusColor()
        {
            return Estado switch
            {
                "Espera" => "red",
                "Montar CPU" => "yellow",
                "Montar RAM" => "yellow",
                "Montar Disco" => "yellow",
                "Montar Cooler" => "yellow",
                "Montar Motherboard" => "yellow",
                "Montar GPU" => "yellow",
                "Montar Fonte de Alimentação" => "yellow",
                "Montar Caixa" => "yellow",
                "Pronto" => "green",
                _ => "red"
            };
        }

        public string GetGifName()
        {
            return Estado switch
            {
                "Espera" => "PCBuild Espera",
                "Montar CPU" => "PCBuild CPU",
                "Montar RAM" => "PCBuild RAM",
                "Montar Disco" => "PCBuild Disco",
                "Montar Cooler" => "PCBuild Cooler",
                "Montar Motherboard" => "PCBuild MotherBoard",
                "Montar GPU" => "PCBuild GPU",
                "Montar Fonte de Alimentação" => "PCBuild Fonte Alimentacao",
                "Montar Caixa" => "PCBuild Caixa",
                "Pronto" => "PCBuild Pronto",
                _ => "Desconhecido"
            };
        }

    }
}
