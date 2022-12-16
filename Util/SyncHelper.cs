namespace MonumentService.Util
{
    public static class SyncHelper
    {
        public static void OperationWithLock(ref SpinLock slock, Action operation)
        {
            OperationWithLock(ref slock, () =>
            {
                operation();
                return 0;
            });
        }

        public static T OperationWithLock<T>(ref SpinLock slock, Func<T> operation)
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
