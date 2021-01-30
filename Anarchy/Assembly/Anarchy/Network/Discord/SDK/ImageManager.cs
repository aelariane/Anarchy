using System;

namespace Anarchy.Network.Discord.SDK
{
    public partial struct ImageHandle
    {
        static public ImageHandle User(Int64 id)
        {
            return User(id, 128);
        }

        static public ImageHandle User(Int64 id, UInt32 size)
        {
            return new ImageHandle
            {
                Type = ImageType.User,
                Id = id,
                Size = size,
            };
        }
    }

    public partial class ImageManager
    {
        public void Fetch(ImageHandle handle, FetchHandler callback)
        {
            Fetch(handle, false, callback);
        }

        public byte[] GetData(ImageHandle handle)
        {
            var dimensions = GetDimensions(handle);
            var data = new byte[dimensions.Width * dimensions.Height * 4];
            GetData(handle, data);
            return data;
        }

    }
}
