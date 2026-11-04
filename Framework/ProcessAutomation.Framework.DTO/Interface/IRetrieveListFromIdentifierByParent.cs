using System.Collections.Generic;
using ProcessAutomation.Framework.DTO.Enum;
using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IRetrieveListFromIdentifierByParent<T_INFO, T_IDENTIFIER, T_PARENT_IDENTIFIER>
    {
        ResponseInfo<List<T_INFO>> RetrieveListFromIdentifierByParent(T_IDENTIFIER identifier, T_PARENT_IDENTIFIER parentIdentifier
        , ListFromIdentifierEnum fromEnum = ListFromIdentifierEnum.GreaterThan);
    }
}