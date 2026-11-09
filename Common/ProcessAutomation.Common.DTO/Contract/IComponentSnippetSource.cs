using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Contract
{
    public interface IComponentSnippetSource
    {
        T ProcessSnippetMethod<T>(string method, object[] parameters);
    }
}
