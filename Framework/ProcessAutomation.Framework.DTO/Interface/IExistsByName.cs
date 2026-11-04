using ProcessAutomation.Framework.DTO.Info;

namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IExistsByName
    {
        bool ExistsByName(string name, int excludeThisIdentifier = 0);
    }
}