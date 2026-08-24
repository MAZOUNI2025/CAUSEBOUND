using System;

namespace Causebound.Interaction
{
    [Serializable]
    public sealed class CauseEffectDefinition
    {
        public string causeId;
        public string effectId;
        public string relationshipType;
    }
}
