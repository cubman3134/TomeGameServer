using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TomeServer.Server.NetCoreServer;

namespace TomeServer.Server
{
    public class TcpSession : IDisposable
    {
        private bool _receiving;

        private Buffer _receiveBuffer;

        private SocketAsyncEventArgs _receiveEventArg;

        private readonly object _sendLock = new object();

        private bool _sending;

        private Buffer _sendBufferMain;

        private Buffer _sendBufferFlush;

        private SocketAsyncEventArgs _sendEventArg;

        private long _sendBufferFlushOffset;

        public Guid Id { get; }

        public TcpServer Server { get; }

        public Socket Socket { get; private set; }

        public long BytesPending { get; private set; }

        public long BytesSending { get; private set; }

        public long BytesSent { get; private set; }

        public long BytesReceived { get; private set; }

        public int OptionReceiveBufferLimit { get; set; }

        public int OptionReceiveBufferSize { get; set; } = 8192;


        public int OptionSendBufferLimit { get; set; }

        public int OptionSendBufferSize { get; set; } = 8192;


        public bool IsConnected { get; private set; }

        public bool IsDisposed { get; private set; }

        public bool IsSocketDisposed { get; private set; } = true;


        public TcpSession(TcpServer server)
        {
            Id = Guid.NewGuid();
            Server = server;
            OptionReceiveBufferSize = server.OptionReceiveBufferSize;
            OptionSendBufferSize = server.OptionSendBufferSize;
        }

        internal void Connect(Socket socket)
        {
            Socket = socket;
            IsSocketDisposed = false;
            _receiveBuffer = new Buffer();
            _sendBufferMain = new Buffer();
            _sendBufferFlush = new Buffer();
            _receiveEventArg = new SocketAsyncEventArgs();
            _receiveEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnAsyncCompleted);
            _sendEventArg = new SocketAsyncEventArgs();
            _sendEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(OnAsyncCompleted);
            if (Server.OptionKeepAlive)
            {
                Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, optionValue: true);
            }

            /*if (Server.OptionTcpKeepAliveTime >= 0)
            {
                Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, Server.OptionTcpKeepAliveTime);
            }*/

            if (Server.OptionTcpKeepAliveInterval >= 0)
            {
                Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.BlockSource, Server.OptionTcpKeepAliveInterval);
            }

            if (Server.OptionTcpKeepAliveRetryCount >= 0)
            {
                Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.DontRoute, Server.OptionTcpKeepAliveRetryCount);
            }

            if (Server.OptionNoDelay)
            {
                Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, optionValue: true);
            }

            _receiveBuffer.Reserve(OptionReceiveBufferSize);
            _sendBufferMain.Reserve(OptionSendBufferSize);
            _sendBufferFlush.Reserve(OptionSendBufferSize);
            BytesPending = 0L;
            BytesSending = 0L;
            BytesSent = 0L;
            BytesReceived = 0L;
            OnConnecting();
            Server.OnConnectingInternal(this);
            IsConnected = true;
            TryReceive();
            if (!IsSocketDisposed)
            {
                OnConnected();
                Server.OnConnectedInternal(this);
                if (_sendBufferMain.IsEmpty)
                {
                    OnEmpty();
                }
            }
        }

        public virtual bool Disconnect()
        {
            if (!IsConnected)
            {
                return false;
            }

            _receiveEventArg.Completed -= new EventHandler<SocketAsyncEventArgs>(OnAsyncCompleted);
            _sendEventArg.Completed -= new EventHandler<SocketAsyncEventArgs>(OnAsyncCompleted);
            OnDisconnecting();
            Server.OnDisconnectingInternal(this);
            try
            {
                try
                {
                    Socket.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException)
                {
                }

                Socket.Close();
                Socket.Dispose();
                _receiveEventArg.Dispose();
                _sendEventArg.Dispose();
                IsSocketDisposed = true;
            }
            catch (ObjectDisposedException)
            {
            }

            IsConnected = false;
            _receiving = false;
            _sending = false;
            ClearBuffers();
            OnDisconnected();
            Server.OnDisconnectedInternal(this);
            Server.UnregisterSession(Id);
            return true;
        }

        public virtual long Send(byte[] buffer)
        {
            return Send(buffer);
        }

        /*public virtual long Send(byte[] buffer, long offset, long size)
        {
            return Send(buffer.AsSpan((int)offset, (int)size));
        }

        public virtual long Send(ReadOnlySpan<byte> buffer)
        {
            if (!IsConnected)
            {
                return 0L;
            }

            if (buffer.IsEmpty)
            {
                return 0L;
            }

            SocketError errorCode;
            long num = Socket.Send(buffer, SocketFlags.None, out errorCode);
            if (num > 0)
            {
                BytesSent += num;
                Interlocked.Add(ref Server._bytesSent, num);
                OnSent(num, BytesPending + BytesSending);
            }

            if (errorCode != 0)
            {
                SendError(errorCode);
                Disconnect();
            }

            return num;
        }*/

        public virtual long Send(string text)
        {
            return Send(Encoding.UTF8.GetBytes(text));
        }

        /*public virtual long Send(ReadOnlySpan<char> text)
        {
            return Send(Encoding.UTF8.GetBytes(text.ToArray()));
        }*/

        public virtual bool SendAsync(byte[] buffer)
        {
            return SendAsync(buffer);
        }

        /*public virtual bool SendAsync(byte[] buffer, long offset, long size)
        {
            return SendAsync(buffer.AsSpan((int)offset, (int)size));
        }

        public virtual bool SendAsync(ReadOnlySpan<byte> buffer)
        {
            if (!IsConnected)
            {
                return false;
            }

            if (buffer.IsEmpty)
            {
                return true;
            }

            lock (_sendLock)
            {
                if (_sendBufferMain.Size + buffer.Length > OptionSendBufferLimit && OptionSendBufferLimit > 0)
                {
                    SendError(SocketError.NoBufferSpaceAvailable);
                    return false;
                }

                _sendBufferMain.Append(buffer);
                BytesPending = _sendBufferMain.Size;
                if (_sending)
                {
                    return true;
                }

                _sending = true;
                TrySend();
            }

            return true;
        }*/

        public virtual bool SendAsync(string text)
        {
            return SendAsync(Encoding.UTF8.GetBytes(text));
        }

        /*public virtual bool SendAsync(ReadOnlySpan<char> text)
        {
            return SendAsync(Encoding.UTF8.GetBytes(text.ToArray()));
        }*/

        public virtual long Receive(byte[] buffer)
        {
            return Receive(buffer, 0L, buffer.Length);
        }

        public virtual long Receive(byte[] buffer, long offset, long size)
        {
            if (!IsConnected)
            {
                return 0L;
            }

            if (size == 0L)
            {
                return 0L;
            }

            SocketError errorCode;
            long num = Socket.Receive(buffer, (int)offset, (int)size, SocketFlags.None, out errorCode);
            if (num > 0)
            {
                BytesReceived += num;
                Interlocked.Add(ref Server._bytesReceived, num);
                OnReceived(buffer, 0L, num);
            }

            if (errorCode != 0)
            {
                SendError(errorCode);
                Disconnect();
            }

            return num;
        }

        public virtual string Receive(long size)
        {
            byte[] array = new byte[size];
            long num = Receive(array);
            return Encoding.UTF8.GetString(array, 0, (int)num);
        }

        public virtual void ReceiveAsync()
        {
            TryReceive();
        }

        private void TryReceive()
        {
            if (_receiving || !IsConnected)
            {
                return;
            }

            bool flag = true;
            while (flag)
            {
                flag = false;
                try
                {
                    _receiving = true;
                    _receiveEventArg.SetBuffer(_receiveBuffer.Data, 0, (int)_receiveBuffer.Capacity);
                    if (!Socket.ReceiveAsync(_receiveEventArg))
                    {
                        flag = ProcessReceive(_receiveEventArg);
                    }
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        private void TrySend()
        {
            if (!IsConnected)
            {
                return;
            }

            bool flag = false;
            bool flag2 = true;
            while (flag2)
            {
                flag2 = false;
                lock (_sendLock)
                {
                    if (!_sendBufferFlush.IsEmpty)
                    {
                        return;
                    }

                    _sendBufferFlush = Interlocked.Exchange(ref _sendBufferMain, _sendBufferFlush);
                    _sendBufferFlushOffset = 0L;
                    BytesPending = 0L;
                    BytesSending += _sendBufferFlush.Size;
                    if (_sendBufferFlush.IsEmpty)
                    {
                        flag = true;
                        _sending = false;
                    }
                }

                if (flag)
                {
                    OnEmpty();
                    break;
                }

                try
                {
                    _sendEventArg.SetBuffer(_sendBufferFlush.Data, (int)_sendBufferFlushOffset, (int)(_sendBufferFlush.Size - _sendBufferFlushOffset));
                    if (!Socket.SendAsync(_sendEventArg))
                    {
                        flag2 = ProcessSend(_sendEventArg);
                    }
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        private void ClearBuffers()
        {
            lock (_sendLock)
            {
                _sendBufferMain.Clear();
                _sendBufferFlush.Clear();
                _sendBufferFlushOffset = 0L;
                BytesPending = 0L;
                BytesSending = 0L;
            }
        }

        private void OnAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (IsSocketDisposed)
            {
                return;
            }

            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    if (ProcessReceive(e))
                    {
                        TryReceive();
                    }

                    break;
                case SocketAsyncOperation.Send:
                    if (ProcessSend(e))
                    {
                        TrySend();
                    }

                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        private bool ProcessReceive(SocketAsyncEventArgs e)
        {
            if (!IsConnected)
            {
                return false;
            }

            long num = e.BytesTransferred;
            if (num > 0)
            {
                BytesReceived += num;
                Interlocked.Add(ref Server._bytesReceived, num);
                OnReceived(_receiveBuffer.Data, 0L, num);
                if (_receiveBuffer.Capacity == num)
                {
                    if (2 * num > OptionReceiveBufferLimit && OptionReceiveBufferLimit > 0)
                    {
                        SendError(SocketError.NoBufferSpaceAvailable);
                        Disconnect();
                        return false;
                    }

                    _receiveBuffer.Reserve(2 * num);
                }
            }

            _receiving = false;
            if (e.SocketError == SocketError.Success)
            {
                if (num > 0)
                {
                    return true;
                }

                Disconnect();
            }
            else
            {
                SendError(e.SocketError);
                Disconnect();
            }

            return false;
        }

        private bool ProcessSend(SocketAsyncEventArgs e)
        {
            if (!IsConnected)
            {
                return false;
            }

            long num = e.BytesTransferred;
            if (num > 0)
            {
                BytesSending -= num;
                BytesSent += num;
                Interlocked.Add(ref Server._bytesSent, num);
                _sendBufferFlushOffset += num;
                if (_sendBufferFlushOffset == _sendBufferFlush.Size)
                {
                    _sendBufferFlush.Clear();
                    _sendBufferFlushOffset = 0L;
                }

                OnSent(num, BytesPending + BytesSending);
            }

            if (e.SocketError == SocketError.Success)
            {
                return true;
            }

            SendError(e.SocketError);
            Disconnect();
            return false;
        }

        protected virtual void OnConnecting()
        {
        }

        protected virtual void OnConnected()
        {
        }

        protected virtual void OnDisconnecting()
        {
        }

        protected virtual void OnDisconnected()
        {
        }

        protected virtual void OnReceived(byte[] buffer, long offset, long size)
        {
        }

        protected virtual void OnSent(long sent, long pending)
        {
        }

        protected virtual void OnEmpty()
        {
        }

        protected virtual void OnError(SocketError error)
        {
        }

        private void SendError(SocketError error)
        {
            if (error != SocketError.ConnectionAborted && error != SocketError.ConnectionRefused && error != SocketError.ConnectionReset && error != SocketError.OperationAborted && error != SocketError.Shutdown)
            {
                OnError(error);
            }
        }

        public void Dispose()
        {
            Dispose(disposingManagedResources: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposingManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposingManagedResources)
                {
                    Disconnect();
                }

                IsDisposed = true;
            }
        }
    }
}
