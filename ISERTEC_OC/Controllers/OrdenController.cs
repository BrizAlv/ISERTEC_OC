using ISERTEC_OC.Models;
using ISERTEC_OC.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ISERTEC_OC.Controllers
{
    [Authorize]
    public class OrdenController : Controller
    {
        private readonly IRepositorioOrden repositorioOrden;
        private readonly IRepositorioProveedores repositorioProveedores;
        private readonly IRepositorioArticulo repositorioArticulo;

        public OrdenController(IRepositorioOrden repositorioOrden,
            IRepositorioProveedores repositorioProveedores, IRepositorioArticulo repositorioArticulo)
        {
            this.repositorioOrden = repositorioOrden;
            this.repositorioProveedores = repositorioProveedores;
            this.repositorioArticulo = repositorioArticulo;
        }
        // GET: OrdenController
        public ActionResult Index()
        {
            var ordenes = repositorioOrden.ListaDeOrdenes();
            return View(ordenes);
        }

        // GET: OrdenController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OrdenController/Create
        [HttpGet]
        public ActionResult Create()
        {
            var Proveedores = repositorioProveedores.LlamarProveedores();
            var Articulos = repositorioArticulo.TraerArticulos();
            ViewBag.Proveedores = new SelectList(Proveedores, "Id", "Nombre");
            ViewBag.Articulos = new SelectList(Articulos, "Id", "Descripcion");
            var modelo = new CrearOrden();
            modelo.Encabezado = new OrdenDeCompra();
            modelo.EncabezadoLista = new List<DetalleOrdenDeCompra>();
            return View(modelo);
        }

        // POST: OrdenController/Create
        [HttpPost]
        public ActionResult Create([FromBody] CrearOrden orden)
        {
            try
            {
                var operacion = repositorioOrden.Crear(orden);
                if (!operacion)
                {
                    return RedirectToAction("Error", "Home");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");

            }
        }

        // GET: OrdenController/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OrdenController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OrdenController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OrdenController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Imprimir(int id)
        {
            try
            {
                var orden = repositorioOrden.Obtener(id);
                if (orden == null || orden.Encabezado == null || orden.EncabezadoLista == null)
                {
                    return RedirectToAction("Error", "Home");
                }

                return View("Imprimir", orden);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
