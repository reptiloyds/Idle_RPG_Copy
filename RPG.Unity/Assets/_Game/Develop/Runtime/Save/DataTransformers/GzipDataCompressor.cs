using System;
using System.IO;
using System.IO.Compression;
using UnityEngine.Scripting;

namespace PleasantlyGames.RPG.Runtime.Save.DataTransformers
{
    public class GzipDataCompressor : DataTransformerDecorator
    {
        public GzipDataCompressor(IDataTransformer wrappedTransformer) : base(wrappedTransformer)
        {
        }

        public override string Transform(string data)
        {
            data = base.Transform(data);
            return Compress(data);
        }

        public override string Reverse(string data)
        {
            data = Decompress(data);
            return base.Reverse(data);
        }

        private string Compress(string uncompressedData)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(uncompressedData);
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionMode.Compress)) 
                gzip.Write(bytes, 0, bytes.Length);
            var compressedBytes = output.ToArray();
            var compressedString = Convert.ToBase64String(compressedBytes);
            return compressedString;
        }

        private string Decompress(string compressedData)
        {
            if (string.IsNullOrEmpty(compressedData)) return default;
            var compressedBytes = Convert.FromBase64String(compressedData);
            using var input = new MemoryStream(compressedBytes);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            gzip.CopyTo(output);
            var uncompressedBytes = output.ToArray();
            var uncompressedData = System.Text.Encoding.UTF8.GetString(uncompressedBytes);
            return uncompressedData;
        }
    }
}