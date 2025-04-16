using System;
namespace Observer
{
    public interface IObserver
    {
        void Attach(string obsName, Delegate method);
        void Detach(string obsName, Delegate method);
        void Notify(string obsName, params object[] args);
    }

}
