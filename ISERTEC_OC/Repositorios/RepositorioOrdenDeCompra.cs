using Azure;
using Dapper;
using ISERTEC_OC.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Specialized;
using System.Data;

namespace ISERTEC_OC.Repositorios
{
    public class RepositorioOrdenDeCompra : IRepositorioOrden
    {
        private readonly IConfiguration configuration;
        private string CadenaConexion;

        public RepositorioOrdenDeCompra(IConfiguration configuration)
        {
            this.configuration = configuration;
            CadenaConexion = configuration.GetConnectionString("ConnectionISERTEC_OC");
        }

        public List<OrdenDeCompra> ListaDeOrdenes()
        {
            using var conexion = new SqlConnection(CadenaConexion);
            var ordenes = conexion.Query<OrdenDeCompra>(@"EXECUTE SP_CRUD_ORDEN_DE_COMPRA @Id, @Tipo_pago, @Fecha, 
                                                        @Fecha_pago, @Costo_orden, @Terminos_de_entrega, 
                                                        @Id_Usuario, @Id_Proveedor, @Operacion",
                                                        new
                                                        {
                                                            Id = 0,
                                                            Tipo_pago = "",
                                                            Fecha = "",
                                                            Fecha_pago = "",
                                                            Costo_orden = 0,
                                                            Terminos_de_entrega = "",
                                                            Id_Usuario = 0,
                                                            Id_Proveedor = 0,
                                                            Operacion = "todo"
                                                        }
                                                        );
            return ordenes.ToList();
        }

        public bool Crear(CrearOrden orden)
        {
            using var conexion = new SqlConnection(CadenaConexion);
            conexion.Open();

            using var transaction = conexion.BeginTransaction(); // Iniciar la transacción

            try
            {
                var id = conexion.QueryFirstOrDefault<int>(@"EXECUTE SP_CRUD_ORDEN_DE_COMPRA @Id, @Tipo_pago, @Fecha, 
                                                        @Fecha_pago, @Costo_orden, @Terminos_de_entrega, 
                                                        @Id_Usuario, @Id_Proveedor, @Operacion",
                                                        new
                                                        {
                                                            Id = 0,
                                                            Tipo_pago = orden.Encabezado.Tipo_Pago,
                                                            Fecha = orden.Encabezado.Fecha,
                                                            Fecha_pago = orden.Encabezado.Fecha_Pago,
                                                            Costo_orden = 0,
                                                            Terminos_de_entrega = orden.Encabezado.Terminos_De_Entrega,
                                                            Id_Usuario = 1,
                                                            Id_Proveedor = orden.Encabezado.Id_Proveedor,
                                                            Operacion = "insert"
                                                        }, transaction
                                                        );
                if (id != 0)
                {
                    foreach (var detalle in orden.EncabezadoLista)
                    {
                        var parametersDetalle = new
                        {
                            Id = 0,
                            Cantidad = detalle.Cantidad,
                            Id_articulo = detalle.IdArticulo,
                            Id_orden = id,
                            Precio = detalle.Precio,
                            Operacion = "insert"
                        };

                        // Ejecutar el SP para crear el detalle.
                        conexion.Execute(
                            "SP_DETALLE_DE_ORDEN",
                            parametersDetalle,
                        commandType: CommandType.StoredProcedure,
                            transaction: transaction // Asociar la transacción con la ejecución del SP
                        );
                    }

                    transaction.Commit(); // Confirmar la transacción si todo ha ido bien
                }
                else
                {
                    transaction.Rollback(); // Deshacer la transacción si no se pudo crear la factura
                }

                return id != 0;
            }
            catch (Exception ex)
            {
                // En caso de error, deshacer la transacción
                transaction.Rollback();
                throw new Exception(ex.Message, ex); // Lanzar la excepción para que se maneje en la capa superior
            }
        }

        public CrearOrden Obtener(int Id)
        {
            try
            {
                using var conexion = new SqlConnection(CadenaConexion);
                var orden = conexion.QueryFirstOrDefault<OrdenDeCompra>(@"EXECUTE SP_CRUD_ORDEN_DE_COMPRA @Id, @Tipo_pago, @Fecha, 
                                                        @Fecha_pago, @Costo_orden, @Terminos_de_entrega, 
                                                        @Id_Usuario, @Id_Proveedor, @Operacion",
                                                            new
                                                            {
                                                                Id = Id,
                                                                Tipo_pago = "",
                                                                Fecha = "",
                                                                Fecha_pago = "",
                                                                Costo_orden = 0,
                                                                Terminos_de_entrega = "",
                                                                Id_Usuario = 1,
                                                                Id_Proveedor = 0,
                                                                Operacion = "select"
                                                            }
                                                            );
                var detalles = conexion.Query<DetalleOrdenDeCompra>(@"EXEC SP_DETALLE_DE_ORDEN
@Id,@Cantidad,  @Id_articulo    ,@Id_orden ,@Precio       ,@Operacion                                          
",
                                                new
                                                {
                                                    Id = 0,
                                                    Cantidad = 0,
                                                    Id_articulo = 0,
                                                    Id_orden = Id,
                                                    Precio = 0,
                                                    Operacion = "PorId",
                                                }
                                                );
                return new CrearOrden { Encabezado = orden, EncabezadoLista = detalles.ToList() };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
