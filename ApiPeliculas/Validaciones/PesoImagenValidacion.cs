using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace ApiPeliculas.Validaciones
{
    public class PesoImagenValidacion : ValidationAttribute
    {
        private readonly int PesoMaximo;
        public PesoImagenValidacion(int pesoMaximo)
        {
            PesoMaximo = pesoMaximo;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value ==null)
            {
                return ValidationResult.Success;
            }


            IFormFile formFile = value as IFormFile;

            if(formFile.Length > PesoMaximo * 1024 * 1024)
            {
                return new ValidationResult($"El peso del archivo no debe ser mayor {PesoMaximo}mb");
            }

            return ValidationResult.Success;
        }
    }
}
