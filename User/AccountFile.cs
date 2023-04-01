using Expenser.Utility;
using System.ComponentModel;

namespace Expenser.User
{
    /// <summary>
    /// Account class file and authorise operations
    /// </summary>
    public partial class Account
    {
        private void LoadUserListFromFile()
        {
            string path = @"Users/userlist.list";

            try
            {
                // If the list is gone somehow, create a new one from 
                // existing files inside folder
                if (!File.Exists(path))
                {
                    IOStream.OutputOther("Unable to load users list. Recovering from local folder...");

                    string[] files = Directory.GetFiles("Users");
                    foreach (string file in files)
                    {
                        string name = string.Empty;
                        if (IOStream.TryGetFileName(Path.GetFileName(file), ref name))
                            registeredUsers.Add(name);
                    }

                    IOStream.OutputOther($"{registeredUsers.Count} account(s) were added to new users list.");
                    using StreamWriter writter = File.CreateText(path);
                    {
                        foreach (string name in registeredUsers)
                            writter.WriteLine(name);
                    }
                }
                else
                {
                    using StreamReader reader = File.OpenText(path);
                    string? line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                        registeredUsers.Add(line);
                }
            }
            catch (Exception ex)
            {
                IOStream.OutputOther("An error occured while reading users list:");
                Console.WriteLine(ex.Message);
            }
        }

        private bool Authorise()
        {
            string? pw = null;
            try
            {
                while (pw != Password)
                {
                    IOStream.Output("Enter your password: ", false);
                    pw = Console.ReadLine();
                    if (pw == Password)
                        return true;
                    else
                    {
                        IOStream.OutputError("Wrong password. Try again? [y/n]");
                        string? confirm = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(confirm))
                            break;
                        else if (confirm.ToLower() == "y")
                            continue;
                        else
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                IOStream.OutputOther(ex.Message);
            }
            return false;
        }

        public bool SetUser(string username)
        {
            if (!registeredUsers.Contains(username))
            {
                IOStream.OutputError($"There is no username {username} in database.");
                return false;
            }
            string usernameBuffer = Username;
            Username = username;
            if (!LoadUserFromData())
            {
                IOStream.OutputOther($"There is a problem reading data of user {username}.");
                Username = usernameBuffer;
                return false;
            }
            if (Authorise())
                return true;
            else
            {
                Username = usernameBuffer;
                return false;
            }
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
                    if (string.IsNullOrWhiteSpace(line))
                        return false;
                    Password = line;

                    line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line) || !toUInt.IsValid(line))
                        return false;
                    Value = uint.Parse(line);

                    reader.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                IOStream.OutputOther($"An error occured when loading data of user {Username}:");
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public bool SaveUserData()
        {
            string path = $"Users/{Username}.data";

            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.WriteLine(Password);
                    writer.WriteLine(Value);
                }
            }
            catch(Exception)
            {
                IOStream.OutputOther($"Cannot save user {Username}'s data.");
                return false;
            }
            return true;
        }
    }
}
