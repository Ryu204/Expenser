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
                while (true)
                {
                    IOStream.Output("Enter your password: ", false);
                    pw = Console.ReadLine();
                    if (pw == password)
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
                    while (true)
                    {
                        string? identifier = reader.ReadLine();
                        if (string.IsNullOrEmpty(identifier))
                            return false;
                        bool ended = false;
                        switch (identifier)
                        {
                            case "Password":
                                {
                                    string? line = reader.ReadLine();
                                    if (string.IsNullOrWhiteSpace(line))
                                        return false;
                                    password = line;
                                }
                                break;
                            case "Value":
                                {
                                    string? line = reader.ReadLine();
                                    if (string.IsNullOrEmpty(line) || !uint.TryParse(line, out uint tempValue))
                                        return false;
                                    Value = tempValue;
                                }
                                break;
                            case "Transaction":
                                {
                                    string? line = reader.ReadLine();
                                    if (string.IsNullOrEmpty(line) || !uint.TryParse(line, out uint count))
                                        return false;
                                    for (int i = 0; i < count; ++i)
                                    {
                                        Transaction tran = new();
                                        if (Transaction.ReadFromStream(reader, ref tran))
                                            transactions.Add(tran);
                                        else
                                        {
                                            transactions.Clear();
                                            return false;
                                        }
                                    }
                                }
                                break;
                            case "End":
                                ended = true;
                                break;
                            default:
                                return false;
                        }
                        if (ended)
                            break;
                    }

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

        public bool SaveUsersData()
        {
            string path = $"Users/{Username}.data";

            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.WriteLine($"Password\n{password}");
                    writer.WriteLine($"Value\n{Value}");
                    writer.WriteLine($"Transaction\n{ transactions.Count}");
                    foreach (Transaction tran in transactions)
                        tran.WriteToStream(writer);
                    writer.WriteLine("End");
                }
                using (StreamWriter writer = File.CreateText("Users/userlist.list"))
                {
                    foreach (string name in registeredUsers)
                        writer.WriteLine(name);
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
