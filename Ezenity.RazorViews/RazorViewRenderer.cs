using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Ezenity.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Ezenity.RazorViews
{
  public class RazorViewRenderer : IRazorViewRenderer
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly ICompositeViewEngine _viewEngine;

    public RazorViewRenderer(IServiceProvider serviceProvider, IWebHostEnvironment hostingEnvironment, ICompositeViewEngine viewEngine)
    {
      _serviceProvider = serviceProvider;
      _hostingEnvironment = hostingEnvironment;
      _viewEngine = viewEngine;
    }

    public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
    {
      var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

      using (var scope = scopeFactory.CreateScope())
      {
        var tempDataProvider = scope.ServiceProvider.GetRequiredService<ITempDataProvider>();

        var actionContext = new ActionContext(
            new DefaultHttpContext { RequestServices = scope.ServiceProvider },
            new Microsoft.AspNetCore.Routing.RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        );

        using (var stringWriter = new StringWriter())
        {
          //var viewResult = _viewEngine.GetView("~/", viewName, false);
          var viewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: false);


          if (!viewResult.Success && !IsPath(viewName))
          {
            viewResult = _viewEngine.FindView(actionContext, viewName, false);
          }

          if (!viewResult.Success)
          {
            var searchedLocations = viewResult.SearchedLocations ?? Enumerable.Empty<string>();
            var error = $"View '{viewName}' not found. Searched in locations: {string.Join(", ", searchedLocations)}";
            //Console.WriteLine($"View name: {viewName}"); // Log view name
            //Console.WriteLine($"Searched Locations: {string.Join(", ", searchedLocations)}"); // Log searched locations
            //Console.WriteLine(error); // Log this error or throw a more detailed exception
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
      }
    }

    static bool IsPath(string name) =>
        name.StartsWith("~/", StringComparison.Ordinal) ||
        name.StartsWith("/", StringComparison.Ordinal);
  }
}
