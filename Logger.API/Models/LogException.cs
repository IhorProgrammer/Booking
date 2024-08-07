namespace Logger.API.Models
{
    public class LogException : Exception
    {
        public String CustomStackTrace { get; set; }
        public LogException(LogModel logModel) : base(logModel.Message)
        {
            this.CustomStackTrace = logModel.Stacktrace + "";
        }

        public override string StackTrace
        {
            get
            {
                return CustomStackTrace;
            }
        }
    }
}
