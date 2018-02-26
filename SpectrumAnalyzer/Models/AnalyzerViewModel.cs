using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using SpectrumAnalyzer.Factories;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace SpectrumAnalyzer.Models
{
    public class AnalyzerViewModel
    {
        /// <summary>
        /// captures the default devices audio stream, performs an fft and stores the result in <see cref="SpectrumData"/>
        /// </summary>
        /// <param name="bins">number of frequency bins in <see cref="SpectrumData"/></param>
        /// <param name="rate">number of refreshes per second</param>
        /// <param name="normal">normalized values in <see cref="SpectrumData"/></param>
        public AnalyzerViewModel(int bins = 50, int rate = 50, int normal = 255)
        {
            _updateSpectrumDispatcherTimer.Tick += UpdateSpectrum;

            _bins = bins;
            _rate = rate;
            Normal = normal;
           
            Initialize();
        }

        #region Fields

        private const int Samples = 8192;

        private WASAPIPROC _process;
        private readonly float[] _fft = new float[Samples];
        private BASS_WASAPI_DEVICEINFO _defaultAudioDevice;

        private readonly DispatcherTimer _updateSpectrumDispatcherTimer = new DispatcherTimer();
        private int _rate;
        private int _bins;

        #endregion

        #region Properties

        public ObservableCollection<FrequencyBin> SpectrumData { get; private set; }

        public int Bins
        {
            get => _bins;
            set
            {
                _bins = value;
                SpectrumData = new ObservableCollection<FrequencyBin>(AnalyzerFactory.CreateMany(Bins));
            }
        }

        public int Rate
        {
            get => _rate;
            set
            {
                _rate = value;
                _updateSpectrumDispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / value);
            }
        }

        public int Normal { get; set; }

        public string CurrentAudioDevice => _defaultAudioDevice.name;

        #endregion

        #region Private Methods

        private void Initialize()
        {
            _updateSpectrumDispatcherTimer.Stop();
            BassWasapi.BASS_WASAPI_Stop(true);
            SpectrumData = new ObservableCollection<FrequencyBin>(AnalyzerFactory.CreateMany(Bins));
            CaptureDefaultDevice();
        }

        private void CaptureDefaultDevice()
        {
            _process = Process;
            var defaultDevice = BassWasapi.BASS_WASAPI_GetDeviceInfos().FirstOrDefault(d => d.IsDefault && !d.IsInput);
            var devices = BassWasapi.BASS_WASAPI_GetDeviceInfos(); // MUST be compiled as x86 since basswasapi.dll is ancient
            _defaultAudioDevice = devices.FirstOrDefault(d => d.IsEnabled && d.IsLoopback && d.name == defaultDevice?.name);
            if (_defaultAudioDevice == null) return;

            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

            var deviceIndex = Array.FindIndex(devices, d => d == _defaultAudioDevice);
            BassWasapi.BASS_WASAPI_Init(deviceIndex, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
            BassWasapi.BASS_WASAPI_Start();
            _updateSpectrumDispatcherTimer.Start();
        }

        private static int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        private void UpdateSpectrum(object sender, EventArgs e)
        {
            var dataSet = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT8192);
            if (dataSet < -1) return;

            var bindex = 0; // sorry for this one
            for (var l = 0; l < Bins; l++)
            {
                float peak = 0;
                var binSize = (int)Math.Pow(2, l * 10.0 / (Bins - 1));
                if (binSize > 1023) binSize = 1023;             // max binSize
                if (binSize <= bindex) binSize = bindex + 1;    // min binSize
                for (; bindex < binSize; bindex++)              // select peak
                    if (peak < _fft[1 + bindex]) peak = _fft[1 + bindex];
                var value = Math.Sqrt(peak) * 3 * Normal - 4;
                if (value > Normal) value = Normal;
                if (value < 0) value = 0;
                SpectrumData[l].Value = value;
            }
        }

        #endregion
    }
}