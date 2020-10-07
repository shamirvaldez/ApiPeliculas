using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var nombrePropiedad = bindingContext.ModelName;
            var proveedorDevalores = bindingContext.ValueProvider.GetValue(nombrePropiedad);

            if(proveedorDevalores == null)
            {
                return Task.CompletedTask;
            }

            try
            {
                var ValorDeserializado = JsonConvert.DeserializeObject<T>(proveedorDevalores.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(ValorDeserializado);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(nombrePropiedad, "Valor invalido para tipo List<int>");
            }

            return Task.CompletedTask;
        }
    }
}
