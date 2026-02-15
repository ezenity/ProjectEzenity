using Ezenity.Application.Abstractions.Emails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ezenity.EmailTemplates;

/// <summary>
/// Renders a Razor view to a string (used for HTML emails).
///
/// Key notes:
/// - Use isMainPage: TRUE so Layout + _ViewStart can run (email layout support).
/// - Supports absolute app-relative view paths like:
///     "~/Views/EmailTemplates/EmailVerification.cshtml"
///     "/Views/EmailTemplates/EmailVerification.cshtml"
/// - If the view cannot be found, throws an exception that includes searched locations.
/// </summary>
public class RazorViewRenderer : IRazorViewRenderer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICompositeViewEngine _viewEngine;

    public RazorViewRenderer(IServiceProvider serviceProvider, ICompositeViewEngine viewEngine)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _viewEngine = viewEngine ?? throw new ArgumentNullException(nameof(viewEngine));
    }

    /// <summary>
    /// Render the specified Razor view to a string.
    /// </summary>
    public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
    {
        if (string.IsNullOrWhiteSpace(viewName))
            throw new ArgumentException("viewName cannot be null/empty.", nameof(viewName));

        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

        using var scope = scopeFactory.CreateScope();

        var tempDataProvider = scope.ServiceProvider.GetRequiredService<ITempDataProvider>();

        // Minimal ActionContext for rendering
        var actionContext = new ActionContext(
            new DefaultHttpContext { RequestServices = scope.ServiceProvider },
            new Microsoft.AspNetCore.Routing.RouteData(),
            new ActionDescriptor()
        );

        await using var stringWriter = new StringWriter();

        // IMPORTANT: isMainPage = true so Layout works
        var isMainPage = true;

        // 1) If viewName is a path (~/ or /), use GetView
        var viewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: isMainPage);

        // 2) Otherwise, try FindView (view location searching)
        if (!viewResult.Success && !IsPath(viewName))
        {
            viewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: isMainPage);
        }

        if (!viewResult.Success)
        {
            var searchedLocations = viewResult.SearchedLocations ?? Enumerable.Empty<string>();
            var error =
                $"View '{viewName}' not found. " +
                $"Searched in: {string.Join(", ", searchedLocations)}";

            throw new InvalidOperationException(error);
        }

        var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
        {
            Model = model
        };

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDictionary,
            new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
            stringWriter,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);

        return stringWriter.ToString();
    }

    /// <summary>
    /// Determines whether a view name is a virtual path.
    /// </summary>
    private static bool IsPath(string name) =>
        name.StartsWith("~/", StringComparison.Ordinal) ||
        name.StartsWith("/", StringComparison.Ordinal);
}
