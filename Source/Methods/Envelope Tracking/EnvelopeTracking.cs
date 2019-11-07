﻿using NationalInstruments.DataInfrastructure;
using NationalInstruments.ModularInstruments;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.SystemServices.TimingServices;
using System;
using System.Linq;

namespace NationalInstruments.ReferenceDesignLibraries.Methods
{
    public static class EnvelopeTracking
    {

        #region Type Definitions
        public struct EnvelopeGeneratorConfiguration
        {
            public RfsgTerminalConfiguration TerminalConfiguration;

            public static EnvelopeGeneratorConfiguration GetDefault()
            {
                return new EnvelopeGeneratorConfiguration()
                {
                    TerminalConfiguration = RfsgTerminalConfiguration.Differential,
                };
            }
        }

        public struct TrackerConfiguration
        {
            public double InputImpedance_Ohms;
            public double CommonModeOffset_V;
            public double Gain_VperV;
            public double OutputOffset_V;

            public static TrackerConfiguration GetDefault()
            {
                return new TrackerConfiguration()
                {
                    InputImpedance_Ohms = 1e6,
                    CommonModeOffset_V = 1.0,
                    Gain_VperV = 2.5,
                    OutputOffset_V = 2.55
                };
            }
        }

        public enum DetroughType { 
            Exponential, 
            Cosine, 
            Power
        };

        public struct DetroughConfiguration
        {
            public DetroughType Type;
            public double MinimumVoltage;
            public double MaximumVoltage;
            public double Exponent;

            public static DetroughConfiguration GetDefault()
            {
                return new DetroughConfiguration()
                {
                    Type = DetroughType.Exponential,
                    MinimumVoltage = 1.5,
                    MaximumVoltage = 3.5,
                    Exponent = 1.2
                };
            }
        }

        public struct LookUpTable
        {
            public float[] DutInputPower_dBm;
            public float[] SupplyVoltage_V;
        }

        public struct SynchronizationConfiguration
        {
            public double RFDelayRange_s;
            public double RFDelay_s;

            public static SynchronizationConfiguration GetDefault()
            {
                return new SynchronizationConfiguration()
                {
                    RFDelayRange_s = 2e-6, // +/- 1us
                    RFDelay_s = 0.0
                };
            }
        }
        #endregion

        #region Envelope Creation
        public static Waveform CreateEnvelopeWaveform(Waveform referenceWaveform, DetroughConfiguration detroughConfig)
        {
            Waveform envelopeWaveform = new Waveform()
            {
                Name = referenceWaveform.Name + "Envelope",
                Data = referenceWaveform.Data.Clone(),
                SignalBandwidth_Hz = 0.8 * referenceWaveform.SampleRate,
                PAPR_dB = double.NaN, // unnecessary to calculate as it won't be used
                BurstLength_s = double.NaN, // not applicable in ET
                SampleRate = referenceWaveform.SampleRate,
                // burst start and stop locations will be left as null since they also will not be used
                IdleDurationPresent = referenceWaveform.IdleDurationPresent, // can't null a bool, so copy from reference waveform
                RuntimeScaling = 10 * Math.Log10(0.9), // applies 10% headroom to the waveform at runtime
                Script = referenceWaveform.Script // we will copy the script but call replace on it in the following lines to change the waveform name
            };
            envelopeWaveform.Script = envelopeWaveform.Script.Replace(referenceWaveform.Name, envelopeWaveform.Name); // this is better here since now we only have to add the suffix once
            WritableBuffer<ComplexSingle> envWfmWriteBuffer = envelopeWaveform.Data.GetWritableBuffer();

            double[] iqMagnitudes = referenceWaveform.Data.GetMagnitudeDataArray(false);
            double detroughRatio = detroughConfig.MinimumVoltage / detroughConfig.MaximumVoltage;

            // Waveforms are assumed to be normalized in range [0, 1], so no normalization will happen here
            switch (detroughConfig.Type)
            {
                case DetroughType.Exponential:
                    // Formula: e = IQmag + d*exp(-IQmag/d)
                    double expScale = 1 + detroughRatio * Math.Exp(-1.0 / detroughRatio);
                    for (int i = 0; i < envWfmWriteBuffer.Size; i++)
                    {
                        double sampleValue = iqMagnitudes[i] + detroughRatio * Math.Exp(-iqMagnitudes[i] / detroughRatio);
                        envWfmWriteBuffer[i] = ComplexSingle.FromSingle((float)(detroughConfig.MaximumVoltage * sampleValue / expScale)); // Scale detrough to Vmax. Divide by detroughed waveform's max value to normalize. IQMagnitude's max value is 1
                    }
                    break;
                case DetroughType.Cosine:
                    // Formula: e = 1 - (1-d)*cos(IQmag*pi/2)
                    double cosScale = 1 - (1 - detroughRatio) * Math.Cos(Math.PI / 2);
                    for (int i = 0; i < envWfmWriteBuffer.Size; i++)
                    {
                        double sampleValue = 1 - (1 - detroughRatio) * Math.Cos(iqMagnitudes[i] * cosScale);
                        envWfmWriteBuffer[i] = ComplexSingle.FromSingle((float)(detroughConfig.MaximumVoltage * sampleValue / cosScale)); // Scale detrough to Vmax. Divide by detroughed waveform's max value to normalize. IQMagnitude's max value is 1: 1-(1-d) *cos(1*pi/2)
                    }
                    break;
                case DetroughType.Power:
                    // Formula: e = (1-d) + IQmag^(exponent)*(1-d)
                    double powScale = 2 - 2 * detroughRatio;
                    for (int i = 0; i < envWfmWriteBuffer.Size; i++)
                    {
                        double sampleValue = (1 - detroughRatio) + (Math.Pow(iqMagnitudes[i], detroughConfig.Exponent) * (1 - detroughRatio));
                        envWfmWriteBuffer[i] = ComplexSingle.FromSingle((float)(detroughConfig.MaximumVoltage * sampleValue / powScale)); // Scale detrough to Vmax. Divide by detroughed waveform's max value. IQMagnitude's max value is 1: (1-d) + (1^exponent)*(1-d)
                    }
                    break;
                default:
                    throw new ArgumentException("Detrough type not supported.\n");
            }

            return envelopeWaveform;
        }

        public static Waveform CreateEnvelopeWaveform(Waveform referenceWaveform, LookUpTable lookUpTable, double dutAverageInputPower)
        {
            ComplexSingle[] iq = referenceWaveform.Data.GetRawData(); // get copy of iq samples
            
            /// power conversions needed to understand following scaling:
            /// power_dBW = 20log(V) - 10log(R) - since R is 100 we get power_dBW = 20log(V) - 20
            /// if we want to normalize to dbW 1ohm, we would have 20log(V) - 10log(1) = 20log(V)
            /// power_dBm = power_dBW + 30  therefore power_dBm = 20log(V) + 10
            /// therefore, to convert from dBm to dBW 1ohm we subtract 10 from dBm value

            // scale waveform to have average dBW 1ohm power equal to dut input power normalized to dBW 1ohm
            ComplexSingle scale = ComplexSingle.FromSingle((float)Math.Pow(10.0, referenceWaveform.PAPR_dB / 20.0)); // scaling value for average power to equal 0dBW 1ohm
            scale *= ComplexSingle.FromSingle((float)Math.Pow(10.0, (dutAverageInputPower - 10.0) / 20.0)); // cascade scale values so we only have to run through the array once
            for (int i = 0; i < iq.Length; i++)
                iq[i] *= scale;

            // get 1 ohm power trace in watts of scaled iq data
            float[] powerTrace_W = new float[iq.Length];
            for (int i = 0; i < iq.Length; i++)
                powerTrace_W[i] = iq[i].Real * iq[i].Real + iq[i].Imaginary * iq[i].Imaginary;

            // get lookup table input power trace in 1ohm watts
            float[] lutDutInputPowerWattOneOhm = new float[lookUpTable.DutInputPower_dBm.Length];
            for (int i = 0; i < lookUpTable.DutInputPower_dBm.Length; i++)
                lutDutInputPowerWattOneOhm[i] = (float)Math.Pow(10.0, (lookUpTable.DutInputPower_dBm[i] - 10.0) / 10.0); // V^2 = 10^((Pin - 10.0)/10)

            // run the trace through 1D interpolation
            float[] rawEnvelope = LinearInterpolation1D(lutDutInputPowerWattOneOhm, lookUpTable.SupplyVoltage_V, powerTrace_W);

            // create waveform to return to the user
            Waveform envelopeWaveform = new Waveform()
            {
                Name = referenceWaveform.Name + "Envelope",
                Data = referenceWaveform.Data.Clone(),
                SignalBandwidth_Hz = 0.8 * referenceWaveform.SampleRate,
                PAPR_dB = double.NaN, // unnecessary to calculate as it won't be used
                BurstLength_s = double.NaN, // not applicable in ET
                SampleRate = referenceWaveform.SampleRate,
                // burst start and stop locations will be left as null since they also will not be used
                IdleDurationPresent = referenceWaveform.IdleDurationPresent, // can't null a bool, so copy from reference waveform
                RuntimeScaling = 10 * Math.Log10(0.9), // applies 10% headroom to the waveform at runtime
                Script = referenceWaveform.Script // we will copy the script but call replace on it in the following lines to change the waveform name
            };
            envelopeWaveform.Script = envelopeWaveform.Script.Replace(referenceWaveform.Name, envelopeWaveform.Name); // this is better here since now we only have to add the suffix once
            WritableBuffer<ComplexSingle> envWfmWriteBuffer = envelopeWaveform.Data.GetWritableBuffer();

            // copy raw envelope data into cloned envelope waveform
            for (int i = 0; i < rawEnvelope.Length; i++)
                envWfmWriteBuffer[i] = ComplexSingle.FromSingle(rawEnvelope[i]);

            return envelopeWaveform;
        }
        #endregion

        #region Instrument Configuration
        public static void ConfigureEnvelopeGenerator(NIRfsg envVsg, EnvelopeGeneratorConfiguration envVsgConfig, TrackerConfiguration trackerConfig)
        {
            if (envVsgConfig.TerminalConfiguration == RfsgTerminalConfiguration.Differential)
                envVsg.IQOutPort[""].LoadImpedance = trackerConfig.InputImpedance_Ohms == 50.0 ? 100.0 : trackerConfig.InputImpedance_Ohms;
            else
                envVsg.IQOutPort[""].LoadImpedance = trackerConfig.InputImpedance_Ohms;
            envVsg.IQOutPort[""].TerminalConfiguration = envVsgConfig.TerminalConfiguration;
            envVsg.IQOutPort[""].CommonModeOffset = trackerConfig.CommonModeOffset_V;
        }

        public static Waveform ScaleAndDownloadEnvelopeWaveform(NIRfsg envVsg, Waveform envelopeWaveform, TrackerConfiguration trackerConfig)
        {
            // grab the raw envelope so we can use linq to get statistics on it
            ComplexSingle.DecomposeArray(envelopeWaveform.Data.GetRawData(), out float[] envelope, out _);

            // scale envelope to adjust for tracker gain and offset
            for (int i = 0; i < envelope.Length; i++)
                envelope[i] = (float)((envelope[i] - trackerConfig.OutputOffset_V) / trackerConfig.Gain_VperV);

            // clone an envelope waveform to return to the user - want unique waveforms per tracker configuration
            Waveform scaledEnvelopeWaveform = envelopeWaveform;
            scaledEnvelopeWaveform.Data = envelopeWaveform.Data.Clone();
            WritableBuffer<ComplexSingle> scaledEnvelopeWaveformBuffer = scaledEnvelopeWaveform.Data.GetWritableBuffer();

            // populate cloned waveform with scaled waveform data
            for (int i = 0; i < envelope.Length; i++)
                scaledEnvelopeWaveformBuffer[i] = ComplexSingle.FromSingle(envelope[i]);

            // get statistics on the waveform
            float max = envelope.Max();
            float min = envelope.Min();
            float gain = (max - min) * 0.5f; // distance between middle of waveform and max/min
            float offset = min + gain;
            float absolutePeak = Math.Abs(offset) + gain; // peak voltage of the waveform with absolute value of offset

            // scale waveform to peak voltage
            for (int i = 0; i < envelope.Length; i++)
                envelope[i] = envelope[i] / (absolutePeak); // brings waveform down to +/- 1 magnitude

            // set instrument properties
            envVsg.IQOutPort[""].Level = 2 * absolutePeak; // gain is interpreted as peak-to-peak
            envVsg.IQOutPort[""].Offset = 0.0; // set offset to 0 since this is done in DSP not in HW on the 5820 and only clips the waveform further

            // create another waveform that we can use to download the scaled envelope to the instrument
            Waveform instrEnvelopeWaveform = envelopeWaveform;
            instrEnvelopeWaveform.Data = envelopeWaveform.Data.Clone();
            WritableBuffer<ComplexSingle> instrEnvelopeWaveformBuffer = scaledEnvelopeWaveform.Data.GetWritableBuffer();

            // populate cloned waveform with scaled waveform data
            for (int i = 0; i < envelope.Length; i++)
                instrEnvelopeWaveformBuffer[i] = ComplexSingle.FromSingle(envelope[i]);

            SG.DownloadWaveform(envVsg, instrEnvelopeWaveform); // download optimized waveform

            return scaledEnvelopeWaveform; // return the waveform as it will appear coming out of the front end of the envelope generator
        }

        public static void InitiateSynchronousGeneration(NIRfsg rfVsg, NIRfsg envVsg, SynchronizationConfiguration syncConfig)
        {   
            TClock tclk = new TClock(new ITClockSynchronizableDevice[] { rfVsg, envVsg });
            tclk.DevicesToSynchronize[0].SampleClockDelay = -syncConfig.RFDelayRange_s / 2.0; // The VST2 can only apply positive delays so we have to establish an inital delay of -RFDelayRange/2 for TCLK to handle negative shifts as well
            rfVsg.Arb.RelativeDelay = syncConfig.RFDelayRange_s + syncConfig.RFDelay_s;
            tclk.ConfigureForHomogeneousTriggers();
            tclk.Synchronize(); // #todo: allow drift?
            tclk.Initiate();
        }
        #endregion

        private static float[] LinearInterpolation1D(float[] x, float[] y, float[] xi, bool monotonic = false)
        {
            // assumes x and y are the same length and lengths are > 1

            if (!monotonic)
            {
                // clone array so we don't act on user's reference
                x = (float[])x.Clone();
                y = (float[])y.Clone();
                Array.Sort(x, y);
            }

            float[] yi = new float[xi.Length];

            for (int i = 0; i < xi.Length; i++)
            {
                float x1 = xi[i];
                int index = Array.BinarySearch(x, x1);
                if (index < 0) // if the needle falls within two values in the haystack, search returns bitwise compliment of index of next larger value
                    index = ~index; // take bitwise compliment to get index of next larger value
                if (index == 0) // handle edge case where value is less than first element in the lut
                    index = 1; // interpolate on first two elements in the lut
                else if (index == x.Length) // handle edge case where value is greater than last element in the lut
                    index = x.Length - 1; // interpolate on last two elements in the lut
                float x0 = x[index - 1];
                float x2 = x[index];
                float y0 = y[index - 1];
                float y2 = y[index];
                yi[i] = (y2 - y0) / (x2 - x0) * (x1 - x0) + y0;
            }

            return yi;
        }
    }
}