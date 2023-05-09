using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MiracleList_MAUI.Messages
{
    public class UserLoggedInMessage : ValueChangedMessage<string>
    {
        public UserLoggedInMessage(string user) : base(user)
        {
        }
    }
}
