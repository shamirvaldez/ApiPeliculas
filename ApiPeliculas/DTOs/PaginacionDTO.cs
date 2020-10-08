using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;

        private int cantidadRegistroPorPagina = 10;
        private readonly int cantidadMaximaRegistroPorPagina = 50;

        public int CantidadRegistroPorPagina
        {
            get => cantidadRegistroPorPagina;
            set
            {
                cantidadRegistroPorPagina = (value > cantidadMaximaRegistroPorPagina) ? cantidadMaximaRegistroPorPagina : value;
            }
        }
    }
}
