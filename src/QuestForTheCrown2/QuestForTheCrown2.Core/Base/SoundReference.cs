using System;

namespace QuestForTheCrown2.Base
{
    public class SoundReference : IDisposable
    {
        Action _onDispose;

        SoundReference(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public static IDisposable Create(Action onDispose)
        {
            return new SoundReference(onDispose);
        }

        #region IDisposable implementation

        ~SoundReference()
        {
            Dispose(false);
        }

        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                    _disposed = true;
                    _onDispose();
            }
        }

        #endregion
    }
}