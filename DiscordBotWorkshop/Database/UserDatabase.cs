using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace DiscordBotWorkshop.Database
{
    public class UserDatabase
    {
        private static readonly string path = "users.json";
        private Dictionary<ulong, User> Users { get; set; }
        /// <summary>
        /// Create new Database if one is not created. Should be only used in Bot.cs.
        /// </summary>
        public UserDatabase()
        {
            Users = new Dictionary<ulong, User>();
        }
        public UserDatabase(Dictionary<ulong, User> data)
        {
            Users = data;
        }
        public static Dictionary<ulong, User> LoadUsers()
        {
            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Dictionary<ulong, User>>(json);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Save Users dictionary.
        /// </summary>
        /// <param name="database"></param>
        public void SaveUsers()
        {
            var json = JsonConvert.SerializeObject(Users);
            File.WriteAllText(path, json);
        }
        /// <summary>
        /// Add user with 200 credits by default.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>User added.</returns>
        public User AddUser(DiscordUser user)
        {
            var newUser = new User(user);
            Users.Add(user.Id, newUser);
            return newUser;
        }
        /// <summary>
        /// Gets the user with discord id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User in relation to id. Null if user does not exist. Throws ArguementNullException if id is not specified.</returns>
        public User GetUser(ulong id)
        {
            Users.TryGetValue(id, out var user);
            return user;
        }
        /// <summary>
        /// Check if user exists in Dictionary.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if user exists. False if user does not exist.</returns>
        public bool CheckForUser(ulong id)
        {
            return Users.ContainsKey(id);
        }
    }
}
