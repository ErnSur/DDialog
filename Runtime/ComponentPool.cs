namespace Doublsb.Dialog
{
    using System.Collections.Generic;
    using UnityEngine;

    internal sealed class ComponentPool<T> where T : Component
    {
        private readonly GameObject _gameObject;

        private readonly int _startSize;

        private readonly Stack<T> _available = new Stack<T>();
        private readonly HashSet<T> _rented = new HashSet<T>();

        public ComponentPool(GameObject gameObject, int size)
        {
            _gameObject = gameObject;
            _startSize = size;
            Initialize();
        }

        public int CountAll => CountRented + CountAvailable;
        public int CountRented => _rented.Count;
        public int CountAvailable => _available.Count;

        public void Initialize()
        {
            for (var i = 0; i < _startSize; i++)
                _available.Push(CreateObject());
        }

        public T Rent()
        {
            var obj = _available.Count > 0 ? _available.Pop() : CreateObject();
            _rented.Add(obj);
            if (obj is Behaviour behaviour)
                behaviour.enabled = true;
            return obj;
        }

        public void Return(T obj)
        {
            if (_available.Contains(obj))
            {
                Debug.LogWarning("Trying to return already released object");
                return;
            }

            _rented.Remove(obj);

            if (obj is Behaviour behaviour)
                behaviour.enabled = false;

            _available.Push(obj);
        }

        public void ReturnAll()
        {
            foreach (var obj in _rented)
                Return(obj);
        }

        private T CreateObject()
        {
            var newObject = _gameObject.AddComponent<T>();
            if (newObject is Behaviour behaviour)
                behaviour.enabled = false;
            return newObject;
        }
    }
}