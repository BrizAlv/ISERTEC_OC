namespace ISERTEC_OC.Models
{
    public class DetalleOrdenDeCompra
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public int IdArticulo { get; set; }
        public int Id_Orden { get; set; }
        public decimal Precio { get; set; }
        public string NombreArticulo { get; set; }
    }
}
