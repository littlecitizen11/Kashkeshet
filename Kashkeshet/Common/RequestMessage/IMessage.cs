namespace Common
{
    public interface IMessage
    {
        public User ClientUser { get; set; }
        public MessageType MessageType { get; set; }
    }
}
