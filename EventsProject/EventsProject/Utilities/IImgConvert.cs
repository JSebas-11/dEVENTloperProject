using EventsProject.Domain.Common;
using System.Windows.Media.Imaging;

namespace EventsProject.Utilities;

//Interface de conversion de imgs a binario y viceversa para interactuar con la DB
public interface IImgConvert {
    Result ValidImgPath(string path);
    Result ValidImgByte(byte[] binImg);
    Task<byte[]?> ImgPathToBinAsync(string path);
    BitmapImage? BinToImg(byte[] binImg);
}
