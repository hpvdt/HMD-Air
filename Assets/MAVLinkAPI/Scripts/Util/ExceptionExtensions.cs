using System;
using System.Reflection;
using System.Text;

namespace MAVLinkAPI.Scripts.Util
{
    internal static class ExceptionExtensions
    {
        public static string GetMessageForDisplay(this Exception exception)
        {
            var messageForDisplay =
                exception != null ? exception.Message : throw new ArgumentNullException(nameof(exception));
            if (exception is AggregateException aggregateException1)
            {
                var aggregateException = aggregateException1.Flatten();
                if (aggregateException.InnerExceptions.Count == 1)
                {
                    messageForDisplay = aggregateException.InnerException.GetMessageForDisplay();
                }
                else if (aggregateException.InnerExceptions.Count > 0)
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(">>> Caused by:");
                    foreach (var innerException in aggregateException.InnerExceptions)
                    {
                        var sub = innerException.GetMessageForDisplay();
                        var indented = $"[{aggregateException.InnerExceptions.IndexOf(innerException)}] " +
                                       sub.Block().Indent(indentFirstLine: false);

                        stringBuilder.AppendLine(indented);
                    }

                    messageForDisplay = stringBuilder.ToString();
                }
            }
            else if (exception is TargetInvocationException invocationException &&
                     invocationException.InnerException != null)
            {
                messageForDisplay = invocationException.InnerException.GetMessageForDisplay();
            }

            return messageForDisplay;
        }
    }
}