using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace FuzzyLogic.TGMCProject.Core
{
    public class StreamCSVReader
    {
        private readonly TextReader _reader;
        private readonly bool _hasHeaders;
        private readonly Stream _inputStream;

        private bool _hitEol;
        private bool _hitEof;
        //private int _position = 0;
        //private char[] _buffer;

        public StreamCSVReader(Stream inputStream, bool hasHeaders=false)
        {
            _hasHeaders = hasHeaders;
            _inputStream = inputStream;
            _reader = new StreamReader(inputStream, true);

            _inputStream.Seek(0, SeekOrigin.Begin);

            if (_hasHeaders) ReadHeader();
        }

        private void ReadHeader()
        {
            if (!_hasHeaders) return;

            _inputStream.Seek(0, SeekOrigin.Begin);

            IList<String> headerList = new List<string>();

            while (HasChunkInRecord())
            {
                headerList.Add(ReadChunk<String>());
            }
        }

        public bool HasChunkInRecord()
        {
            return !_hitEol && !_hitEof;
        }

        public bool NextRecord()
        {
            if (_hitEof) return false;
            _hitEol = false;

            return true;
        }

        /**
         * Reads a chunk of data and tries to parse it as a specific type
         */
        public T ReadChunk<T>() where T : IConvertible
        {
            var readChunk = false;
            var st = new StringBuilder();

            while (!readChunk)
            {
                // Fill the buffer
                var c = _reader.Read();

                if (c < 0)
                {
                    _hitEof = true;
                    break;
                }

                switch ((char)c)
                {
                    case '\r':
                        break;
                    case '\n':
                    case '\0':
                        _hitEol = true;
                        readChunk = true;
                        break;
                    case '"':
                    case '\'':
                        break;
                    case ',':
                        readChunk = true;
                        break;
                    default:
                        st.Append((char)c);
                        break;
                }
            }

            var converter = TypeDescriptor.GetConverter(typeof (T));
            return st.Length > 0 ? (T) converter.ConvertFromString(st.ToString()) : default(T);
        }
    }
}
