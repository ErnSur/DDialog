namespace QuickEye.PeeDialog
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.UI;

    public class BasicDialogMenu : MonoBehaviour, IDialogMenuView
    {
        public event Action<int> OptionSelected;
        public event Action<string,int> MenuOptionSelected;

        [SerializeField]
        private GameObject selector;

        [SerializeField]
        private GameObject selectorItem;

        [SerializeField]
        private Text selectorItemText;

        private IReadOnlyList<string> _options;
        private int? _lastInput;
        
        public async UniTask<int> Open(IReadOnlyList<string> options)
        {
            _options = options;
            Clear();

            if (_options.Count > 0)
            {
                selector.SetActive(true);

                for (int i = 0; i < _options.Count; i++)
                {
                    AddMenuOption(i);
                }
            }
            else
                selector.SetActive(false);
            await UniTask.WaitUntil(() => _lastInput.HasValue);
            return _lastInput!.Value;
        }
        
        public UniTask Close()
        {
            selector.SetActive(false);
            return UniTask.CompletedTask;
        }

        private void Clear()
        {
            for (int i = 1; i < selector.transform.childCount; i++)
            {
                Destroy(selector.transform.GetChild(i).gameObject);
            }
        }

        private void AddMenuOption(int index)
        {
            selectorItemText.text = _options[index];

            var newItem = Instantiate(selectorItem, selector.transform);
            newItem.GetComponent<Button>().onClick.AddListener(() => Select(index));
            newItem.SetActive(true);
        }

        private void Select(int index)
        {
            _lastInput = index;
            OptionSelected?.Invoke(index);
        }
    }
}