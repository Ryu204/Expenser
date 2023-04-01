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
    public partial class Account
    {
        public string Username { get; private set; } = string.Empty;
        private string Password { get; set; } = string.Empty;
        public uint Value { get; private set; } = 0;

        private readonly HashSet<string> registeredUsers = new();

        public Account()
        {
            LoadUserListFromFile();
        }

        public bool Add(uint increment)
        {
            uint newValue = 0;
            try
            {
                checked
                {
                    newValue = Value + increment;
                }
            }
            catch (OverflowException)
            {
                IOStream.OutputError($"Addition value {increment} is too large.");
                return false;
            }

            Value = newValue;
            return true;
        }
    }
}
