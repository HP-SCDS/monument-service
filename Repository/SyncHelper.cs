namespace MonumentService.Repository
{
    public static class SyncHelper
    {
        public static T OperationWithSpinLock<T>(ref SpinLock slock, Func<T> operation)
        {
            bool lockTaken = false;
            try
            {
                slock.Enter(ref lockTaken);
                return operation();
            }
            finally
            {
                if (lockTaken)
                {
                    slock.Exit();
                }
            }
        }
    }
}
