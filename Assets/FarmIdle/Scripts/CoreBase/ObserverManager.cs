using System;
using UnityEngine;

namespace Observer
{
    public class ObserverManager : MonoBehaviour
    {
        public static ObserverManager Instance { get; private set; }

        public Observer observer = new Observer();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Attach(string name, Delegate method)
        {
            observer.Attach(name, method);
        }

        public void Detach(string name, Delegate method)
        {
            observer.Detach(name, method);
        }

        public void Notify(string name, params object[] args)
        {
            observer.Notify(name, args);
        }
    }
}