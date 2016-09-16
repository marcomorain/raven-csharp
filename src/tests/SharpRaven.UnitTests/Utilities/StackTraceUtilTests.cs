using NUnit.Framework;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRaven.UnitTests.Utilities
{
    [TestFixture]
    class StackTraceUtilTests
    {
        [Test]
        public void AsyncFramesAreDemangled()
        {
            var frame = new ExceptionFrame(new StackFrame("Test.cs", 13))
            {
                Module = "Foo.Bar.RemotePrinterService+<UpdateNotification>d__24",
                Function = "MoveNext"
            };

            StackTraceUtil.FixAsyncMoveNext(frame);

            Assert.AreEqual("UpdateNotification", frame.Function);
            Assert.AreEqual("Foo.Bar.RemotePrinterService", frame.Module);
        }


        [Test]
        public void AllFramesAreVisited()
        {
            int framesVisited = 0;

            Action<ExceptionFrame> vistor = (e) =>
            {
                framesVisited++;
            };

            var exception = TestHelper.GetException();
            var sentryException = new SentryException(exception);
            var trace = new StackTrace(exception);

            // Call our visitor twice
            StackTraceUtil.Demangle(sentryException, new[] { vistor, vistor });

            Assert.AreEqual(2 * trace.FrameCount, framesVisited);
        }
    }
}
