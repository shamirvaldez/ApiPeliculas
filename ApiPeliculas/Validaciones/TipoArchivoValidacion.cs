using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Validaciones
{
    public class TipoArchivoValidacion : ValidationAttribute
    {
        private readonly string[] tiposvalidos;
        public TipoArchivoValidacion(string[] TiposValidos)
        {
            this.tiposvalidos = TiposValidos;
        }

        public TipoArchivoValidacion( GrupoTipoArchivo grupoTipoArchivo)
        {
            if(grupoTipoArchivo == GrupoTipoArchivo.Imagen)
            {
                tiposvalidos = new string[] { "image/jpeg", "image/png", "image/gif", "image/jpg" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }


            IFormFile formFile = value as IFormFile;

          if(!tiposvalidos.Contains(formFile.ContentType))
            {
                return new ValidationResult($"el tipo de archivo debe ser uno de los siguiente: {string.Join(", ", tiposvalidos)}");
            }
            return ValidationResult.Success;
        }
    }
}
