using System.Collections.Generic;
using System.Linq;
using Data;
using Locator;
using UniRx;
using UnityEngine;

namespace Services
{
    public static class Utils
    {
        public static T GetRandom<T>(this List<T> array)
        {
            var index = UnityEngine.Random.Range(0, array.Count);
            return array[index];
        }
    }

    public class SoundService
    {
        private AudioSource _source;
        private readonly VisualData _data;

        private readonly Dictionary<string, List<AudioClip>> _cacheSelect = new Dictionary<string, List<AudioClip>>();
        private readonly Dictionary<string, List<AudioClip>> _cacheConfirm = new Dictionary<string, List<AudioClip>>();
        private readonly Dictionary<string, List<AudioClip>> _cacheMove = new Dictionary<string, List<AudioClip>>();
        private readonly Dictionary<string, List<AudioClip>> _cacheAttack = new Dictionary<string, List<AudioClip>>();
        public SoundService()
        {
            _data = Container.Get<VisualData>();
        }
        public void RegisterAudioSource(AudioSource audioSource)
        {
            _source = audioSource;
        }
        public void PlaySelect(string id)
        {
            Play(id, _cacheSelect, _data.SoundsSelect);
        }
        public void PlayConfirm(string id)
        {
            Play(id, _cacheConfirm, _data.SoundsConfirm);

        }
        public void PlayMove(string id)
        {
            Play(id, _cacheMove, _data.SoundsMove);

        }
        public void PlayAttack(string id)
        {
            Play(id, _cacheAttack, _data.SoundsAttack);
        }
        private void Play(string id,  Dictionary<string, List<AudioClip>> dict, List<ObjectEntry<AudioClip>> data )
        {
            if(data.Count == 0)
                return;

            if (!dict.ContainsKey(id))
            {
                dict.Add(id, new List<AudioClip>());
                var list = data.Where(x => x.Id == id);
                foreach (var objectEntry in list)
                {
                    dict[id].Add(objectEntry.Obj);
                }
            }

            var dataList = dict[id];
            if(dataList == null || dataList.Count == 0)
                return;

            var clip = dataList.GetRandom();
            _source.PlayOneShot(clip);
        }
    }
}