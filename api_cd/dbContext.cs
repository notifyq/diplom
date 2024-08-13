using api_CodeFlow.Model;

namespace api_CodeFlow
{
    public class dbContext : database_codeflowContext
    {
        public static database_codeflowContext Context { get; } = new database_codeflowContext();
    }
}
