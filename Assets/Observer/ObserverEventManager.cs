using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace Observer
{
    public class ObserverEventManager : MonoBehaviour
    {
        private static Observer instance;
        private static Observer Instance
        {
            get
            {
                if (instance == null)
                    instance = new Observer();
                return instance;
            }
        }
        
        public static void attach(string observerName, Delegate method)
        {
            Instance.Attach(observerName, method);
        }
        public static void detach(string observerName, Delegate method)
        {
            Instance.Detach(observerName,method);
        }
        public static void notify(string observerName, params object[] args )
        {
            Instance.Notify(observerName, args);
        }
    }
}

