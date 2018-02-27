using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using CSCore;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using SpectrumAnalyzer.Factories;

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

        private const FftSize FftSize = CSCore.DSP.FftSize.Fft4096;

        private readonly DispatcherTimer _updateSpectrumDispatcherTimer = new DispatcherTimer();
        private int _rate;
        private int _bins;
        private WasapiLoopbackCapture _soundIn;
        private SpectrumProvider _spectrumProvider;
        private float[] _spectrumData;
        private IWaveSource _source;

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

        public string CurrentAudioDevice { get; set; }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            Stop();

            SpectrumData = new ObservableCollection<FrequencyBin>(AnalyzerFactory.CreateMany(Bins));
            _spectrumData = new float[(int) FftSize];

            _soundIn = new WasapiLoopbackCapture();
            _soundIn.Initialize();
            CurrentAudioDevice = _soundIn.Device.FriendlyName;

            var soundInSource = new SoundInSource(_soundIn);
            _spectrumProvider = new SpectrumProvider(soundInSource.WaveFormat.Channels, soundInSource.WaveFormat.SampleRate, FftSize);

            var notificationSource = new SingleBlockNotificationStream(soundInSource.ToSampleSource());
            notificationSource.SingleBlockRead += (s, a) => _spectrumProvider.Add(a.Left, a.Right);
            ForceSingleBlockCall(soundInSource, notificationSource);

            _updateSpectrumDispatcherTimer.Start();

            _soundIn.Start();
        }

        internal void Stop()
        {
            if (_soundIn != null)
            {
                _soundIn.Stop();
                _soundIn.Dispose();
                _soundIn = null;
            }

            if (_source != null)
            {
                _source.Dispose();
                _source = null;
            }

            _updateSpectrumDispatcherTimer.Stop();
        }

        private void ForceSingleBlockCall(SoundInSource soundInSource, ISampleSource notificationSource)
        {
            _source = notificationSource.ToWaveSource(16);
            var buffer = new byte[_source.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += (s, aEvent) =>
            {
                while (_source.Read(buffer, 0, buffer.Length) > 0) { }
            };
        }

        private void UpdateSpectrum(object sender, EventArgs e)
        {
            if (!_spectrumProvider.IsNewDataAvailable) return;
            _spectrumProvider.GetFftData(_spectrumData);

            var bindex = 0; // sorry for this one
            for (var l = 0; l < Bins; l++)
            {
                float peak = 0;
                var binSize = (int)Math.Pow(2, l * 10.0 / (Bins - 1));
                if (binSize > 1023) binSize = 1023;             // max binSize
                if (binSize <= bindex) binSize = bindex + 1;    // min binSize
                for (; bindex < binSize; bindex++)              // select peak
                    if (peak < _spectrumData[1 + bindex]) peak = _spectrumData[1 + bindex];
                var value = Math.Sqrt(peak) * 3 * Normal - 4;
                if (value > Normal) value = Normal;
                if (value < 0) value = 0;
                SpectrumData[l].Value = value;
            }
        }

        #endregion
    }
}