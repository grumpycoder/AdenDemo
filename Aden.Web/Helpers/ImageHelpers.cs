using System.IO;
using System.Web;

namespace Aden.Web.Helpers
{
    public static class ImageHelpers
    {
        public static byte[] ConvertToByte(this HttpPostedFileBase file)
        {
            byte[] imageByte = null;
            file.InputStream.Position = 0;
            BinaryReader rdr = new BinaryReader(file.InputStream);
            imageByte = rdr.ReadBytes((int)file.ContentLength);
            return imageByte;
        }

        //public byte[] ConvertToByte(HttpPostedFileBase file)
        //{
        //    byte[] imageByte = null;
        //    file.InputStream.Position = 0;
        //    BinaryReader rdr = new BinaryReader(file.InputStream);
        //    imageByte = rdr.ReadBytes((int)file.ContentLength);
        //    return imageByte;
        //}
    }
}
