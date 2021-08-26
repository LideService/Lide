using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VendingMachine.Services.Test.Helpers
{
    public static class ExcAssert
    {
        public static void NotThrown(Action action)
        {
            var exceptionIsThrown = true;
            try
            {
                action?.Invoke();
                exceptionIsThrown = false;
            }
            catch (Exception)
            {
                // ignored
            }

            Assert.AreEqual(exceptionIsThrown, false);
        }
    }
}
