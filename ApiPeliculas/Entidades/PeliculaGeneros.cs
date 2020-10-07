using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Entidades
{
    public class PeliculaGeneros
    {
        public int GeneroId { get; set; }
        public int PeliculaId { get; set; }
        public Genero Genero { get; set; }
        public Pelicula pelicula { get; set; }

    }
}
