using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Entidades
{
    public class PeliculasSalasDeCine
    {
        public int PeliculaId { get; set; }
        public int SalaDeCineId { get; set; }
        public Pelicula pelicula { get; set; }
        public SalasDeCine SalaDeCine { get; set;  }
    }
}
