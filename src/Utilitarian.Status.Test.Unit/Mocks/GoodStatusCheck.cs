namespace Utilitarian.Status.Test.Unit.Mocks
{
    public class GoodStatusCheck : IStatusCheck
    {
        public bool IsActive()
        {
            return true;
        }
    }
}
