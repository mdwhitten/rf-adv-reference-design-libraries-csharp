﻿using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.SpecAnMX;



namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class RFmxSpecAn
    {
        #region Type Definitions
        public struct CommonConfiguration
        {
            public string SelectedPorts;
            public double CenterFrequency_Hz;
            public double ReferenceLevel_dBm;
            public double ExternalAttenuation_dB;
            public string FrequencyReferenceSource;
            public string DigitalEdgeSource;
            public RFmxSpecAnMXDigitalEdgeTriggerEdge DigitalEdgeType;
            public double TriggerDelay_s;
            public bool EnableTrigger;
            public static CommonConfiguration GetDefault()
            {
                return new CommonConfiguration
                {
                    SelectedPorts = "",
                    CenterFrequency_Hz = 1e9,
                    ReferenceLevel_dBm = 0,
                    ExternalAttenuation_dB = 0,
                    FrequencyReferenceSource = RFmxInstrMXConstants.PxiClock,
                    DigitalEdgeSource = RFmxInstrMXConstants.PxiTriggerLine0,
                    DigitalEdgeType = RFmxSpecAnMXDigitalEdgeTriggerEdge.Rising,
                    TriggerDelay_s = 0,
                    EnableTrigger = true,
                };
            }
        }

        public struct AutoLevelConfiguration
        {
            public bool AutoLevelReferenceLevel;
            public double AutoLevelMeasureTime_s;
            public double AutoLevelBandwidth_Hz;
            public static AutoLevelConfiguration GetDefault()
            {
                return new AutoLevelConfiguration
                {
                    AutoLevelReferenceLevel = false,
                    AutoLevelMeasureTime_s = 10e-3,
                    AutoLevelBandwidth_Hz=20e6
                };
            }
        }
        public struct TxpConfiguration
        {
            public double MeasurementInterval_s;
            public double Rbw_Hz;
            public RFmxSpecAnMXTxpRbwFilterType RbwFilterType;
            public double RrfAlpha;

            public RFmxSpecAnMXTxpAveragingEnabled AveragingEnabled;
            public int AveragingCount;
            public RFmxSpecAnMXTxpAveragingType AveragingType;

            public static TxpConfiguration GetDefault()
            {
                return new TxpConfiguration
                {
                    MeasurementInterval_s = 1e-3,
                    Rbw_Hz = 20e6,
                    RbwFilterType= RFmxSpecAnMXTxpRbwFilterType.None,
                    RrfAlpha=0.01,

                    AveragingEnabled=RFmxSpecAnMXTxpAveragingEnabled.False,
                    AveragingCount = 10 ,
                    AveragingType = RFmxSpecAnMXTxpAveragingType.Rms,
                 };
            }
        }

        public struct AcpCommonConfiguration
        {
            public RFmxSpecAnMXAcpPowerUnits PowerUnits;
            public RFmxSpecAnMXAcpAveragingEnabled AveragingEnabled;
            public int AveragingCount;
            public RFmxSpecAnMXAcpAveragingType AveragingType;
            public RFmxSpecAnMXAcpFftWindow FftWindow;
            public double FftPadding;

            public RFmxSpecAnMXAcpRbwAutoBandwidth RbwAuto;
            public double Rbw_Hz;
            public RFmxSpecAnMXAcpRbwFilterType RbwFilterType;

            public RFmxSpecAnMXAcpSweepTimeAuto SweepTimeAuto;
            public double SweepTimeInterval_s;
            public static AcpCommonConfiguration GetDefault()
            {
                return new AcpCommonConfiguration
                {
                    PowerUnits = RFmxSpecAnMXAcpPowerUnits.dBm,
                    AveragingEnabled = RFmxSpecAnMXAcpAveragingEnabled.False,
                    AveragingCount = 10,
                    AveragingType = RFmxSpecAnMXAcpAveragingType.Rms,
                    FftWindow = RFmxSpecAnMXAcpFftWindow.FlatTop,
                    FftPadding = 1,
                    RbwAuto = RFmxSpecAnMXAcpRbwAutoBandwidth.True,
                    Rbw_Hz = 10e3,
                    RbwFilterType = RFmxSpecAnMXAcpRbwFilterType.Gaussian,
                    SweepTimeAuto = RFmxSpecAnMXAcpSweepTimeAuto.True,
                    SweepTimeInterval_s = 1e-3

                    };
            }
        }
        public struct AcpCarrierChannelConfiguration
        {
            public double IntegrationBandwidth_Hz;
            public RFmxSpecAnMXAcpCarrierRrcFilterEnabled RrcFilterEnabled;
            public double RrcAlpha;
            public RFmxSpecAnMXAcpCarrierMode Mode;
            public double Frequency;
            public static AcpCarrierChannelConfiguration GetDefault()
            {
                return new AcpCarrierChannelConfiguration
                {
                    IntegrationBandwidth_Hz = 18e6,
                    RrcFilterEnabled = RFmxSpecAnMXAcpCarrierRrcFilterEnabled.False,
                    RrcAlpha = 0.22,
                    Mode = RFmxSpecAnMXAcpCarrierMode.Active,
                    Frequency = 0.00
                };
        
            }
        }
        public struct AcpOffsetChannelConfiguration
        {
            public double IntegrationBandwidth_Hz;
            public RFmxSpecAnMXAcpOffsetEnabled OffsetEnabled;
            public double OffsetFrequency_Hz;
            public RFmxSpecAnMXAcpOffsetSideband OffsetSideBand;
            public RFmxSpecAnMXAcpOffsetPowerReferenceCarrier OffsetPowerReferenceCarrier;
            public int OffsetPowerReferenceSpecificIndex;
            public double RelativeAttenuation_dB;
            public RFmxSpecAnMXAcpOffsetRrcFilterEnabled RrcFilterEnabled;
            public double RrcFilterAlpha;

            public static AcpOffsetChannelConfiguration GetDefault()
            {
                return new AcpOffsetChannelConfiguration
                {
                    IntegrationBandwidth_Hz = 18e6,
                    OffsetEnabled = RFmxSpecAnMXAcpOffsetEnabled.True,
                    OffsetFrequency_Hz = 20e6,
                    OffsetSideBand = RFmxSpecAnMXAcpOffsetSideband.Both,
                    OffsetPowerReferenceCarrier = RFmxSpecAnMXAcpOffsetPowerReferenceCarrier.Closest,
                    OffsetPowerReferenceSpecificIndex = 0,
                    RelativeAttenuation_dB = 0,
                    RrcFilterEnabled = RFmxSpecAnMXAcpOffsetRrcFilterEnabled.False,
                    RrcFilterAlpha = 0.22
                };
            }
        }

        public struct AmpmConfiguration
        {
            public double MeasurementInterval_s;
            public double DutInputPower_dBm;
            public ComplexWaveform<ComplexSingle> ReferenceWaveform;
            public RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent IdleDurationPresent;
            public RFmxSpecAnMXAmpmSignalType SignalType;


            public static AmpmConfiguration GetDefault()
            {
                return new AmpmConfiguration
                {
                    MeasurementInterval_s = 100e-6,
                    DutInputPower_dBm = -20,
                    ReferenceWaveform = new ComplexWaveform<ComplexSingle>(0),
                    IdleDurationPresent = RFmxSpecAnMXAmpmReferenceWaveformIdleDurationPresent.False,
                    SignalType = RFmxSpecAnMXAmpmSignalType.Modulated
                };

            }
        }

        public struct TxpResults
        {
            public double AverageMeanPower_dBm;
            public double PeakToAverageRatio_dB;
            public double MaximumPower_dBm;
            public double MinimumPower_dBm;
        }
        public struct AcpOffsetResults
        {
            public double LowerAbsolutePower_dBm;
            public double LowerRelativePower_dB;
            public double UpperAbsolutePower_dBm;
            public double UpperRelativePower_dB;
            public double OffsetFrequency_Hz;
            public double OffsetIntegrationBandwidth_Hz;
        }

        public struct AcpComponentCarrierResults
        {
            public double CarrierFrequency_Hz;
            public double IntegrationBandwidth_Hz;
            public double AbsolutePower_dBm_or_dBmHz;
            public double TotalRelativePower_dB;
        }

        public struct AcpResults
        {
            public AcpOffsetResults[] OffsetResults;
            public AcpComponentCarrierResults[] ComponentCarrierResults;
        }
        public struct AmpmResults
        {
            public double MeanLinearGain_dB;
            public double OnedBCompressionPoint_dBm;
            public double MeanRmsEvm_percent;
            public double AmToAMResidual;
            public double AmToPMResidual;
        }


        #endregion
        #region Instrument Configurations
        public static void ConfigureCommon(RFmxInstrMX sessionHandle, RFmxSpecAnMX specAnSignal, CommonConfiguration commonConfig,
            AutoLevelConfiguration autoLevelConfig, string selectorString = "")
        {
            sessionHandle.ConfigureFrequencyReference("", commonConfig.FrequencyReferenceSource, 10e6);
            specAnSignal.SetSelectedPorts(selectorString, commonConfig.SelectedPorts);
            specAnSignal.ConfigureDigitalEdgeTrigger(selectorString, commonConfig.DigitalEdgeSource, commonConfig.DigitalEdgeType, commonConfig.TriggerDelay_s, commonConfig.EnableTrigger);
            specAnSignal.ConfigureFrequency(selectorString, commonConfig.CenterFrequency_Hz);
            specAnSignal.ConfigureExternalAttenuation(selectorString, commonConfig.ExternalAttenuation_dB);
            if (autoLevelConfig.AutoLevelReferenceLevel) specAnSignal.AutoLevel(selectorString, autoLevelConfig.AutoLevelBandwidth_Hz, autoLevelConfig.AutoLevelMeasureTime_s, out _);
            else specAnSignal.ConfigureReferenceLevel(selectorString, commonConfig.ReferenceLevel_dBm);

        }
        public static void ConfigureTxp(RFmxSpecAnMX specAn, TxpConfiguration txpConfig, string selectorString = "")
        {
            specAn.Txp.Configuration.SetMeasurementEnabled(selectorString, true);
            specAn.Txp.Configuration.SetMeasurementInterval(selectorString,txpConfig.MeasurementInterval_s);
            specAn.Txp.Configuration.ConfigureRbwFilter(selectorString,txpConfig.Rbw_Hz,txpConfig.RbwFilterType,txpConfig.RrfAlpha);
           
        }
        public static void ConfigureAcpCommon(RFmxSpecAnMX specAn, AcpCommonConfiguration acpCommonConfig, string selectorString = "")
        {
            specAn.Acp.Configuration.SetMeasurementEnabled(selectorString, true);
            specAn.Acp.Configuration.ConfigurePowerUnits(selectorString, acpCommonConfig.PowerUnits);
            specAn.Acp.Configuration.ConfigureAveraging(selectorString, acpCommonConfig.AveragingEnabled, acpCommonConfig.AveragingCount, acpCommonConfig.AveragingType);
            specAn.Acp.Configuration.ConfigureFft(selectorString, acpCommonConfig.FftWindow, acpCommonConfig.FftPadding);
            specAn.Acp.Configuration.ConfigureRbwFilter(selectorString, acpCommonConfig.RbwAuto, acpCommonConfig.Rbw_Hz, acpCommonConfig.RbwFilterType);
            specAn.Acp.Configuration.ConfigureSweepTime(selectorString, acpCommonConfig.SweepTimeAuto, acpCommonConfig.SweepTimeInterval_s);
        }
        public static void ConfigureAcpCarrierChannel(RFmxSpecAnMX specAn, AcpCarrierChannelConfiguration[] acpCarrierChannelConfiguration, string selectorString = "")
        {
            specAn.Acp.Configuration.ConfigureNumberOfCarriers(selectorString,acpCarrierChannelConfiguration.Length);
            for (int i = 0; i < acpCarrierChannelConfiguration.Length; i++)
            {
                string carrierString = RFmxSpecAnMX.BuildCarrierString2(selectorString, i);
                specAn.Acp.Configuration.ConfigureCarrierIntegrationBandwidth(carrierString, acpCarrierChannelConfiguration[i].IntegrationBandwidth_Hz);
                specAn.Acp.Configuration.ConfigureCarrierRrcFilter(carrierString, acpCarrierChannelConfiguration[i].RrcFilterEnabled, acpCarrierChannelConfiguration[i].RrcAlpha);
                specAn.Acp.Configuration.ConfigureCarrierMode(carrierString, acpCarrierChannelConfiguration[i].Mode);
                specAn.Acp.Configuration.ConfigureCarrierFrequency(carrierString, acpCarrierChannelConfiguration[i].Frequency);
            }
        }
        public static void ConfigureAcpOffsetChannel(RFmxSpecAnMX specAn, AcpOffsetChannelConfiguration[] acpOffsetChannelConfiguration, string selectorString = "")
        {
            specAn.Acp.Configuration.ConfigureNumberOfOffsets(selectorString,acpOffsetChannelConfiguration.Length);

            for (int i = 0; i < acpOffsetChannelConfiguration.Length; i++)
            {
                string offsetString = RFmxSpecAnMX.BuildOffsetString2(selectorString, i);
                specAn.Acp.Configuration.ConfigureOffsetIntegrationBandwidth(offsetString, acpOffsetChannelConfiguration[i].IntegrationBandwidth_Hz);
                specAn.Acp.Configuration.ConfigureOffset(offsetString, acpOffsetChannelConfiguration[i].OffsetFrequency_Hz,acpOffsetChannelConfiguration[i].OffsetSideBand,acpOffsetChannelConfiguration[i].OffsetEnabled);
                specAn.Acp.Configuration.ConfigureOffsetPowerReference(offsetString, acpOffsetChannelConfiguration[i].OffsetPowerReferenceCarrier,acpOffsetChannelConfiguration[i].OffsetPowerReferenceSpecificIndex);
                specAn.Acp.Configuration.ConfigureOffsetRelativeAttenuation(offsetString, acpOffsetChannelConfiguration[i].RelativeAttenuation_dB);
                specAn.Acp.Configuration.ConfigureOffsetRrcFilter(offsetString, acpOffsetChannelConfiguration[i].RrcFilterEnabled, acpOffsetChannelConfiguration[i].RrcFilterAlpha);
            }
        }

        public static void ConfigureAmpm(RFmxSpecAnMX specAn, AmpmConfiguration ampmConfig, string selectorString = "")
        {
            specAn.Ampm.Configuration.SetMeasurementEnabled(selectorString, true);
            specAn.Ampm.Configuration.ConfigureMeasurementInterval(selectorString, ampmConfig.MeasurementInterval_s);
            specAn.Ampm.Configuration.ConfigureDutAverageInputPower(selectorString, ampmConfig.DutInputPower_dBm);
            specAn.Ampm.Configuration.ConfigureReferenceWaveform(selectorString, ampmConfig.ReferenceWaveform, ampmConfig.IdleDurationPresent, ampmConfig.SignalType);
        }

        #endregion
        #region Measurement Results
        public static TxpResults FetchTxp(RFmxSpecAnMX specAn, string selectorString = "")
        {
            TxpResults txpResults = new TxpResults();
            specAn.Txp.Results.FetchMeasurement(selectorString, 10.0, out txpResults.AverageMeanPower_dBm, 
                out txpResults.PeakToAverageRatio_dB, out txpResults.MaximumPower_dBm, out txpResults.MinimumPower_dBm);
            return txpResults;
        }

        public static AcpResults FetchAcp(RFmxSpecAnMX specAn, int numCarrier, string selectorString = "")
        {
            double[] lowerRelativePower = null;
            double[] upperRelativePower = null;
            double[] lowerAbsolutePower = null;
            double[] upperAbsolutePower = null;
            specAn.Acp.Results.FetchOffsetMeasurementArray(selectorString, 10.0,
                ref lowerRelativePower, ref upperRelativePower, ref lowerAbsolutePower, ref upperAbsolutePower);
            AcpResults results = new AcpResults()
            {
                OffsetResults = new AcpOffsetResults[lowerRelativePower.Length]
            };
            for (int i = 0; i < lowerRelativePower.Length; i++)
            {
                string offsetString = RFmxSpecAnMX.BuildOffsetString2(selectorString, i);
                specAn.Acp.Configuration.GetOffsetFrequency(offsetString, out double offsetFrequency);
                specAn.Acp.Configuration.GetOffsetIntegrationBandwidth(offsetString, out double offsetIbw);
                results.OffsetResults[i] = new AcpOffsetResults()
                {
                    LowerRelativePower_dB = lowerRelativePower[i],
                    UpperRelativePower_dB = upperRelativePower[i],
                    LowerAbsolutePower_dBm = lowerAbsolutePower[i],
                    UpperAbsolutePower_dBm = upperAbsolutePower[i],
                    OffsetFrequency_Hz = offsetFrequency,
                    OffsetIntegrationBandwidth_Hz = offsetIbw
                };
            }

            
            results.ComponentCarrierResults = new AcpComponentCarrierResults[numCarrier];
            for (int i = 0; i < numCarrier; i++)
            {
                string carrierString = RFmxSpecAnMX.BuildCarrierString2(selectorString, i);
                specAn.Acp.Results.FetchCarrierMeasurement(carrierString, 10.0, out results.ComponentCarrierResults[i].AbsolutePower_dBm_or_dBmHz,
                            out results.ComponentCarrierResults[i].TotalRelativePower_dB, out results.ComponentCarrierResults[i].CarrierFrequency_Hz,
                            out results.ComponentCarrierResults[i].IntegrationBandwidth_Hz);
            }
            return results;
        }

        public static AmpmResults FetchAmpm(RFmxSpecAnMX specAn, string selectorString = "")
        {
            AmpmResults ampmResults = new AmpmResults();
            specAn.Ampm.Results.FetchDutCharacteristics(selectorString,10.0, out ampmResults.MeanLinearGain_dB, out ampmResults.OnedBCompressionPoint_dBm, out ampmResults.MeanRmsEvm_percent);
            specAn.Ampm.Results.FetchCurveFitResidual(selectorString,10.0,out ampmResults.AmToAMResidual,out ampmResults.AmToPMResidual);
            return ampmResults;
        }

        #endregion
    }
}

