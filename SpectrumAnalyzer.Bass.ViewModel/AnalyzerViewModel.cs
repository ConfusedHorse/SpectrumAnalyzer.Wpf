using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace SpectrumAnalyzer.Bass.ViewModel
{
    public class AnalyzerViewModel : ViewModelBase
    {
        public AnalyzerViewModel()
        {
            _updateSpectrumDispatcherTimer.Tick += UpdateSpectrum;
            _updateSpectrumDispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / Hertz);

            CaptureDefaultDevice();
        }

        #region Fields

        public const int Lines = 128;
        public const int Hertz = 60;
        public const int Normal = 255;
        private const int Samples = 8192;

        private WASAPIPROC _process;
        private readonly float[] _fft = new float[Samples];

        private readonly DispatcherTimer _updateSpectrumDispatcherTimer = new DispatcherTimer();

        #endregion

        #region Properties

        public BASS_WASAPI_DEVICEINFO DefaultAudioDevice { get; set; }

        public int Maximum => Normal;

        public ObservableCollection<int> SpectrumData { get; set; } = new ObservableCollection<int>();

        #endregion

        #region Private Methods

        private void CaptureDefaultDevice()
        {
            _process = new WASAPIPROC(Process);
            var defaultDevice = BassWasapi.BASS_WASAPI_GetDeviceInfos().FirstOrDefault(d => d.IsDefault && !d.IsInput);
            var devices = BassWasapi.BASS_WASAPI_GetDeviceInfos(); // MUST be compiled as x86 since basswasapi.dll is ancient
            DefaultAudioDevice = devices.FirstOrDefault(d => d.IsEnabled && d.IsLoopback && d.name == defaultDevice?.name);
            if (DefaultAudioDevice == null) return;

            Un4seen.Bass.Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            Un4seen.Bass.Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            var deviceIndex = Array.FindIndex(devices, d => d == DefaultAudioDevice);
            BassWasapi.BASS_WASAPI_Init(deviceIndex, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
            BassWasapi.BASS_WASAPI_Start();
            _updateSpectrumDispatcherTimer.Start();
        }

        private static int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        //private void ResetCapture()
        //{
        //    _updateSpectrumDispatcherTimer.Stop();
        //    BassWasapi.BASS_WASAPI_Stop(true);
        //    BassWasapi.BASS_WASAPI_Free();
        //    Un4seen.Bass.Bass.BASS_Free();
        //}

        private void UpdateSpectrum(object sender, EventArgs e)
        {
            SpectrumData.Clear();
            
            var dataSet = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT8192);
            if (dataSet < -1) return;

            var bindex = 0; // sorry for this one
            for (var l = 0; l < Lines; l++)
            {
                float peak = 0;
                var binSize = (int)Math.Pow(2, l * 10.0 / (Lines - 1));
                if (binSize > 1023) binSize = 1023;             // max binSize
                if (binSize <= bindex) binSize = bindex + 1;    // min binSize
                for (; bindex < binSize; bindex++)              // select peak
                    if (peak < _fft[1 + bindex]) peak = _fft[1 + bindex];
                var value = (int)(Math.Sqrt(peak) * 3 * Normal - 4);
                if (value > Normal) value = Normal;
                if (value < 0) value = 0;
                SpectrumData.Add(value);
            }
        }

        #endregion
    }
}