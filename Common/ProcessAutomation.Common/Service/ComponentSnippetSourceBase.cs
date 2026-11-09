using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProcessAutomation.Common.Service
{
    public abstract class ComponentSnippetSourceBase
    {
        private Dictionary<string, string> snippetMethodList = null;

        public ComponentSnippetSourceBase()
        {
            snippetMethodList = new Dictionary<string, string>();
            AddMethodList();
        }

        protected abstract void AddMethodList();

        protected void AddSnippetMethod(string aliasMethod, string snippetMethod)
        {
            if (!snippetMethodList.ContainsKey(aliasMethod))
            {
                snippetMethodList.Add(aliasMethod, snippetMethod);
            }
        }

        public T ProcessSnippetMethod<T>(string method, object[] parameters)
        {
            T result = default(T);

            if (snippetMethodList.ContainsKey(method))
            {
                try
                {
                    result = (T)Convert.ChangeType(GetType().InvokeMember(snippetMethodList[method], BindingFlags.InvokeMethod, null, null, parameters)
                    , typeof(T));
                }
                catch(Exception)
                {
                }
            }

            return result;
        }
    }
}
