using System.Collections.Generic;
using Lide.Core.Model.Settings;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lide.WebAPI.Extension;

public class EnableSwaggerHeaders : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        if (context.ApiDescription.RelativePath != Endpoint.LideReplayEndpoint)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Lide.Enabled",
                @In = ParameterLocation.Header,
                Examples = new Dictionary<string, OpenApiExample>()
                {
                    { "Default", new OpenApiExample { Value = new OpenApiBoolean(false) } },
                    { "Enabled", new OpenApiExample { Value = new OpenApiBoolean(true) } },
                },
                Required = false,
            });
        }

        if (context.ApiDescription.RelativePath != Endpoint.LideReplayEndpoint)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Lide.PropagateSettings",
                @In = ParameterLocation.Header,
                Description = "Json or JsonToBase64 formatted of PropagateSettings object",
                Examples = new Dictionary<string, OpenApiExample>()
                {
                    { "None", new OpenApiExample { Value = new OpenApiString("") } },
                    { "Replay", new OpenApiExample { Value = new OpenApiString("eyJPdmVycmlkZURlY29yYXRvcnNXaXRoUGF0dGVybiI6dHJ1ZSwiRGVjb3JhdG9yc1dpdGhQYXR0ZXJuIjpbIkxpZGUuU3Vic3RpdHV0ZS5SZXBsYXlcdTAwMkIqIl19") } },
                    { "Record", new OpenApiExample { Value = new OpenApiString("eyJPdmVycmlkZURlY29yYXRvcnNXaXRoUGF0dGVybiI6dHJ1ZSwiRGVjb3JhdG9yc1dpdGhQYXR0ZXJuIjpbIkxpZGUuU3Vic3RpdHV0ZS5SZWNvcmRcdTAwMkIqIl19") } },
                },
                Required = false,
            });
        }
        else
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Lide.PropagateSettings",
                @In = ParameterLocation.Header,
                Description = "Json or JsonToBase64 formatted of PropagateSettings object",
                Examples = new Dictionary<string, OpenApiExample>()
                {
                    { "Replay", new OpenApiExample { Value = new OpenApiString("eyJPdmVycmlkZURlY29yYXRvcnNXaXRoUGF0dGVybiI6dHJ1ZSwiRGVjb3JhdG9yc1dpdGhQYXR0ZXJuIjpbIkxpZGUuU3Vic3RpdHV0ZS5SZXBsYXlcdTAwMkIqIl19") } },
                },
                Required = true,
                AllowEmptyValue = false,
            });
        }
    }
}