using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Servicios
{
    public class AlmacenadorAchivoLocal : IAlmacenadorArchivos
    {

        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AlmacenadorAchivoLocal(IWebHostEnvironment env, 
            IHttpContextAccessor httpContextAccessor)
        {
            this._env = env;
            this._httpContextAccessor = httpContextAccessor;
        }


        public Task BorraArchivo(string ruta, string contenedor)
        {
            if(ruta == null)
            {
                var nombreArchivo = Path.GetFileName(ruta);
                string directorioArchivo = Path.Combine(_env.WebRootPath, contenedor, nombreArchivo);

                if (File.Exists(directorioArchivo))
                {
                    File.Delete(directorioArchivo);
                }

               
            }

            return Task.FromResult(0);
        }

        public Task<string> EditarArchivo(byte[] contenido, string extension, string contenedor, string ruta, string contentType)
        {
            throw new NotImplementedException();
        }

        public  async Task<string> GuardarAchivo(byte[] contenido, string extension, string contenedor, string contentType)
        {
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(_env.WebRootPath, contenedor);


            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, nombreArchivo);
            await File.WriteAllBytesAsync(ruta, contenido);

            var UrlActual = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var urlParaDB = Path.Combine(UrlActual, contenedor, nombreArchivo).Replace("\\", "/");

            return urlParaDB;
        }
    }
}
