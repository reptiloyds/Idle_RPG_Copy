using PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.Model;
using PleasantlyGames.RPG.Runtime.Core.UI.Base.Window;
using UnityEngine;

namespace PleasantlyGames.RPG.Runtime.Core.Phone.Gallery.View
{
    public class PhotoViewerWindow : BaseWindow
    {
        [SerializeField] private PhotoView _photoView;

        public void Setup(Photo photo) => 
            _photoView.Setup(photo);

        public override void Close()
        {
            base.Close();
            _photoView.Clear();
        }
    }
}