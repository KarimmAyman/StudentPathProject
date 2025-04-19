using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Dtoes
{
    public class SwaggerIgnoreFilter :ISchemaFilter
    {

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context?.Type == null)
                return;

            var ignoredProps = context.Type
                .GetProperties()
                .Where(t => t.GetCustomAttributes(typeof(SwaggerIgnoreAttribute), true).Any());

            foreach (var prop in ignoredProps)
            {
                var propertyToRemove = schema.Properties.Keys
                    .FirstOrDefault(x => string.Equals(x, prop.Name, StringComparison.OrdinalIgnoreCase));

                if (propertyToRemove != null)
                {
                    schema.Properties.Remove(propertyToRemove);
                }
            }
        }
    }
}