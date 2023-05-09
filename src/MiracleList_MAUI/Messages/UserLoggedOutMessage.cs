using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MiracleList_MAUI.Messages
{
    public class UserLoggedOutMessage : ValueChangedMessage<string>
    {
        public UserLoggedOutMessage(string user) : base(user)
        {
        }
    }
}
