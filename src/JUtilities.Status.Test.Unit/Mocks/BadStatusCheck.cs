using System;

namespace JUtilities.Status.Test.Unit.Mocks
{
    public class BadStatusCheck : IStatusCheck
    {
        public bool IsActive()
        {
            throw new Exception();
        }
    }
}
