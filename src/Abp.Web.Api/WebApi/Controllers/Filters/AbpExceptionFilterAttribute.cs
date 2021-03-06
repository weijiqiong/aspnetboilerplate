using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Adorable.Dependency;
using Adorable.Events.Bus;
using Adorable.Events.Bus.Exceptions;
using Adorable.Logging;
using Adorable.Web.Models;
using Castle.Core.Logging;

namespace Adorable.WebApi.Controllers.Filters
{
    /// <summary>
    /// Used to handle exceptions on web api controllers.
    /// </summary>
    public class AbpExceptionFilterAttribute : ExceptionFilterAttribute, ITransientDependency
    {
        /// <summary>
        /// Reference to the <see cref="ILogger"/>.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Reference to the <see cref="IEventBus"/>.
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpExceptionFilterAttribute"/> class.
        /// </summary>
        public AbpExceptionFilterAttribute()
        {
            Logger = NullLogger.Instance;
            EventBus = NullEventBus.Instance;
        }

        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            var wrapResultAttribute = HttpActionDescriptorHelper.GetWrapResultAttributeOrNull(context.ActionContext.ActionDescriptor);

            if (wrapResultAttribute == null)
            {
                var returnType = context.ActionContext.ActionDescriptor.ReturnType;
                if (returnType == typeof(AjaxResponse) || returnType == typeof(Task<AjaxResponse>))
                {
                    wrapResultAttribute = WrapResultAttribute.Default;
                }
                else
                {
                    wrapResultAttribute = DontWrapResultAttribute.Default;                    
                }
            }
            
            if (wrapResultAttribute.LogError)
            {
                LogHelper.LogException(Logger, context.Exception);
            }

            if (wrapResultAttribute.WrapOnError)
            {
                context.Response = context.Request.CreateResponse(
                    HttpStatusCode.OK, //TODO: Consider to return 500
                    new AjaxResponse(
                        ErrorInfoBuilder.Instance.BuildForException(context.Exception),
                        context.Exception is Adorable.Authorization.AbpAuthorizationException)
                    );

                EventBus.Trigger(this, new AbpHandledExceptionData(context.Exception));
            }
        }
    }
}