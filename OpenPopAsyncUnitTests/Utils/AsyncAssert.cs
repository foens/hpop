using System.Threading.Tasks;
using NUnit.Framework;

namespace OpenPopAsyncUnitTests.Utils
{
    public static class AsyncAssert
    {
        public static void Throws<T>(Task task)
        {
            var exceptions = task.Exception;
            Assert.NotNull(exceptions);
            Assert.IsInstanceOf<T>(exceptions.InnerException);
        }

        public static void DoesNotThrow(Task task)
        {
            var exceptions = task.Exception;
            Assert.Null(exceptions);            
        }
    }
}