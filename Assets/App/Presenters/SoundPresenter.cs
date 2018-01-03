using System;
using System.Collections.Generic;
using ModestTree;
using UniRx;
using UnityEngine;

namespace Presenters {
	public class SoundPresenter : MVCPresenter2D {
		public double QueueProcessSpeed = 25;
		private AudioSource _audioSource;
		private Queue<AudioClip> AudioQueue = new Queue<AudioClip>();
		
		public void QueueAudio(params AudioClip[] audioClips) => audioClips.ForEach(x => AudioQueue.Enqueue(x));

		public void Start() {
			_audioSource = GetComponent<AudioSource>();
			Observable.EveryUpdate().Buffer(TimeSpan.FromMilliseconds(QueueProcessSpeed)).Subscribe(x => {
				try {
					var clip = AudioQueue.Dequeue();
					_audioSource.PlayOneShot(clip);
				} catch (InvalidOperationException e) {}
			});
		}
	}
}