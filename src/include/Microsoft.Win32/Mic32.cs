namespace Microsoft.WinMM {
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Microsoft.Win32;

    public sealed partial class Mic32 : IDisposable {
        private object _dataLock = new object(),
            _disposeLock = new object();

        private int _cc;
        private IntPtr _hwih;
        private WinMM.WaveInProc _hwiproc;

        long _startTime = 0;

        public float GetLocalTime() {
            if (_startTime == 0) { _startTime = Environment.TickCount; }
            return (Environment.TickCount - _startTime) * 0.001f;
        }

        public float ElapsedTime => GetLocalTime();

        Action<Mic32, IntPtr> _onReady;

        public Mic32(int cc, int nSamplesPerSec, Action<Mic32, IntPtr> ready) {
            var m = (int)Math.Log(cc, 2);
            if (cc <= 0 || Math.Pow(2, m) != cc || cc > nSamplesPerSec) {
                throw new ArgumentException();
            }
            this._cc = cc;
            this._CH1 = new float[cc];
            this._CH2 = new float[cc];
            this._wfx = new WinMM.WaveFormatEx();
            this._wfx.wBitsPerSample = 16;
            this._wfx.nChannels = 2;
            this._wfx.nBlockAlign = (short)(_wfx.nChannels * _wfx.wBitsPerSample / 8);
            this._wfx.wFormatTag = (short)WinMM.WaveFormatTag.Pcm;
            this._wfx.nSamplesPerSec = nSamplesPerSec;
            this._wfx.nAvgBytesPerSec = _wfx.nSamplesPerSec * _wfx.nBlockAlign;
            this._wfx.cbSize = 0;
            this._onReady = ready;
            this._hwiproc = new WinMM.WaveInProc((IntPtr waveInHandle, WinMM.WaveInMessage message,
                        IntPtr instance, IntPtr wh, IntPtr param2) => {
                            lock (_disposeLock) {
                                try {
                                    if (_hwih != IntPtr.Zero &&
                                            _onReady != null && message == WinMM.WaveInMessage.DataReady) {
                                        _onReady(this, wh);
                                    }
                                } catch (WinMMException) {
                                }
                            }
                        });
        }

        float[] _CH1;
        float[] _CH2;

        public unsafe void CaptureData(WaveHeader* pwh, short* psData) {
            lock (_dataLock) {
                for (int s = 0; s < _cc; s++) {
                    float ch1 = (psData[(s * Channels)] / 32767.0f),
                        ch2 = (psData[(s * Channels) + 1] / 32767.0f);
                    _CH1[s] = (float)ch1;
                    _CH2[s] = (float)ch2;
                }
            }
        }

        public unsafe void CH1(float[] ch1) {
            Debug.Assert(ch1.Length == _cc);
            lock (_dataLock) {
                for (int s = 0; s < _cc; s++) {
                    _CH1[s] = (float)ch1[s];
                    _CH2[s] = 0;
                }
            }
        }

        public float[] CH1() {
            float[] local;
            lock (_dataLock) {
                local = (float[])_CH1.Clone();
            }
            return local;
        }

        ~Mic32() {
            this.Dispose(false);
        }

        WinMM.WaveFormatEx _wfx;

        public int Hz {
            get {
                return _wfx.nSamplesPerSec;
            }
        }

        public int Channels {
            get {
                return _wfx.nChannels;
            }
        }

        public void Open(bool startmMuted = true) {
            const int WaveInMapperDeviceId = -1;
            if (this._hwih != IntPtr.Zero) {
                throw new InvalidOperationException("The device is already open.");
            }
            IntPtr h = new IntPtr();
            WinMM.Throw(
                WinMM.waveInOpen(
                    ref h,
                    WaveInMapperDeviceId,
                    ref _wfx,
                    this._hwiproc,
                    (IntPtr)0,
                    WinMM.WaveOpenFlags.CALLBACK_FUNCTION | WinMM.WaveOpenFlags.WAVE_FORMAT_DIRECT),
                WinMM.ErrorSource.WaveIn);
            this._hwih = h;
            this.AllocateHeaders();
            _isMuted = true;
            if (!startmMuted) {
                UnMute();
            }
        }

        public void Close() {
            lock (_disposeLock) {
                Mute();
                if (this._hwih == IntPtr.Zero) return;
                WinMM.waveInClose(this._hwih);
                this.FreeHeaders();
                _isMuted = true;
                this._hwih = IntPtr.Zero;
            }
        }

        IntPtr[] _headers = new IntPtr[32];

        unsafe void FreeHeaders() {
            for (int i = 0; i < _headers.Length; i++) {
                WaveHeader* pwh = (WaveHeader*)_headers[i];
                Marshal.FreeHGlobal((*pwh).lpData);
                Marshal.FreeHGlobal((IntPtr)pwh);
                _headers[i] = IntPtr.Zero;
            }
        }

        unsafe void AllocateHeaders() {
            if (this._hwih == IntPtr.Zero) {
                throw new InvalidOperationException();
            }
            for (int i = 0; i < _headers.Length; i++) {
                if (_headers[i] == IntPtr.Zero) {
                    WaveHeader* pwh = (WaveHeader*)Marshal.AllocHGlobal(Marshal.SizeOf(typeof(WaveHeader)));
                    (*pwh).dwFlags = 0;
                    (*pwh).dwBufferLength = _cc
                        * _wfx.nBlockAlign;
                    (*pwh).lpData = Marshal.AllocHGlobal((*pwh).dwBufferLength);
                    (*pwh).dwUser = IntPtr.Zero;
                    _headers[i] = (IntPtr)pwh;
                    WinMM.Throw(
                        WinMM.waveInPrepareHeader(this._hwih, _headers[i], Marshal.SizeOf(typeof(WaveHeader))),
                        WinMM.ErrorSource.WaveOut);
                    WinMM.Throw(
                        WinMM.waveInAddBuffer(this._hwih, _headers[i], Marshal.SizeOf(typeof(WaveHeader))),
                        WinMM.ErrorSource.WaveOut);
                }
            }
        }

        public IntPtr Handle { get => _hwih; }

        public int Samples { get => _cc; }

        unsafe void UnPrepareHeaders() {
            if (this._hwih == IntPtr.Zero) {
                throw new InvalidOperationException();
            }
            for (int i = 0; i < _headers.Length; i++) {
                WaveHeader* pwh = (WaveHeader*)_headers[i];
                if (pwh != null && (((*pwh).dwFlags & WaveHeaderFlags.Prepared) == WaveHeaderFlags.Prepared)) {
                    WinMM.Throw(
                        WinMM.waveInUnprepareHeader(this._hwih, _headers[i], Marshal.SizeOf(typeof(WaveHeader))),
                        WinMM.ErrorSource.WaveOut);
                }
            }
        }

        bool _isMuted = true;

        public bool IsMuted {
            get => _isMuted;
        }

        public void Toggle() {
            if (!_isMuted) {
                Mute();
            } else {
                UnMute();
            }
        }

        public void Mute() {
            _isMuted = true;
            WinMM.Throw(
                WinMM.waveInStop(this._hwih),
                WinMM.ErrorSource.WaveIn);
            if (_onReady != null) {
                _onReady(this, IntPtr.Zero);
            }
        }

        public void UnMute() {
            WinMM.Throw(
               WinMM.waveInStart(this._hwih),
               WinMM.ErrorSource.WaveIn);
            _isMuted = false;
            if (_onReady != null) {
                _onReady(this, IntPtr.Zero);
            }
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing) {
            if (this._hwih != IntPtr.Zero) {
                this.Close();
            }
        }
    }
}