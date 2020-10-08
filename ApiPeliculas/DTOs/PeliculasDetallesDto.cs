using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.DTOs
{
    public class PeliculasDetallesDto : PeliculaDTO
    {
        public List<GenerosDTO> Generos { get; set;}
        public List<ActoPeliculaDetalleDTO> Actores { get; set; }
    }
}
