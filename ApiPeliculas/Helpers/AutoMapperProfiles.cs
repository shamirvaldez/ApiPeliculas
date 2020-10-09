using ApiPeliculas.DTOs;
using ApiPeliculas.Entidades;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genero, GenerosDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<SalasDeCine, SalaDeCineDTO>().ReverseMap();
            CreateMap<SalaDeCineCreacionDTO,SalasDeCine>();

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<CreacionActorDTO, Actor>().ForMember(x => x.Foto, options => options.Ignore());
            CreateMap<ActorPatchDTO, Actor>().ReverseMap();

            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            CreateMap<CreacionPeliculasDTO, Pelicula>().ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.peliculaGeneros, options => options.MapFrom(MaPeliculasGeneros))
                .ForMember(x => x.peliculasActores, options => options.MapFrom(MapPeliculasActores));

            CreateMap<Pelicula, PeliculasDetallesDto>()
            .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculasGeneros))
            .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculasActores));

            CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();
        }

        private List<ActoPeliculaDetalleDTO> MapPeliculasActores(Pelicula pelicula, PeliculasDetallesDto peliculasDetallesDto)
        {
            var resultado = new List<ActoPeliculaDetalleDTO>();
            if(pelicula.peliculasActores == null) { return resultado; }
            foreach(var actorPelicula in pelicula.peliculasActores)
            {
                resultado.Add(new ActoPeliculaDetalleDTO() { 
                
                ActorId = actorPelicula.ActorId,
                Personaje = actorPelicula.personaje,
                NombrePersona = actorPelicula.actor.Nombre
                
                
                }); 
                      
            }

            return resultado;
        }
        private List<GenerosDTO> MapPeliculasGeneros(Pelicula pelicula, PeliculasDetallesDto peliculasDetallesDto)
        {
            var resultado = new List<GenerosDTO>();
            if(pelicula.peliculaGeneros == null) { return resultado; }
            foreach(var generoPelicula in pelicula.peliculaGeneros)
            {
                resultado.Add(new GenerosDTO() { Id = generoPelicula.GeneroId, Nombre = generoPelicula.Genero.Nombre });
            }

            return resultado;
        }

        private List<PeliculaGeneros> MaPeliculasGeneros(CreacionPeliculasDTO creacionPeliculasDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculaGeneros>();
            if (creacionPeliculasDTO.GenerosIds == null) { return resultado; }
            foreach(var id in creacionPeliculasDTO.GenerosIds)
            {
                resultado.Add(new PeliculaGeneros() { GeneroId = id });
            }

            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(CreacionPeliculasDTO creacionPeliculasDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if(creacionPeliculasDTO.actores == null) { return resultado; };

            foreach(var actores in creacionPeliculasDTO.actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actores.ActorId, personaje = actores.Personaje });
            }

            return resultado;
        }
    }
}
