using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Reflection;

namespace GDGPDFDistiller.Filters
{
    public class RequiredPropertiesSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || schema.Properties.Count == 0)
                return;

            var requiredProps = context.Type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !p.GetCustomAttributes(typeof(OptionalFieldAttribute), true).Any());

            foreach (var prop in requiredProps)
            {
                // Swagger usa nomi camelCase per default
                var swaggerPropertyName = char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);

                if (schema.Properties.ContainsKey(swaggerPropertyName))
                {
                    schema.Required ??= new HashSet<string>();
                    schema.Required.Add(swaggerPropertyName);
                }
            }
        }
    }
}
