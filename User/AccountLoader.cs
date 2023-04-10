using Expenser.Utility;
using System.Transactions;

namespace Expenser.User
{
    /// <summary>
    /// Deal with account file streaming
    /// </summary>
    public static class AccountLoader
    {
        public static bool TryLoadUserFromFile(string username, ref Account account)
        {
            Exception cantRead = new($"The file containing your data may be broken and cannot be read.");

            string password = "123456";
            List<Transaction> transactions = new();
            List<Wallet> wallets = new();
            try
            {
                string path = $"Users/{username}{GrammarChecker.UserFileSuffix}";
                if (!File.Exists(path))
                {
                    IOStream.OutputOther($"Cannot find user {username}'s data.");
                    return false;
                }
                using (StreamReader reader = File.OpenText(path))
                {
                    while (true)
                    {
                        string? identifier = reader.ReadLine();
                        // Parse by identifier
                        if (identifier == "Password")
                        {
                            string? line = reader.ReadLine();
                            if (string.IsNullOrWhiteSpace(line) || !Account.IsPassword(line))
                                throw cantRead;
                            password = line;
                        }
                        else if (identifier == "Transaction")
                        {
                            string? line = reader.ReadLine();
                            if (string.IsNullOrEmpty(line) || !uint.TryParse(line, out uint count))
                                throw cantRead;
                            for (int i = 0; i < count; ++i)
                            {
                                Transaction tran = new();
                                if (Transaction.Parser.TryParseFromStream(reader, ref tran))
                                    transactions.Add(tran);
                                else
                                    throw cantRead;
                            }
                        }
                        else if (identifier == "Wallet")
                        {
                            string? line = reader.ReadLine();
                            if (string.IsNullOrEmpty(line) || !uint.TryParse(line, out uint count))
                                throw cantRead;
                            for (int i = 0; i < count; ++i)
                            {
                                if (Wallet.Loader.TryParseFromStream(reader, out Wallet wallet))
                                    wallets.Add(wallet);
                                else
                                    throw cantRead;
                            }
                        }
                        else if (identifier == "End")
                            break;
                    }
                    reader.Close();
                }
                transactions.Sort((Transaction a, Transaction b) => a.Time.CompareTo(b.Time));
                account = new(username, password, transactions, wallets);
                return true;
            }
            catch (Exception e)
            {
                if (e == cantRead)
                {
                    IOStream.OutputOther(e.Message);
                }
                else
                {
                    IOStream.OutputOther($"An error occured when loading data of user {username}:");
                    Console.WriteLine(e.Message);
                }
                return false;
            }
        }

        public static bool SaveUserToFile(Account user)
        {
            string path = $"Users/{user.Username}{GrammarChecker.UserFileSuffix}";

            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.WriteLine($"Password\n{user.Password}");
                    writer.WriteLine($"Transaction\n{user.Transactions.Count}");
                    foreach (Transaction tran in user.Transactions)
                        Transaction.Parser.WriteToStream(writer, tran);
                    writer.WriteLine($"Wallet\n{user.Wallets.Count}");
                    foreach (var pair in user.Wallets)
                        Wallet.Loader.WriteToStream(writer, pair.Value);
                    writer.WriteLine("End");
                    return true;
                }
            }
            catch (Exception)
            {
                IOStream.OutputOther($"Cannot save user {user.Username}'s data.");
                return false;
            }
        }

        public static HashSet<string> LoadUserListFromFile()
        {
            string path = @"Users/userlist.list";
            HashSet<string> list = new();

            try
            {
                // If the list is gone somehow, create a new one from 
                // existing files inside folder
                if (!File.Exists(path))
                {
                    IOStream.OutputOther("Unable to load users list. Recovering from local folder...");

                    string[] filenames = Directory.GetFiles("Users");
                    foreach (string file in filenames)
                    {
                        if (GrammarChecker.TryGetFileName(Path.GetFileName(file), out string name))
                            list.Add(name);
                    }

                    IOStream.OutputOther($"{list.Count} account(s) were added to new users list.");
                    using StreamWriter writter = File.CreateText(path);
                    {
                        foreach (string name in list)
                            writter.WriteLine(name);
                    }
                }
                else
                {
                    using StreamReader reader = File.OpenText(path);
                    string? line = string.Empty;
                    while ((line = reader.ReadLine()) != null)
                        if (Account.IsUsername(line))
                            list.Add(line);
                }
            }
            catch (Exception ex)
            {
                IOStream.OutputOther("An error occured while reading users list:");
                Console.WriteLine(ex.Message);
            }

            return list;
        }
    }
}