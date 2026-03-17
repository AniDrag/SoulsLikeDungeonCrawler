using System.Collections.Generic;
using AniDrag.Core;
using UnityEngine;

namespace AniDrag.Core
{
    public class WorldTickService : IWorldTickService
    { private List<ITickable> _tickables = new List<ITickable>();
        private List<ITickable> _pendingAdd = new List<ITickable>();
        private List<ITickable> _pendingRemove = new List<ITickable>();
        private bool _isTicking = false;

        private float _tickInterval = 0.5f; // Default; can be changed
        private float _timer = 0f;

        public float TickInterval 
        { 
            get => _tickInterval;
            set => _tickInterval = Mathf.Max(0.01f, value);
        }

        public void Register(ITickable tickable)
        {
            if (_isTicking)
                _pendingAdd.Add(tickable);
            else if (!_tickables.Contains(tickable))
                _tickables.Add(tickable);
        }

        public void Unregister(ITickable tickable)
        {
            if (_isTicking)
                _pendingRemove.Add(tickable);
            else
                _tickables.Remove(tickable);
        }

        public void Tick(float deltaTime)
        {
            _timer += deltaTime;
            while (_timer >= _tickInterval)
            {
                _timer -= _tickInterval;
                ProcessTick();
            }
        }

        private void ProcessTick()
        {
            _isTicking = true;

            foreach (var tickable in _tickables)
            {
                try { tickable.OnWorldTick(); }
                catch (System.Exception e) { Debug.LogException(e); }
            }

            _isTicking = false;

            // Apply pending registrations
            if (_pendingAdd.Count > 0)
            {
                _tickables.AddRange(_pendingAdd);
                _pendingAdd.Clear();
            }

            // Apply pending unregistrations
            if (_pendingRemove.Count > 0)
            {
                foreach (var t in _pendingRemove)
                    _tickables.Remove(t);
                _pendingRemove.Clear();
            }
        }
    }
}