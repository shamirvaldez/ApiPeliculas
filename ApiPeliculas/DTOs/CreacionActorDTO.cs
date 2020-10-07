using ApiPeliculas.Validaciones;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.DTOs
{
    public class CreacionActorDTO : ActorPatchDTO
    {
      
    
        [PesoImagenValidacion(pesoMaximo: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Foto { get; set; }
    }
}
