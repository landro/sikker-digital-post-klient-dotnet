using System.IO;

namespace Difi.SikkerDigitalPost.Klient.Extensions
{
    public static class StreamExtensions
    {
        public static Stream CopyTo(this Stream input, Stream output)
        {
            var buffer = new byte[32768];
            
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }

            return output;
        }
    }
}
