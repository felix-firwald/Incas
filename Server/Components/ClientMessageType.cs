using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Server.Components
{
    public enum ClientMessageType
    {
        // REQUESTS
        InitialRequest = 0, // получение списка пользователей из РП
        Authorization = 1, // попытка залогиниться под пользователем с паролем
        ServiceAction = 20, // для вызова sql запроса в служебной базе данных Execute(), ExecuteOne(), ExecuteVoid()
        ObjectAction = 30, // для вызова любых sql манипуляций с пользовательскими объектами 
        DestroyConnection = -1 // разрыв соединения

        // RESPONSES
    }
}
