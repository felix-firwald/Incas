using Incas.Core.Classes;
using Incas.Objects.Documents.Components;
using Incas.Objects.Engine;
using Incas.Objects.ServiceClasses.Groups.Components;
using Incas.Objects.ServiceClasses.Groups.Views.Controls;
using Incas.Objects.ServiceClasses.Users.Components;
using Incas.Objects.ServiceClasses.Users.Views.Controls;

namespace Incas.Objects.Components
{
    public static class ServiceExtensionFieldsManager
    {
        public static IServiceFieldFiller GetFillerByType(IObject obj)
        {
            if (obj is Group objGroup)
            {
                return new GroupSettings().SetUp(objGroup);
            }
            else if (obj is User objUser)
            {
                return new UserSettings().SetUp(objUser);
            }
            return null;
        }
    }
}
