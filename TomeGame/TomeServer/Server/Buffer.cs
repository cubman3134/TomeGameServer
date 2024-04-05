using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TomeServer.Server
{
    public class Buffer
    {
        private byte[] _data;

        private long _size;

        private long _offset;

        public bool IsEmpty
        {
            get
            {
                if (_data != null)
                {
                    return _size == 0;
                }

                return true;
            }
        }

        public byte[] Data => _data;

        public long Capacity => _data.Length;

        public long Size => _size;

        public long Offset => _offset;

        public byte this[long index] => _data[index];

        public Buffer()
        {
            _data = new byte[0];
            _size = 0L;
            _offset = 0L;
        }

        public Buffer(long capacity)
        {
            _data = new byte[capacity];
            _size = 0L;
            _offset = 0L;
        }

        public Buffer(byte[] data)
        {
            _data = data;
            _size = data.Length;
            _offset = 0L;
        }

        public override string ToString()
        {
            return ExtractString(0L, _size);
        }

        public void Clear()
        {
            _size = 0L;
            _offset = 0L;
        }

        public string ExtractString(long offset, long size)
        {
            if (offset + size > Size)
            {
                throw new ArgumentException("Invalid offset & size!", "offset");
            }

            return Encoding.UTF8.GetString(_data, (int)offset, (int)size);
        }

        public void Remove(long offset, long size)
        {
            if (offset + size > Size)
            {
                throw new ArgumentException("Invalid offset & size!", "offset");
            }

            Array.Copy(_data, offset + size, _data, offset, _size - size - offset);
            _size -= size;
            if (_offset >= offset + size)
            {
                _offset -= size;
            }
            else if (_offset >= offset)
            {
                _offset -= _offset - offset;
                if (_offset > Size)
                {
                    _offset = Size;
                }
            }
        }

        public void Reserve(long capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentException("Invalid reserve capacity!", "capacity");
            }

            if (capacity > Capacity)
            {
                byte[] array = new byte[Math.Max(capacity, 2 * Capacity)];
                Array.Copy(_data, 0L, array, 0L, _size);
                _data = array;
            }
        }

        public void Resize(long size)
        {
            Reserve(size);
            _size = size;
            if (_offset > _size)
            {
                _offset = _size;
            }
        }

        public void Shift(long offset)
        {
            _offset += offset;
        }

        public void Unshift(long offset)
        {
            _offset -= offset;
        }

        public long Append(byte value)
        {
            Reserve(_size + 1);
            _data[_size] = value;
            _size++;
            return 1L;
        }

        public long Append(byte[] buffer)
        {
            Reserve(_size + buffer.Length);
            Array.Copy(buffer, 0L, _data, _size, buffer.Length);
            _size += buffer.Length;
            return buffer.Length;
        }

        public long Append(byte[] buffer, long offset, long size)
        {
            Reserve(_size + size);
            Array.Copy(buffer, offset, _data, _size, size);
            _size += size;
            return size;
        }

        /*public long Append(byte[] buffer)
        {
            Reserve(_size + buffer.Length);
            buffer.CopyTo(new Span<byte>(_data, (int)_size, buffer.Length));
            _size += buffer.Length;
            return buffer.Length;
        }*/

        public long Append(Buffer buffer)
        {
            return Append(buffer.Data);
        }

        public long Append(string text)
        {
            int maxByteCount = Encoding.UTF8.GetMaxByteCount(text.Length);
            Reserve(_size + maxByteCount);
            long num = Encoding.UTF8.GetBytes(text, 0, text.Length, _data, (int)_size);
            _size += num;
            return num;
        }

        /*public long Append(char[] text)
        {
            int maxByteCount = Encoding.UTF8.GetMaxByteCount(text.Length);
            Reserve(_size + maxByteCount);
            long num = Encoding.UTF8.GetBytes(text, _data, (int)_size, maxByteCount));
            _size += num;
            return num;
        }*/
    }
}
