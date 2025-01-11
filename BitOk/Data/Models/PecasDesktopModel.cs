namespace BitOk.Data.Models
{
    public class PecasDesktopModel
    {
        public int PecaId { get; set; }
        public int DesktopId { get; set; }
        public int Quantidade { get; set; }
        public PecaModel Peca { get; set; }
        public DesktopModel Desktop { get; set; }
    }
}
