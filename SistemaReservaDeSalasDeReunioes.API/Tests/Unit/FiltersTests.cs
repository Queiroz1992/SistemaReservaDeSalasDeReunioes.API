using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SistemaReservaDeSalasDeReunioes.API.Filters;
using Xunit;

namespace SistemaReservaDeSalasDeReunioes.API.Tests.Unit;

public class ValidateRouteIdAttributeTests
{
    private static ActionExecutingContext BuildContext(int routeId, object? bodyDto)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor(),
            new ModelStateDictionary());
        var actionArguments = new Dictionary<string, object?>();
        actionArguments["id"] = routeId;
        if (bodyDto != null) actionArguments["dto"] = bodyDto;
        return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), actionArguments, controller: null!);
    }

    [Fact]
    public void Should_Not_SetResult_When_Id_Matches()
    {
        var attr = new ValidateRouteIdAttribute();
        var dto = new { Id = 5, Nome = "Teste" };
        var ctx = BuildContext(5, dto);
        attr.OnActionExecuting(ctx);
        Assert.Null(ctx.Result); // no error
        Assert.True(ctx.ModelState.IsValid);
    }

    [Fact]
    public void Should_SetBadRequestResult_When_Id_Differs()
    {
        var attr = new ValidateRouteIdAttribute();
        var dto = new { Id = 10, Nome = "Teste" };
        var ctx = BuildContext(9, dto);
        attr.OnActionExecuting(ctx);
        Assert.NotNull(ctx.Result);
        var bad = Assert.IsType<BadRequestObjectResult>(ctx.Result);
        var problem = Assert.IsType<ValidationProblemDetails>(bad.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Contains("Id da rota difere", problem.Errors["Id"].First());
    }

    [Fact]
    public void Should_Ignore_When_No_Body_Dto()
    {
        var attr = new ValidateRouteIdAttribute();
        var ctx = BuildContext(3, null);
        attr.OnActionExecuting(ctx);
        Assert.Null(ctx.Result);
        Assert.True(ctx.ModelState.IsValid);
    }

    [Fact]
    public void Should_Ignore_When_Body_Has_No_Id_Property()
    {
        var attr = new ValidateRouteIdAttribute();
        var dto = new { Other = 1 };
        var ctx = BuildContext(1, dto);
        attr.OnActionExecuting(ctx);
        Assert.Null(ctx.Result);
        Assert.True(ctx.ModelState.IsValid);
    }
}

public class ValidateModelAttributeTests
{
    [Fact]
    public void Should_SetBadRequest_When_ModelState_Invalid()
    {
        var attr = new ValidateModelAttribute();
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor(),
            new ModelStateDictionary());
        actionContext.ModelState.AddModelError("Campo", "Erro de validação");
        var ctx = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), controller: null!);
        attr.OnActionExecuting(ctx);
        var bad = Assert.IsType<BadRequestObjectResult>(ctx.Result);
        var problem = Assert.IsType<ValidationProblemDetails>(bad.Value);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.Status);
        Assert.Contains("Erro de validação", problem.Errors["Campo"].First());
    }

    [Fact]
    public void Should_Not_SetResult_When_ModelState_Valid()
    {
        var attr = new ValidateModelAttribute();
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor(),
            new ModelStateDictionary());
        var ctx = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), controller: null!);
        attr.OnActionExecuting(ctx);
        Assert.Null(ctx.Result);
    }
}
