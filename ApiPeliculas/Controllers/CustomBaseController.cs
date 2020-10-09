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
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;

        public CustomBaseController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<Tentidad,TDTO>(PaginacionDTO paginacion) where Tentidad : class, IID
        {
            var queryable = context.Set<Tentidad>().AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacion.CantidadRegistroPorPagina);
            var entidades = await queryable.Paginar(paginacion).ToListAsync();


            return mapper.Map<List<TDTO>>(entidades);
        }

        protected async Task<List<TDTO>> Get<Tentidad, TDTO>() where Tentidad : class
        {
            var entidades = await context.Set<Tentidad>().AsNoTracking().ToListAsync();
            var dtos = mapper.Map<List<TDTO>>(entidades);
            return dtos;
        }


        protected async Task<ActionResult<TDTO>> Get<Tentidad, TDTO>(int id) where Tentidad : class, IID
        {
            var entidades = await context.Set<Tentidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if(entidades == null)
            {
                return NotFound();
            }
            var dtos = mapper.Map<TDTO>(entidades);
            return dtos;
        }
        protected async Task<ActionResult> Post<Tcreacion, Tentidad, Tlectura>
            (Tcreacion creacionDTO, string nombreRuta) where Tentidad : class, IID
        {
            var entidad = mapper.Map<Tentidad>(creacionDTO);
            context.Add(entidad);
            await context.SaveChangesAsync();

            var GeneroDTO = mapper.Map<Tlectura>(entidad);

            return new CreatedAtRouteResult(nombreRuta, new { id = entidad.Id }, GeneroDTO);
        }

        protected async Task<ActionResult> Put<Tcreacion, Tentidad>(int id, Tcreacion creacionDTO ) where Tentidad : class, IID
        {
            var entidad = mapper.Map<Tentidad>(creacionDTO);

            entidad.Id = id;
            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Patch<Tentidad, TDTO>(int id, JsonPatchDocument<TDTO> patchDocument)
       where TDTO : class
        where Tentidad : class, IID
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var entidad = await context.Set<Tentidad>().FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }


            var entidadDTO = mapper.Map<TDTO>(entidad);

            patchDocument.ApplyTo(entidadDTO, ModelState);

            var esValido = TryValidateModel(entidadDTO);

            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(entidadDTO, entidad);

            await context.SaveChangesAsync();

            return NoContent();
        }

        protected async Task<ActionResult> Delete<Tentidad>(int id) where Tentidad : class, IID, new()
        {
            var existe = await context.Set<Tentidad>().AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Tentidad { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
