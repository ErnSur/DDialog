namespace Doublsb.Dialog
{
    public interface IActorManager
    {
        /// <summary>
        /// 
        /// </summary>
        string ActiveActorId { get; set; }

        void Emote(string actorId, string emote);
    }
}