﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TModules.Core;
using System.Text.RegularExpressions;
using SpotiFire;
using Analytics;
using System.Media;
using WitAI;

namespace TModules.DefaultModules
{
    public class SpotifyModule : TModule
    {
        NAudio.Wave.BufferedWaveProvider buffer;

        public byte[] appkey = {
	        0x01, 0x84, 0x39, 0xC7, 0x65, 0xA9, 0xA7, 0x93, 0xB0, 0xCD, 0x07, 0xED, 0xE7, 0xFA, 0x56, 0xD6,
	        0xB4, 0xA8, 0x72, 0x21, 0x56, 0xBB, 0xD7, 0xC3, 0x72, 0x60, 0x31, 0xD4, 0x72, 0xA2, 0xEC, 0x6C,
	        0x3C, 0x46, 0x86, 0x35, 0xC6, 0xE4, 0xEC, 0x9D, 0x1F, 0xB9, 0xEA, 0x66, 0xE0, 0x3B, 0x6E, 0x4F,
	        0xF8, 0x87, 0xD3, 0xB9, 0xCF, 0xD9, 0x13, 0xC6, 0xB5, 0x42, 0x15, 0x20, 0x32, 0xA6, 0xFD, 0x73,
	        0x4C, 0x80, 0x75, 0x64, 0x83, 0x7A, 0xDA, 0xCE, 0x0A, 0x00, 0x8C, 0xF8, 0x30, 0x41, 0x99, 0x87,
	        0x2D, 0x88, 0x2E, 0x52, 0xE3, 0x8D, 0xEB, 0x59, 0x16, 0xB0, 0x1A, 0xAB, 0x64, 0xF7, 0x76, 0xBC,
	        0x07, 0x3B, 0x56, 0x43, 0xED, 0xB2, 0x4A, 0xFB, 0xB6, 0x6E, 0x33, 0x3E, 0x38, 0x46, 0xF8, 0x66,
	        0x74, 0x02, 0xDC, 0xAD, 0x0B, 0xA9, 0x28, 0x09, 0xF7, 0x5B, 0x9A, 0x4E, 0xF9, 0x4E, 0xB7, 0x97,
	        0xD5, 0x41, 0x6F, 0x7D, 0xC2, 0x19, 0xFB, 0x33, 0x73, 0x7B, 0xBB, 0x65, 0x29, 0xEE, 0xE3, 0x4C,
	        0x94, 0x4A, 0x63, 0x33, 0xDD, 0x17, 0xA8, 0x50, 0xCE, 0x63, 0x49, 0x8C, 0x03, 0xF2, 0xBA, 0x94,
	        0x9B, 0xCA, 0x07, 0xAA, 0x36, 0x0B, 0x4A, 0x25, 0x48, 0xF2, 0x8C, 0x1B, 0x1A, 0x75, 0x5B, 0xBD,
	        0x14, 0xCD, 0x14, 0xE5, 0x53, 0x1A, 0x98, 0xD3, 0x01, 0xBE, 0xD4, 0xD4, 0xAC, 0x6E, 0x7B, 0x49,
	        0xA0, 0x04, 0xCA, 0x01, 0xDA, 0xD2, 0x05, 0x5D, 0x93, 0x6D, 0x30, 0x6D, 0xD8, 0x74, 0x41, 0x47,
	        0x43, 0x9C, 0xAD, 0xD3, 0xFD, 0xF0, 0x90, 0x2A, 0x1E, 0x21, 0x7C, 0xF7, 0x16, 0x77, 0xEF, 0x48,
	        0x9D, 0xA0, 0x02, 0xF5, 0x75, 0xF9, 0x5F, 0x64, 0x1B, 0x2F, 0x69, 0x9B, 0x4A, 0xEE, 0x21, 0x69,
	        0xC3, 0x88, 0x28, 0x83, 0xFC, 0x54, 0xCE, 0xC3, 0x0E, 0xD7, 0x2A, 0x4F, 0xC8, 0x90, 0x89, 0x6A,
	        0x79, 0xA8, 0x2E, 0x59, 0x81, 0xFE, 0x29, 0xB0, 0x24, 0x58, 0x58, 0xF4, 0xE5, 0x0E, 0x53, 0xDF,
	        0x2B, 0x28, 0xCD, 0x7B, 0xDA, 0xC9, 0x5E, 0xC9, 0x1F, 0xAE, 0x1E, 0xA7, 0x3C, 0x78, 0x4D, 0x35,
	        0x34, 0x31, 0x08, 0xBA, 0xFE, 0x6E, 0xBB, 0x35, 0xEC, 0x59, 0xD3, 0x3A, 0x4E, 0xEF, 0x0C, 0x7B,
	        0xFF, 0x51, 0x86, 0xDE, 0x40, 0xF1, 0x5F, 0x5A, 0xA3, 0xED, 0x93, 0x98, 0xC7, 0x4A, 0x01, 0x5B,
	        0x22,
        };

        private Session mCurrentSession;
        private PlaylistContainer _currentContainer;

        private Track mTrackToPlay;

        private bool _initialized = false;

        public SpotifyModule(ModuleManager manager)
            : base("Spotify", manager)
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            SetupSpotify();
        }

        async Task SetupSpotify()
        {
            Session session = await Spotify.CreateSession(appkey, "C:\\temp\\libspotify", "C:\\temp\\libspotify", "Tarynn");
            Error err = await session.Login(Host.RetrieveCachedFile("email"), Host.RetrieveCachedFile("password"), true);
            session.PreferredBitrate = BitRate.Bitrate320k;
            session.MusicDelivered += session_MusicDelivered;
            if (err != Error.OK)
            {
                _logger.Debug(err.Message());
            }
            mCurrentSession = session;

            await session.PlaylistContainer;
            while (session.PlaylistContainer.Playlists.Count < 1)
            {
                Console.WriteLine("Found {0} playlists, retrying in 2 sec.", session.PlaylistContainer.Playlists.Count);
                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            _currentContainer = await session.PlaylistContainer;

            Intents.Add("spotify", PlaySpotify);
            _initialized = true;
            _logger.InfoFormat("Spotify Module ready to go...");
        }

        void session_MusicDelivered(Session sender, MusicDeliveryEventArgs e)
        {
            if (buffer == null)
            {
                buffer = new NAudio.Wave.BufferedWaveProvider(new NAudio.Wave.WaveFormat(e.Rate, e.Channels));
                NAudio.Wave.DirectSoundOut dso = new NAudio.Wave.DirectSoundOut(NAudio.Wave.DirectSoundOut.DSDEVID_DefaultPlayback);
                dso.Init(buffer);
                dso.Play();
            }

            int space = buffer.BufferLength - buffer.BufferedBytes;
            if (space > e.Samples.Length)
            {
                buffer.AddSamples(e.Samples, 0, e.Samples.Length);
                e.ConsumedFrames = e.Frames;
            }
            else
            {
                e.ConsumedFrames = 0;
            }
        }

        private void ListPlaylists(WitOutcome outcome)
        {
            foreach (Playlist p in mCurrentSession.PlaylistContainer.Playlists)
            {
                Host.SpeakEventually(p.Name);
            }
        }

        public void PlayRandomTrack()
        {
            if (!_initialized)
                return;

            var track = RandomTrack().GetAwaiter().GetResult();
            var name = track.Artists.FirstOrDefault().Name ?? "Someone";

            Host.BlockingSpeak(string.Format("Playing track: {0} by {1}", track.Name, name));
            PlayTrack(track);            
        }

        private void PlaySpotify(WitOutcome outcome)
        {
            if (!_initialized)
                return;

            var track = RandomTrack().GetAwaiter().GetResult();
            var name = track.Artists.FirstOrDefault().Name ?? "Someone";

            Host.BlockingSpeak(string.Format("Playing track: {0} by {1}", track.Name, name));

            PlayTrack(track);

            _logger.InfoFormat("Playing track: {0} by {1}", track.Name, name);
        }

        private async Task<Track> RandomTrack()
        {
            int playListCounts = _currentContainer.Playlists.Count;
            int randomIndex = new Random().Next(playListCounts);
            var playlist = _currentContainer.Playlists[randomIndex];

            int trackCounts = playlist.Tracks.Count;
            randomIndex = new Random().Next(trackCounts);
            return await playlist.Tracks[randomIndex];
        }

        private void PlayTrack(Track t)
        {
            mCurrentSession.PlayerLoad(t);
            mCurrentSession.PlayerPlay();
        }
    }
}
