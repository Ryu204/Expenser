using System.ComponentModel;
using System.Diagnostics;
using Expenser.Utility;

/// <summary>
/// Provide methods to interact with user.
/// For eg. saving data, updating status,
/// creating wallet,...
/// </summary>
namespace Expenser.User
{
    /// <summary>
    /// Create and edit user's login information
    /// and navigate to wallet
    /// </summary>
    public class Account
    {
        public string Username { get; private set; } = string.Empty;
        public uint Value { get; private set; } = 0;

        private readonly HashSet<string> registerdUsers = new();

        public Account()
        {
            LoadUserListFromFile();
        }

        public bool SetUser(string username)
        {
            if (!registerdUsers.Contains(username))
            {
                IOStream.Output($"There is no user {username} in database.");
                return false;
            }
            string usernameBuffer = Username;
            Username = username;
            if (LoadUserFromData() == false)
            {
                IOStream.Output($"There is a problem reading data of user {username}.");
                Username = usernameBuffer;
                return false;
            }
            return true;
        }

        private bool LoadUserFromData()
        {
            try
            {
                string path = $"Users/{Username}.data";
                if (!File.Exists(path))
                    return false;
                using (StreamReader reader = File.OpenText(path))
                {
                    TypeConverter toUInt = TypeDescriptor.GetConverter(Value);
                    string? line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line) || !toUInt.IsValid(line))
                        return false;
                    Value = uint.Parse(line);
                    reader.Close();
                }
                return true;
            }
            catch(Exception e)
            {
                IOStream.Output($"An error occured when loading data of user {Username}:");
                Console.WriteLine(e.Message);
                return false;
            }
        }

        private void LoadUserListFromFile()
        {
            string path = @"Users/userlist.list";
            try
            {
                // If the list is gone somehow, create a new one from 
                // existing files inside folder
                if (!File.Exists(path))
                {
                    IOStream.Output("Unable to load users list. Recovering from local folder...");

                    string[] files = Directory.GetFiles("Users");
                    foreach (string file in files)
                    {
                        string name = string.Empty;
                        if (IOStream.TryGetFileName(Path.GetFileName(file), ref name))
                            registerdUsers.Add(name);
                    }

                    IOStream.Output($"{registerdUsers.Count} account(s) were added to new users list.");
                    using StreamWriter writter = File.CreateText(path);
                    {
                        foreach (string name in registerdUsers)
                            writter.WriteLine(name);
                    }
                }
                else
                {
                    using StreamReader reader = File.OpenText(path);
                    string? line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                        registerdUsers.Add(line);
                }
            }
            catch (Exception ex)
            {
                IOStream.Output("An error occured while reading users list:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
