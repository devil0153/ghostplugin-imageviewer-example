using Ghost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageViewer
{
    public class ImageExtension : IExtension
    {
        public HubBase Hub { get; private set; }

        public void Initialize(IExtensionConfig config)
        {
            this.Hub = new ImageHub();
        }

        public void OnConnected()
        {

        }

        public void OnDisconnect()
        {

        }

        public void Dispose()
        {
        }
    }
}
