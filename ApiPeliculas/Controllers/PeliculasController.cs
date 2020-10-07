using ApiPeliculas.DTOs;
using ApiPeliculas.Entidades;
using ApiPeliculas.Helpers;
using ApiPeliculas.Servicios;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext Context;
        private readonly IMapper Mapper;
        private readonly IAlmacenadorArchivos AlmacenadorArchivos;
        private readonly string contenedor = "Peliculas";
        public PeliculasController(ApplicationDbContext context, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            this.Context = context;
            this.Mapper = mapper;
            this.AlmacenadorArchivos = almacenadorArchivos;

        }

        [HttpGet]
        public async Task<ActionResult<PeliculasIndexDTO>> Get()
        {
            var Top = 5;
            var Hoy = DateTime.Today;

            var proximosEstrenos = await Context.Peliculas
                                  .Where(x => x.FechaEstreno > Hoy)
                                  .OrderBy(x => x.FechaEstreno)
                                  .Take(Top)
                                  .ToListAsync();

            var enCines = await Context.Peliculas
                          .Where(x => x.EnCines)
                          .Take(Top)
                          .ToListAsync();


            var resultado = new PeliculasIndexDTO();
            resultado.FuturosEstrenos = Mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
            resultado.EnCines = Mapper.Map<List<PeliculaDTO>>(enCines);
            return resultado;

            //  var peliculas = await Context.Peliculas.ToListAsync();

            // return Mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculasDTO filtroPeliculasDTO)
        {
            var PeliculaQueryable = Context.Peliculas.AsQueryable();

            if(!string.IsNullOrEmpty(filtroPeliculasDTO.Titulo))
            {
                PeliculaQueryable = PeliculaQueryable.Where(x => x.Titulo.Contains(filtroPeliculasDTO.Titulo));
            }

            if (filtroPeliculasDTO.EnCines)
            {
                PeliculaQueryable = PeliculaQueryable.Where(x => x.EnCines);
            }
            if(filtroPeliculasDTO.ProximosEstrenos)
            {
                var Hoy = DateTime.Today;
                PeliculaQueryable = PeliculaQueryable.Where(x => x.FechaEstreno > Hoy);
            }
            if(filtroPeliculasDTO.GeneroId != 0)
            {
                PeliculaQueryable = PeliculaQueryable.Where(x => x.peliculaGeneros.Select(y => y.GeneroId)
                                   .Contains(filtroPeliculasDTO.GeneroId));
            }

            await HttpContext.InsertarParametrosPaginacion(PeliculaQueryable, filtroPeliculasDTO.CantidadRegistrosPorPagina);


          var peliculas = await PeliculaQueryable.Paginar(filtroPeliculasDTO.Paginacion).ToListAsync();

            return Mapper.Map<List<PeliculaDTO>>(peliculas);
          }

        [HttpGet("{id}", Name = "ObtenerPelicula")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var peliculas = await Context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (peliculas == null)
            {
                return NotFound();
            }

            return Mapper.Map<PeliculaDTO>(peliculas);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] CreacionPeliculasDTO creacionPelicula)
        {

            
            var pelicula = Mapper.Map<Pelicula>(creacionPelicula);

         
         
            if (creacionPelicula.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await creacionPelicula.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(creacionPelicula.Poster.FileName);
                    pelicula.Poster = await AlmacenadorArchivos.GuardarAchivo(contenido, extension, contenedor,
                        creacionPelicula.Poster.ContentType);
                }
            }
            AsignarOrdenActores(pelicula);
            Context.Add(pelicula);
            await Context.SaveChangesAsync();
            return new CreatedAtRouteResult("ObtenerPelicula", new { id = pelicula.Id }, creacionPelicula);
       
        }

        private void AsignarOrdenActores(Pelicula pelicula)
        {
            for(int i=0; i < pelicula.peliculasActores.Count; i++)
            {
                pelicula.peliculasActores[i].Orden = i;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] CreacionPeliculasDTO creacionPeliculasDTO)
        {

    

            var peliculaDB = await Context.Peliculas.Include(x => x.peliculasActores)
                .Include(x => x.peliculaGeneros).FirstOrDefaultAsync(x => x.Id == id);
            
            if (peliculaDB == null) { return NotFound(); }

            peliculaDB = Mapper.Map(creacionPeliculasDTO, peliculaDB);

            if (creacionPeliculasDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await creacionPeliculasDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(creacionPeliculasDTO.Poster.FileName);
                    peliculaDB.Poster = await AlmacenadorArchivos.GuardarAchivo(contenido, extension, contenedor,
                        creacionPeliculasDTO.Poster.ContentType);
                }
            }

            AsignarOrdenActores(peliculaDB);
            Context.Entry(peliculaDB).State = EntityState.Modified;
            await Context.SaveChangesAsync();
            return new CreatedAtRouteResult("ObtenerPelicula", new { id = peliculaDB.Id }, creacionPeliculasDTO);
    
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entidad = await Context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }


            var entidadDTO = Mapper.Map<PeliculaPatchDTO>(entidad);

            patchDocument.ApplyTo(entidadDTO, ModelState);

            var esValido = TryValidateModel(entidadDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(entidadDTO, entidad);

            await Context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await Context.Peliculas.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            Context.Remove(new Pelicula() { Id = id });
            await Context.SaveChangesAsync();

            return NoContent();
        }
    }
}
