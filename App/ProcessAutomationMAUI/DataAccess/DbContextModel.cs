#if __IOS__ || __MACCATALYST__

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
#pragma warning disable 219, 612, 618
#nullable enable

namespace ProcessAutomationMAUI.DataAccess
{
    [DbContext(typeof(StoreDbContext))]
    public partial class DbContextModel : RuntimeModel
    {
        static DbContextModel()
        {
            var model = new DbContextModel();
            model.Initialize();
            model.Customize();
            _instance = model;
        }
        private static DbContextModel _instance;
        public static IModel Instance => _instance;
        partial void Initialize();
        partial void Customize();
    }
}

#endif