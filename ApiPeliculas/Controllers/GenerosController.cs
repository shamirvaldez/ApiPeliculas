using ApiPeliculas.DTOs;
using ApiPeliculas.Entidades;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Route("api/generos")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext Context;

        private readonly IMapper Mapper;

        public GenerosController(ApplicationDbContext context, IMapper mapper)
        {
            this.Context = context;
            this.Mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenerosDTO>>> Get()
        {
            var entidades = await Context.Generos.ToListAsync();

            var dtos = Mapper.Map<List<GenerosDTO>>(entidades);

            return dtos;
        }

        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GenerosDTO>> Get(int id)
        {
            var entidad = await Context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if(entidad == null)
            {
                return NotFound();

            }

            var dto = Mapper.Map<GenerosDTO>(entidad);

            return dto;
        }

        [HttpPost]

        public async Task<ActionResult> Post ([FromBody] GeneroCreacionDTO generacionDto)
        {
            var entidad = Mapper.Map<Genero>(generacionDto);
            Context.Add(entidad);
            await Context.SaveChangesAsync();

            var GeneroDTO = Mapper.Map<GenerosDTO>(entidad);

            return new CreatedAtRouteResult("obtenerGenero", new { id = GeneroDTO.Id }, GeneroDTO);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generacionDto)
        {
            var entidad = Mapper.Map<Genero>(generacionDto);

            entidad.Id = id;
            Context.Entry(entidad).State = EntityState.Modified;
            await Context.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await Context.Generos.AnyAsync(x => x.Id == id);

            if(!existe)
            {
                return NotFound();
            }

            Context.Remove(new Genero { Id = id });
            await Context.SaveChangesAsync();

            return NoContent();
        }

    }
}
