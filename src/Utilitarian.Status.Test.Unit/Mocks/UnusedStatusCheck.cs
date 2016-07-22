using System;

namespace Utilitarian.Status.Test.Unit.Mocks
{
    public class UnusedStatusCheck : IStatusCheck
    {
        public bool IsActive()
        {
            throw new NotImplementedException();
        }
    }
}
