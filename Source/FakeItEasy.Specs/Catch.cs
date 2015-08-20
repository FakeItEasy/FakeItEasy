namespace FakeItEasy.Specs
{
    using System;

    public static class Catch
    {
        public static Exception Exception(Action throwingAction)
        {
            return Only<Exception>(throwingAction);
        }

        public static Exception Exception<T>(Func<T> throwingFunc)
        {
            try
            {
                throwingFunc();
            }
            catch (Exception exception)
            {
                return exception;
            }

            return null;
        }

        public static TException Only<TException>(Action throwingAction)
          where TException : Exception
        {
            try
            {
                throwingAction();
            }
            catch (TException exception)
            {
                return exception;
            }

            return null;
        }
    }
}