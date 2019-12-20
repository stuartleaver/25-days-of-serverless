using System;

namespace Christmas.GiftWrappingAnalyser.Entities
{
    public class GiftDeliveryResult
    {
        public bool IsBoxed { get; set; }

        public bool IsGiftWrapped { get; set; }

        public bool HasRibbon { get; set; }

        public bool IsPresent { get; set; }

        public bool IsDelivered { get; set; }

        public GiftDeliveryResult(bool isBoxed, bool isGiftWrapped, bool hasRibbon, bool isPresent)
        {
            IsBoxed = isBoxed;

            IsGiftWrapped = isGiftWrapped;

            HasRibbon = hasRibbon;

            IsPresent = isPresent;

            IsDelivered = isBoxed && isGiftWrapped && hasRibbon && isPresent;
        }
    }
}