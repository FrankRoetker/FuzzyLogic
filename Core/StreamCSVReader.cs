using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace FuzzyLogic.TGMCProject.Core
{
    public class StreamCSVReader
    {
        private StreamReader _reader;
        private readonly bool _hasHeaders;
        private readonly Stream _inputStream;
        private readonly int _numColumns;

        private readonly StringBuilder _builder;

        private bool _hitEol;
        private bool _hitEof;
        private bool _enableQuotes;
        //private int _position = 0;
        //private char[] _buffer;

        public int NumberColumns
        {
            get { return _numColumns; }
        }

        public StreamCSVReader(Stream inputStream, bool hasHeaders=false, bool enableQuotes=true)
        {
            _hasHeaders = hasHeaders;
            _enableQuotes = enableQuotes;

            _inputStream = inputStream;
            _reader = new StreamReader(inputStream, true);
            _builder = new StringBuilder(20);

            _inputStream.Seek(0, SeekOrigin.Begin);

            if (_hasHeaders) ReadHeader();

            _numColumns = GetNumColumns();
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

        // Reads a string as a chunk
        public String ReadChunkString()
        {
            var readChunk = false;

            _builder.Clear();

            var inQuote = false;
            var quoteCharSeen = 0;
            var skipNext = false;

            while (!readChunk)
            {
                if (_reader.EndOfStream) 
                {
                    _hitEof = true;
                    break;
                }

                var c = _reader.Read();

                if ((_enableQuotes && (inQuote && c != quoteCharSeen)) || skipNext)
                {
                    skipNext = false;
                    _builder.Append((char)c);
                    continue;
                }

                switch (c)
                {
                    case '\r':
                        break;
                    case '\\':
                        skipNext = true;
                        break;
                    case '\n':
                    case '\0':
                        _hitEol = true;
                        readChunk = true;
                        break;
                    case '"':
                    case '\'':
                        if (_enableQuotes)
                        {
                            quoteCharSeen = c;
                            inQuote = !inQuote;
                        }
                        else
                        {
                            _builder.Append((char) c);
                        }
                        break;
                    case ',':
                        readChunk = true;
                        break;
                    default:
                        _builder.Append((char)c);
                        break;
                }
            }

            return _builder.ToString();
        }

        public int ReadChunkInt()
        {
            var st = ReadChunkString();

            int result;

            int.TryParse(st, out result);
         
            return result;
        }

        public float ReadChunkFloat()
        {
            var st = ReadChunkString();

            float result;

            float.TryParse(st, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result);

            return result;
        }

        public double ReadChunkDouble()
        {
            var st = ReadChunkString();

            float result;

            float.TryParse(st, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result);

            return result;
        }

        public bool ReadChunkBoolean()
        {
            var st = ReadChunkString();

            bool result;

            bool.TryParse(st, out result);

            return result;
        }

        /**
         * Reads a chunk of data and tries to parse it as a specific type
         */
        public T ReadChunk<T>() where T : IConvertible
        {
            var st = ReadChunkString();

            var converter = TypeDescriptor.GetConverter(typeof (T));
            return st.Length > 0 ? (T) converter.ConvertFromString(st) : default(T);
        }

        private int GetNumColumns()
        {
            var toReturn = 0;

            if (NextRecord())
            {
                while (HasChunkInRecord())
                {
                    ReadChunk<String>();
                    toReturn++;
                }
            }

            ResetReader();

            Console.Out.WriteLine("Got {0} columns", toReturn);

            return toReturn;
        }

        public void ResetReader()
        {
            _inputStream.Seek(0, SeekOrigin.Begin);
            _hitEof = false;
            _hitEol = false;

            _reader = new StreamReader(_inputStream, true);

            if (_hasHeaders) ReadHeader();
        }
    }
}