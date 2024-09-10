using NLog;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace api.Helpers
{
    [PSerializable]
    public class LogAspect : OnMethodBoundaryAspect
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override void OnEntry(MethodExecutionArgs args)
        {
            // Skip constructors
            if (args.Method.IsConstructor)
            {
                return;
            }
            Logger.Info($"Entering method {args.Method.Name} with arguments: {string.Join(", ", args.Arguments)}");
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            // Skip constructors
            if (args.Method.IsConstructor)
            {
                return;
            }
            if (args.Exception == null)
            {
                Logger.Info($"Exiting method {args.Method.Name} executed successfully. Result: {args.ReturnValue}");
            }
            else
            {
                Logger.Error($"Method {args.Method.Name} threw an exception: {args.Exception.Message}");
            }
        }
    }
}