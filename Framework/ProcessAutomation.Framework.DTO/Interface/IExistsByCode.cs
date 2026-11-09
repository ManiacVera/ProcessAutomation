namespace ProcessAutomation.Framework.DTO.Interface
{
    public interface IExistsByCode
    {
        bool ExistsByCode(string code, int excludeThiIdentifier = 0);
    }
}