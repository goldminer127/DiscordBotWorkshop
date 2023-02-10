using DSharpPlus;
using DSharpPlus.Entities;
using System.Globalization;

namespace DiscordBotWorkshop.Database
{
    public class User
    {
        public string UserName { get; private set; }
        public string Avatar { get; private set; }
        public ulong Id { get; private set; }
        /// <summary>
        /// User credits for games.
        /// </summary>
        public int Credits { get; private set; } = 200;
        public string JoinDate { get; private set; } = DateTime.Now.ToString(new CultureInfo("en-us"));
        public User(DiscordUser userInfo)
        {
            UserName = userInfo.Username;
            Avatar = userInfo.AvatarUrl;
            Id = userInfo.Id;
        }
        /// <summary>
        /// Adds currency to user.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>User that is being modified.</returns>
        public User AddCurrency(int amount)
        {
            Credits += amount;
            return this;
        }
        /// <summary>
        /// Removes currency from user.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>User that is being modified.</returns>
        public User RemoveCurrency(int amount)
        {
            Credits -= amount;
            return this;
        }
    }
}
