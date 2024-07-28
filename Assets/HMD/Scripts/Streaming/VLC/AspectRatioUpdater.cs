namespace HMD.Scripts.Streaming.VLC
{
    using UnityEngine.UI;
    using Util;

    public class AspectRatioUpdater
    {
        public readonly VlcDisplay Display;
        public readonly Frac Value;

        public AspectRatioUpdater(VlcDisplay display)
        {
            this.Display = display;
            Value = display.AspectRatio;
        }

        public void SyncSlider()
        {
            Display.controller.aspectRatioSlider.GetComponent<Slider>()
                .SetValueWithoutNotify((float)Value.ToDouble());
        }

        public void SyncText()
        {
            Display.controller.aspectRatioText.GetComponent<Text>().text =
                $"{Value.ToRatioText()} - {Value.ToDouble().ToString()}";
        }

        public void SyncAll()
        {
            SyncText();
            SyncSlider();
        }
    }
}
