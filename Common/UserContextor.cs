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

        public void ApplyStandartProperties()
        {
            tasks_visibility = true;
            communication_visibility = true;
            database_visibility = true;
            create_templates = true;
            modify_templates = true;
        }

    }
    public static class UserContextor
    {
        private static UserParameters FromFile(User user, string key)
        {
            try
            {
                string path = $"{ProgramState.UsersContext}\\{user.sign}{user.id}.enic";
                string output = Cryptographer.DecryptString(key, File.ReadAllText(path));
                return JsonConvert.DeserializeObject<UserParameters>(output);
            }
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
