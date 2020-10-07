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
    [Route("api/actores")]
    [ApiController]
    public class ActoresController : ControllerBase
    {

        public readonly ApplicationDbContext Context;
        public readonly IMapper Mapper;
        public readonly IAlmacenadorArchivos AlmacenadorArchivos;
        public readonly string contenedor = "actores";

        public ActoresController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.Context = context;
            this.Mapper = mapper;
            this.AlmacenadorArchivos = almacenadorArchivos;
        }


        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = Context.Actores.AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistroPorPagina);
            var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();
           

            return Mapper.Map<List<ActorDTO>>(entidades);

        }

        [HttpGet("{id}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var entidad = await Context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            return Mapper.Map<ActorDTO>(entidad);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] CreacionActorDTO generarActorDTO)
        {

            var entidad = Mapper.Map<Actor>(generarActorDTO);
            if (generarActorDTO.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await generarActorDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(generarActorDTO.Foto.FileName);
                    entidad.Foto = await AlmacenadorArchivos.GuardarAchivo(contenido, extension, contenedor,
                        generarActorDTO.Foto.ContentType);
                }
            }


            Context.Add(entidad);
            await Context.SaveChangesAsync();

            return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, generarActorDTO);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] CreacionActorDTO generarActorDTO)
        {
            // var entidad = Mapper.Map<Actor>(generarActorDTO);

            //entidad.Id = id;
            //Context.Entry(entidad).State = EntityState.Modified;
            var actorDB = await Context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actorDB == null)
            {
                return NotFound();
            }

            actorDB = Mapper.Map(generarActorDTO, actorDB);
            if (generarActorDTO.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await generarActorDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(generarActorDTO.Foto.FileName);
                    actorDB.Foto = await AlmacenadorArchivos.EditarArchivo(contenido, extension, contenedor,
                    actorDB.Foto, generarActorDTO.Foto.ContentType);
                }
            }

            await Context.SaveChangesAsync();
            return NoContent();

        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch( int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            if(patchDocument == null)
            {
                return BadRequest();
            }

            var entidad = await Context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if(entidad == null)
            {
                return NotFound();
            }


            var entidadDTO = Mapper.Map<ActorPatchDTO>(entidad);

            patchDocument.ApplyTo(entidadDTO, ModelState);

            var esValido = TryValidateModel(entidadDTO);

            if(!esValido)
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
            var existe = await Context.Actores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            Context.Remove(new Actor() { Id = id });
            await Context.SaveChangesAsync();

            return NoContent();
        }
    }
}
