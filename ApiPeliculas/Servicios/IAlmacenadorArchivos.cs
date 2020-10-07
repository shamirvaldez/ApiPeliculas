using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta,
            string contentType);
        Task BorraArchivo(string ruta, string contenedor);
        Task<string> GuardarAchivo(byte[] contenido, string extension, string contenedor, string contentType);
    }
}
