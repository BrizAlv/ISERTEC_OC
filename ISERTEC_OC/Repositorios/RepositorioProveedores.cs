using Azure;
using Dapper;
using ISERTEC_OC.Models;
using Microsoft.Data.SqlClient;

namespace ISERTEC_OC.Repositorios
{
    public class RepositorioProveedores : IRepositorioProveedores
    {
        private readonly IConfiguration configuration;
        private string cadenaConexion;
        public RepositorioProveedores(IConfiguration configuration)
        {
            this.configuration = configuration;
            cadenaConexion = configuration.GetConnectionString("ConnectionISERTEC_OC");
        }
        public List<Proveedor> LlamarProveedores()
        {
            using var conexion = new SqlConnection(cadenaConexion);
            var Proveedores = conexion.Query<Proveedor>(@"EXEC SP_CRUD_PROVEEDOR  @Id, @Nombre, @NIT, @Direccion, @Email,
                                                        @Operacion",
                                                        new
                                                        {
                                                            Id = 0,
                                                            Nombre = "",
                                                            NIT = "",
                                                            Direccion = "",
                                                            Email = "",
                                                            Operacion = "todo",
                                                        }
                                                        );
            return Proveedores.ToList();
                            
        }
    }
}
