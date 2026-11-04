using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ProcessAutomationMAUI.Messages
{
    public class ValueMessage : ValueChangedMessage<string>
    {
        public ValueMessage(string value) : base(value)
        {
        }
    }
}
