namespace Logger.API.Models
{
    public class LogException : Exception
    {
        public String CustomStackTrace { get; set; }
        public String CustomLoggerName { get; set; }


        public LogException(LogModel logModel) : base(logModel.Message)
        {
            this.CustomStackTrace = logModel.Stacktrace + "";
            this.CustomLoggerName = logModel.LogLogger;
            
        }

        public override string StackTrace
        {
            get
            {
                return CustomStackTrace;
            }
        }

        public override string Source
        {
            get
            {
                return CustomLoggerName;
            }
        }
    }
}
