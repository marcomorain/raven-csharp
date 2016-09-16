using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace SharpRaven
{
    /// <summary>
    /// Utilities that deal with making C# stack-traces more friendly for humans.
    /// </summary>
    public static class StackTraceUtil
    {
        static readonly IEnumerable<Action<ExceptionFrame>> DefaultProcessors = new Action<ExceptionFrame> [] { FixAsyncMoveNext };

        /// <summary>Clean up the the `async` state machine calls.</summary>
        public static void FixAsyncMoveNext(ExceptionFrame frame)
        {
            if (frame.Module == null || frame.Function != "MoveNext")
            {
                return;
            }

            //  Search for the function name in angle brackets followed by d__<digits>.
            //
            // Change:
            //   RemotePrinterService+<UpdateNotification>d__24 in MoveNext at line 457:13
            // to:
            //   RemotePrinterService in UpdateNotification at line 457:13
            var mangled = @"^(.*)\+<(\w*)>d__\d*$";
            var match = Regex.Match(frame.Module, mangled);
            if (match.Success && match.Groups.Count == 3)
            {
                frame.Module = match.Groups[1].Value;
                frame.Function = match.Groups[2].Value;
            }
        }

        /// <summary>
        /// Apply the given ExceptionFrame processors to all frames in exception.
        /// </summary>
        /// <remarks>
        /// The goal here is to seperate iteration over stackframes from the processing of stackframes.
        /// </remarks>
        public static void Demangle(SentryException exception, IEnumerable<Action<ExceptionFrame>> proccessors)
        {
            if (exception.Stacktrace == null || exception.Stacktrace.Frames == null)
            {
                return;
            }

            foreach (var frame in exception.Stacktrace.Frames)
            {
                foreach (var processor in proccessors)
                {
                    processor(frame);
                }
            }
        }

        /// <summary>
        /// Apply the built-in set of ExceptionFrame processors to all frames in exception.
        /// </summary>
        public static void Demangle(SentryException exception)
        {
            Demangle(exception, DefaultProcessors);
        }
    }
}

