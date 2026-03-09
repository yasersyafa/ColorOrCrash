using ColorOrCrash.Features.Achievement.Models;
using NocturneThree.EventSystem;

namespace ColorOrCrash.Features.Achievement.Events
{
    /// <summary>
    /// Event definition for ProgressUpdateEvent.
    /// </summary>
    public struct ProgressUpdateEvent : IGameEvent
    {
        public MissionType Type;
        public int Amount;
        public string TargetId;

        /// <summary>
        /// Constructor of event
        /// </summary>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        /// <param name="targetId"></param>
        public ProgressUpdateEvent(MissionType type, int amount, string targetId = "")
        {
            Type = type;
            Amount = amount;
            TargetId = targetId;
        }
    }    
}
