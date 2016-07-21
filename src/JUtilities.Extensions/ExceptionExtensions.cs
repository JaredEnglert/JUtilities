using System;
using Newtonsoft.Json;

namespace JUtilities.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToJson(this Exception exception)
        {
            return JsonConvert.SerializeObject(new SerializableException(exception));
        }
    }

    [Serializable]
    public class SerializableException
    {
        public string Message { get; set; }

        public string StackTrace { get; set; }

        public SerializableException InnerException { get; set; }

        public int Level { get; set; }

        public SerializableException(Exception exception, int level = 0)
        {
            Message = exception.Message;
            StackTrace = exception.StackTrace;
            Level = level;

            if (exception.InnerException != null) InnerException = new SerializableException(exception.InnerException, level + 1);
        }

        public SerializableException()
        {
        }
    }
}
