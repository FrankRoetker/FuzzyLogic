﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace FuzzyLogic.TGMCProject.Core
{
    public class StreamCSVReader
    {
        private TextReader _reader;
        private readonly bool _hasHeaders;
        private readonly Stream _inputStream;
        private readonly int _numColumns;

        private bool _hitEol;
        private bool _hitEof;
        //private int _position = 0;
        //private char[] _buffer;

        public int NumberColumns
        {
            get { return this._numColumns; }
        }

        public StreamCSVReader(Stream inputStream, bool hasHeaders=false)
        {
            _hasHeaders = hasHeaders;
            _inputStream = inputStream;
            _reader = new StreamReader(inputStream, true);

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

        /**
         * Reads a chunk of data and tries to parse it as a specific type
         */
        public T ReadChunk<T>() where T : IConvertible
        {
            var readChunk = false;
            var st = new StringBuilder();

            var inQuote = false;
            var quoteCharSeen = '\0';
            var skipNext = false;

            while (!readChunk)
            {
                // Fill the buffer
                var c = _reader.Read();

                if (c < 0)
                {
                    _hitEof = true;
                    break;
                }

                if ((inQuote && (char) c != quoteCharSeen) || skipNext)
                {
                    skipNext = false;
                    st.Append((char) c);
                    continue;
                }

                switch ((char)c)
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
                        quoteCharSeen = (char) c;
                        inQuote = !inQuote;
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

        private int GetNumColumns()
        {
            int toReturn = 0;

            if (this.NextRecord())
            {
                while (this.HasChunkInRecord())
                {
                    var chunk = this.ReadChunk<String>();
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