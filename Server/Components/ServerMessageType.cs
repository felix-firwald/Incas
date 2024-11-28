using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Server.Components
{
    public enum ServerMessageType
    {
        // RESPONSES
        InitialResponse, // выдаю список пользователей
        AuthorizationFailed, // неправильный пароль при входе
        AuthorizationSuccess, // пароль правильный, входи
        DatabaseDataResult, // возвращает DataTable

        // REQUESTS
        RefreshAll, // обновить вид главного окна
        Shutdown, // немедленно закрыть программу
        ShowExplicitDialog, // показать окно восклицания
        ShowErrorDialog, // показать окно ошибки
        ShowInfoDialog, // показать окно информации
        ShowSimpleFormDialog // показать окно DSF
    }
}
