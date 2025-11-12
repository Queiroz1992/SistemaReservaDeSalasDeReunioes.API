using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SistemaReservaDeSalasDeReunioes.API.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class ValidateRouteIdAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ActionArguments.TryGetValue("id", out var routeIdObj) || routeIdObj is not int routeId)
            return;

        // Encontrar o primeiro argumento com uma propriedade Id do tipo int (DTO)
        foreach (var arg in context.ActionArguments.Values.Where(v => v is not null))
        {
            var type = arg!.GetType();
            var idProp = type.GetProperty("Id");
            if (idProp is null || idProp.PropertyType != typeof(int)) continue;
            var bodyId = (int)idProp.GetValue(arg)!;
            if (bodyId != routeId)
            {
                context.ModelState.AddModelError("Id", "Id da rota difere do Id do corpo.");
                var problem = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Ocorreu um ou mais erros de validação.",
                };
                context.Result = new BadRequestObjectResult(problem);
            }
            break;
        }
    }
}
