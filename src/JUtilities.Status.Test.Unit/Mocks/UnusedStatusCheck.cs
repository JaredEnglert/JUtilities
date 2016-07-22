using System;

namespace JUtilities.Status.Test.Unit.Mocks
{
    public class UnusedStatusCheck : IStatusCheck
    {
        public bool IsActive()
        {
            throw new NotImplementedException();
        }
    }
}
