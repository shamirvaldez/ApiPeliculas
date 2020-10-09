
using ApiPeliculas.DTOs;
using ApiPeliculas.Entidades;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Controllers
{
    [Route("api/SalasDeCine")]
    [ApiController]
    public class SalasDeCineController :CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public SalasDeCineController(ApplicationDbContext context, IMapper mapper) 
            :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<SalaDeCineDTO>>> Get()
        {
            return await Get<SalasDeCine, SalaDeCineDTO>();
        }

        [HttpGet("{id:int}",Name ="ObtenerSalaDeCine" )]
        public async Task<ActionResult<SalaDeCineDTO>> Get(int id)
        {
            return await Get<SalasDeCine, SalaDeCineDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post(SalaDeCineCreacionDTO salaDeCineCreacionDTO)
        {
            return await Post<SalaDeCineCreacionDTO, SalasDeCine, SalaDeCineDTO>(salaDeCineCreacionDTO, "ObtenerSalaDeCine");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, SalaDeCineCreacionDTO salaDeCineCreacionDTO)
        {
            return await Put<SalaDeCineCreacionDTO, SalasDeCine>(id, salaDeCineCreacionDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalasDeCine>(id);
        }
    }
}
