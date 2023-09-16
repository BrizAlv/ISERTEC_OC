using Azure;
using Dapper;
using ISERTEC_OC.Models;
using Microsoft.Data.SqlClient;

namespace ISERTEC_OC.Repositorios
{
    public class RepositorioArticulo : IRepositorioArticulo
    {
        private readonly IConfiguration configuration;
        private string conexion;

        public RepositorioArticulo(IConfiguration configuration)

        {
            this.configuration = configuration;
            conexion = configuration.GetConnectionString("ConnectionISERTEC_OC");
        }
        public List<Articulo> TraerArticulos()
        {
            using var conexionbd = new SqlConnection(conexion);
            var Articulos = conexionbd.Query<Articulo>("EXEC SP_ARTICULO @Id, @Descripcion, @Operacion ",
                                                        new
                                                        {
                                                            Id = 0,
                                                            Descripcion = "",
                                                            Operacion = "todo"
                                                        }
                                                        );
            return Articulos.ToList();
        }
    }
}
