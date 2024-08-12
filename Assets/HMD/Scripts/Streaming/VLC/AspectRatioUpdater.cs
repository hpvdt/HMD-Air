namespace HMD.Scripts.Streaming.VLC
{
    using UnityEngine.UI;
    using Util;

    public class AspectRatioUpdater
    {
        public readonly VlcScreen Screen;
        public readonly Frac Value;

        public AspectRatioUpdater(VlcScreen screen)
        {
            this.Screen = screen;
            Value = screen.AspectRatio;
        }

        public void SyncSlider()
        {
            Screen.controller.aspectRatioBar.GetComponent<Slider>()
                .SetValueWithoutNotify((float)Value.ToLn());
        }

        public void SyncText()
        {
            Screen.controller.aspectRatioText.GetComponent<Text>().text =
                $"{Value.ToRatioText()}\n{Value.ToDouble().ToString()}";
        }

        public void SyncAll()
        {
            SyncText();
            SyncSlider();
        }
    }
}
