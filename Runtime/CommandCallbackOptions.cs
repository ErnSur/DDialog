namespace QuickEye.PeeDialog
{
    public struct CommandCallbackOptions
    {
        // TODO: `BeginAfter` and `EndAfter` methods
        // public static CommandCallbackOptions BeginBefore(Type type) => new CommandCallbackOptions
        // {
        // };
        
        // TODO: maybe registrators should register the callbacks with IDs.
        // then other registrators can register callbacks to be executed before or after the callbacks with the specified IDs.
        public float BeginIndex { get; set; }
        public float EndIndex { get; set; }
        public CommandCallbackOptions(float beginIndex = 0, float endIndex = 0)
        {
            BeginIndex = beginIndex;
            EndIndex = endIndex;
        }
    }
}