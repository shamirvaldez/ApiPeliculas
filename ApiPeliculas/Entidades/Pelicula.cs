using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Entidades
{
    public class Pelicula : IID
    {
        public int Id {get; set;}
        [Required]
        [StringLength(300)]
        public string Titulo { get; set; }
        public bool EnCines { get; set;  }
        public DateTime FechaEstreno { get; set; }
        public string Poster { get; set; }
        public List<PeliculasActores> peliculasActores { get; set; }

        public List<PeliculaGeneros> peliculaGeneros { get; set; }
        public List<PeliculasSalasDeCine> PeliculasSalasDeCine { get; set; }
    }
}
