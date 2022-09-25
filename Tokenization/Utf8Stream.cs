using System.Collections;
using System.Text;

namespace Uncertainty.Tokenization;

/// <summary>
/// Utility class to convert a Stream into UTF8 characters in chunks.
/// </summary>
public class BufferedUTF8Converter : IEnumerable<char>
{
    Stream s;
    int chunkSize;

    public BufferedUTF8Converter(Stream stream, int chunkSizeBytes = 1024)
    {
        s = stream;
        chunkSize = chunkSizeBytes;
    }

    public IEnumerator<char> GetEnumerator()
    {
        return new Utf8ConverterEnumerator(s, chunkSize);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    internal class Utf8ConverterEnumerator : IEnumerator<char>
    {
        private Stream baseStream;

        private Decoder decoder;

        private List<char> converted;
        private int convertedIndex = 0;

        private int chunkSize;

        public Utf8ConverterEnumerator(Stream stream, int chunkSizeBytes = 1024)
        {
            baseStream = stream;
            decoder = Encoding.UTF8.GetDecoder();
            converted = new List<char>(chunkSizeBytes);
            chunkSize = chunkSizeBytes;
        }

        public char Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            baseStream.Dispose();
        }

        public bool MoveNext()
        {
            if (convertedIndex >= converted.Count)
            {
                var data = new byte[chunkSize];
                var chars = new char[chunkSize];
                int length = baseStream.Read(data, 0, chunkSize);
                if (length == 0)
                {
                    return false;
                }
                int size = decoder.GetChars(new Span<byte>(data, 0, length), chars, false);
                if (size == 0)
                {
                    return false;
                }
                converted.AddRange(new ArraySegment<char>(chars, 0, size));
                convertedIndex = 0;
            }

            Current = converted[convertedIndex++];
            return true;
        }

        public void Reset()
        {
            baseStream.Seek(0, SeekOrigin.Begin);
        }
    }
}

