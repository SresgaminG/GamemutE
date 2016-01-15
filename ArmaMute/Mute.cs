using System.Collections.Generic;
using CoreAudio;

namespace SresgaminG.Arma3
{
    public class Mute
    {
        private static MMDeviceCollection devCol;

        private static List<KeyValuePair<int, int>> devicesAndSessions = new List<KeyValuePair<int, int>>();

        public static void MuteUnmute(string appName)
        {
            LogHelper.Debug(null, "Starting mute functionality");

            devicesAndSessions.Clear();

            FindApplication(appName);

            foreach (KeyValuePair<int, int> ds in devicesAndSessions)
            {
                LogHelper.Info(null, "Muting application {0}, {1}", ds.Key, ds.Value);
                devCol[ds.Key].AudioSessionManager2.Sessions[ds.Value].SimpleAudioVolume.Mute = !devCol[ds.Key].AudioSessionManager2.Sessions[ds.Value].SimpleAudioVolume.Mute;
            }
        }

        public static void VolumeUp(string appName)
        {
            devicesAndSessions.Clear();

            FindApplication(appName);

            foreach (KeyValuePair<int, int> ds in devicesAndSessions)
            {
                if (devCol[ds.Key].AudioSessionManager2.Sessions[ds.Value].SimpleAudioVolume.MasterVolume == 1f) break;
                if (devCol[ds.Key].AudioSessionManager2.Sessions[ds.Value].SimpleAudioVolume.MasterVolume > 0.9f)
                    devCol[ds.Key].AudioSessionManager2.Sessions[ds.Value].SimpleAudioVolume.MasterVolume = 1f;
                else
                    devCol[ds.Key].AudioSessionManager2.Sessions[ds.Value].SimpleAudioVolume.MasterVolume += 0.1f;
            }
        }

        public static void VolumeDown(string appName)
        {
            devicesAndSessions.Clear();

            FindApplication(appName);

            foreach (KeyValuePair<int, int> ds in devicesAndSessions)
            {
                if (devCol[ds.Key].AudioSessionManager2.Sessions[ds.Value].SimpleAudioVolume.MasterVolume == 0f) break;
                if (devCol[ds.Key].AudioSessionManager2.Sessions[ds.Value].SimpleAudioVolume.MasterVolume < 0.1f)
                    devCol[ds.Key].AudioSessionManager2.Sessions[ds.Value].SimpleAudioVolume.MasterVolume = 0.0f;
                else
                    devCol[ds.Key].AudioSessionManager2.Sessions[ds.Value].SimpleAudioVolume.MasterVolume -= 0.1f;
            }
        }

        public static List<string> GetListOfApplicationsWithAudio()
        {
            List<string> results = new List<string>();

            EDataFlow flow = new EDataFlow();
            MMDeviceEnumerator deviceEnum = new MMDeviceEnumerator();

            devCol = deviceEnum.EnumerateAudioEndPoints(flow, DEVICE_STATE.DEVICE_STATE_ACTIVE);

            for (int devIdx = 0; devIdx < devCol.Count; devIdx++)
            {
                for (int sesIdx = 0; sesIdx < devCol[devIdx].AudioSessionManager2.Sessions.Count; sesIdx++)
                {
                    results.Add(devCol[devIdx].AudioSessionManager2.Sessions[sesIdx].GetSessionIdentifier);
                }
            }

            return results;
        }

        private static void FindApplication(string appName)
        {
            LogHelper.Debug(null, "Finding any mixers that contain the word '{0}'", appName);

            EDataFlow flow = new EDataFlow();
            MMDeviceEnumerator deviceEnum = new MMDeviceEnumerator();

            devCol = deviceEnum.EnumerateAudioEndPoints(flow, DEVICE_STATE.DEVICE_STATE_ACTIVE);

            for (int devIdx = 0; devIdx < devCol.Count; devIdx++)
            {
                for (int sesIdx = 0; sesIdx < devCol[devIdx].AudioSessionManager2.Sessions.Count; sesIdx++)
                {
                    if (devCol[devIdx].AudioSessionManager2.Sessions[sesIdx].GetSessionIdentifier.Contains(appName))
                    {
                        LogHelper.Debug(null, "Found {0}", devCol[devIdx].AudioSessionManager2.Sessions[sesIdx].GetSessionIdentifier);

                        devicesAndSessions.Add(new KeyValuePair<int, int>(devIdx, sesIdx));
                    }
                }
            }
        }
    }
}
