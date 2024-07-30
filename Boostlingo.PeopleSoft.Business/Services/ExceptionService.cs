namespace Boostlingo.PeopleSoft.Business.Services;
using Serilog;

public interface IExceptionService
{
    void LogException(Exception ex);
}

public class ExceptionService : IExceptionService
{
    public void LogException(Exception ex)
    {
        // Extract relevant details
        var exceptionDetails = new
        {
            Type = ex.GetType().Name,
            ex.Message,
            ex.StackTrace,
            ex.Source,
            TargetSite = ex.TargetSite?.ToString(),
            InnerException = ex.InnerException != null ? new
            {
                Type = ex.InnerException.GetType().Name,
                ex.InnerException.Message,
                ex.InnerException.StackTrace
            } : null
        };

        // Log exception
        Log.ForContext("Category", "ExceptionService").Error("An exception occurred: {@ExceptionDetails}", exceptionDetails);
    }
}
