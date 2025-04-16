using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
namespace Observer
{
    public class Observer : IObserver
    {
        private Dictionary<string, Delegate> observers = new Dictionary<string, Delegate>();
        public void Attach(string obsName, Delegate method)
        {
            if (!observers.ContainsKey(obsName))
            {
                observers.Add(obsName, method);
            }
            else
            {
                observers[obsName] = Delegate.Combine(observers[obsName], method);
            }
        }
        public void Detach(string obsName, Delegate method)
        {
            if (observers.ContainsKey(obsName))
            {
                observers[obsName] = Delegate.Remove(observers[obsName], method);
            }
        }
        public void Notify(string obsName, params object[] args)
        {
            if (observers.ContainsKey(obsName))
            {
                Delegate del = observers[obsName];
                if (del != null)
                {
                    if (del.GetInvocationList().Length > 0)
                    {
                        foreach (Delegate handler in del.GetInvocationList())
                        {
                            if (handler.Method.GetParameters().Length == 0)
                            {
                                handler.DynamicInvoke();
                            }
                            else
                            {
                                ParameterInfo[] parametersInfo = handler.Method.GetParameters();
                                object[] parameters = new object[parametersInfo.Length];

                                for (int i = 0; i < parametersInfo.Length; i++)
                                {
                                    if (i < args.Length && args[i] != null && args[i].GetType() == parametersInfo[i].ParameterType)
                                    {
                                        parameters[i] = args[i];
                                    }
                                    else
                                    {
                                        parameters[i] = GetDefaultValue(parametersInfo[i].ParameterType);
                                    }
                                }
                                handler.DynamicInvoke(parameters);
                            }
                        }
                    }
                }
            }
        }
        private object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
