using KG;
using UnityEngine.UI;

namespace UI
{
    public class MinimapView : UIAutoComponent<RawImage>
    {
        public RawImage minimap => instance;
    }
}