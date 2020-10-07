using ApiPeliculas.Helpers;
using ApiPeliculas.Validaciones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.DTOs
{
    public class CreacionPeliculasDTO : PeliculaPatchDTO
    {
        [PesoImagenValidacion(4)]
        [TipoArchivoValidacion(GrupoTipoArchivo.Imagen)]
        public IFormFile Poster { get; set; }
        [ModelBinder(binderType: typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIds { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculasCreacionDTO>>))]
        public List<ActorPeliculasCreacionDTO> actores { get; set;}
    }
}
