namespace ISERTEC_OC.Models
{
    public class CrearOrden
    {
        public OrdenDeCompra Encabezado { get; set; }
        public List<DetalleOrdenDeCompra> EncabezadoLista { get; set; }
    }
}
