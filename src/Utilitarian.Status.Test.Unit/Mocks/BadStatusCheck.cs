using System;

namespace Utilitarian.Status.Test.Unit.Mocks
{
    public class BadStatusCheck : IStatusCheck
    {
        public bool IsActive()
        {
            throw new Exception();
        }
    }
}
