using ISERTEC_OC.Models;

namespace ISERTEC_OC.Repositorios
{
    public interface IRepositorioOrden
    {
        bool Crear(CrearOrden orden);
        List<OrdenDeCompra> ListaDeOrdenes();
        CrearOrden Obtener(int Id);
    }

}
