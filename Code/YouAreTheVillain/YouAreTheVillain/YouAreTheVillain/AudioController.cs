using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YouAreTheVillain
{
    static class AudioController
    {
        static Dictionary<string, SoundEffect> SFX = new Dictionary<string, SoundEffect>();

        public static void LoadContent(ContentManager content)
        {
            SFX.Add("herojump", content.Load<SoundEffect>("sfx/herojump"));
        }

        public static void PlaySFX(string key)
        {
            PlaySFX(key, 0f, 1f, 0f);
        }
        public static void PlaySFX(string key, float pitch)
        {
            PlaySFX(key, pitch, 1f, 0f);
        }
        public static void PlaySFX(string key, float pitch, float volume)
        {
            PlaySFX(key, pitch, volume, 0f);
        }
        public static void PlaySFX(string key, float pitch, float volume, float pan)
        {
            SFX[key].Play(volume, pitch, pan);
        }
    }
}
