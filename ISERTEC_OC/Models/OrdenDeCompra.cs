namespace ISERTEC_OC.Models
{
    public class OrdenDeCompra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo_Pago { get; set; }
        public DateTime Fecha_Pago { get; set; }
        public decimal Costo_De_La_Orden { get; set; }
        public string Terminos_De_Entrega { get; set; }
        public int Id_Usuario { get; set; }
        public int Id_Proveedor { get; set; }
        public string Proveedor { get; set; }
    }
}
