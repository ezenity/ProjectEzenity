using Ezenity.Application.Abstractions.Emails;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Ezenity.Infrastructure.Services.EmailTemplates
{
    // TODO: look into making this a Razor Class Library (RCL)
    public class RazorViewToStringRenderer_OLD : IRazorViewToStringRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProdiver;

        public RazorViewToStringRenderer_OLD(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProdiver)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProdiver = serviceProdiver;
        }

        public async Task<string> RenderViewtoStringAsync<TModel>(string viewName, TModel model)
        {
            var actionContext = GetActionContext();
            var view = FindView(actionContext, viewName);

            await using var output = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<TModel>(
                    metadataProvider: new EmptyModelMetadataProvider(),
                    modelState: new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(
                    actionContext.HttpContext,
                    _tempDataProvider),
                output,
                new HtmlHelperOptions());

            await view.RenderAsync(viewContext);

            return output.ToString();
        }

        private IView FindView(ActionContext actionContext, string viewName)
        {
            var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (!getViewResult.Success)
            {
              Console.WriteLine("GetView failed. Searched at: " + string.Join(", ", getViewResult.SearchedLocations));
            }

            if (getViewResult.Success)
            {
                return getViewResult.View;
            }

            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (!findViewResult.Success)
            {
              Console.WriteLine("FindView failed. Searched at: " + string.Join(", ", findViewResult.SearchedLocations));
            }

            if (findViewResult.Success)
            {
                return findViewResult.View;
            }

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProdiver
            };

            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
