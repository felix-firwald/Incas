using Common;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Common
{
    public enum PseudoBoolean // потому что эта тупая хуета не десериализует
    {
        Yes,
        No
    }
    public class UserParameters
    {
        public bool tasks_visibility { get; set; }
        public bool communication_visibility { get; set; }
        public bool database_visibility { get; set; }

        public bool create_templates { get; set; }
        public bool modify_templates { get; set; }
        public string startup_password { get; set; }
        public string password { get; set; }
        public PermissionGroup permission_group { get; set; }

        public bool IsRightPassword(string input) // всегда false потому что я еще не записывал в UserParameters через UserEditor пароль
        {
            if (string.IsNullOrEmpty(password))
            {
                return input == startup_password;
            }
            return input == password;
        }

        public void ApplyStandartProperties(string pwd)
        {
            tasks_visibility = true;
            communication_visibility = true;
            database_visibility = false;
            create_templates = false;
            modify_templates = false;
            startup_password = pwd;
            permission_group = PermissionGroup.Operator;
        }
        public void ApplyAdminProperties(string pwd)
        {
            tasks_visibility = true;
            communication_visibility = true;
            database_visibility = true;
            create_templates = true;
            modify_templates = true;
            startup_password = pwd;
            permission_group = PermissionGroup.Admin;
        }

    }
    public static class UserContextor
    {
        private static UserParameters FromFile(User user, string key)
        {
            string path = $"{ProgramState.UsersContext}\\{user.sign}{user.id}.enic";
            try
            {
                string output = Cryptographer.DecryptString(key, File.ReadAllText(path));
                return JsonConvert.DeserializeObject<UserParameters>(output);
            }
            catch (FileNotFoundException)
            {
                ProgramState.ShowErrorDialog($"Служебный файл пользователя не найден. " +
                    $"Инкубатор попробует исправить ситуацию.", "Данные отсутствуют");
                if (user.id == 1)
                {
                    UserParameters p = new();
                    p.ApplyAdminProperties(ProgramState.GenerateSlug(4));
                    ToFile(p, user, GetKey(user));
                    ProgramState.ShowExlamationDialog($"Пароль для пользователя {user.fullname} был сброшен. Временный пароль: {p.startup_password}", "Пользователь восстановлен");
                    return p;
                }
                else
                {
                    UserParameters p = new();
                    p.ApplyStandartProperties(ProgramState.GenerateSlug(4));
                    ToFile(p, user, GetKey(user));
                    ProgramState.ShowExlamationDialog($"Пароль для пользователя {user.fullname} был сброшен. Статус установлен на \"оператор\". Временный пароль: {p.startup_password}", "Пользователь восстановлен");
                    return p;
                }
            }
            //catch (IOException ex)
            //{
            //    if (ProgramState.ShowQuestionDialog($"Возникла непредвиденная ошибка: {ex}.\nПопробовать снова?", "Возникла ошибка") == Windows.DialogStatus.Yes)
            //    {
            //        return FromFile(user, key);
            //    }
            //    else
            //    {
            //        return new();
            //    }
            //}
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog($"Данные о пользователе повреждены.\n{ex}");
                return new();
            }
        }
        private async static void ToFile(UserParameters parameters, User user, string key)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                string content = JsonConvert.SerializeObject(parameters);
                string filename = $"{ProgramState.UsersContext}\\{user.sign}{user.id}.enic";
                File.WriteAllTextAsync(filename, Cryptographer.EncryptString(key, content));
            });
        }
        private static string GetKey(User user)
        {
            string result = Cryptographer.GenerateKey($"{user.id}{user.sign}");
            return result;
        }
        public static UserParameters GetContext(User user)
        {
            return FromFile(user, GetKey(user));
        }
        public static void SaveContext(UserParameters parameters, User user)
        {
            ToFile(parameters, user, GetKey(user));
        }
    }
}
