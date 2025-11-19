using EventsProject.Domain.Common;
using EventsProject.Utilities;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace EventsProject.Infrastructure.Utilities;

public class ImgConverter : IImgConvert {
    //------------------------INITIALIZATION------------------------
    public ImgConverter() { }

    //------------------------METHODS------------------------
    public Result ValidImgPath(string path) {
        var accError = new StringBuilder("Image's errors:\n");

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            accError.AppendLine($"- File ({path}) was not found");
        
        string extension = Path.GetExtension(path).ToLower();
        if (!Config.validExtensions.Contains(extension))
            accError.AppendLine($"{extension} extension isn't allowed");

        long fileSize = new FileInfo(path).Length;
        if (fileSize > Config.maxSizeBytes)
            accError.AppendLine($"File size is not allowed. Maximum size is: {Config.maxSizeBytes}MBS");

        if (accError.ToString() == "Image's errors:\n") return Result.Ok("File exists, extension and size are corrects");
        else return Result.Fail(accError.ToString());
    }

    public Result ValidImgByte(byte[] binImg)
    {
        return Result.Fail("Error in: ImgConverter - ValidImgPath", "Aca iria el tipo de error donde se identifiquen lo tamaños maxs etc");
    }

    public async Task<byte[]?> ImgPathToBinAsync(string path) {
        try { return await File.ReadAllBytesAsync(path); }
        catch (Exception) { return null; }
    }
    
    public BitmapImage? BinToImg(byte[] binImg) {
        try {
            using (var ms = new MemoryStream(binImg)) {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; 
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();

                return image;
            }
        }
        catch (Exception) { return null; }
    }

}
