using NocturneThree.EventSystem;
using NocturneThree.ServiceLocator;
using UnityEngine;
using UnityEngine.Pool;

namespace ColorOrCrash.Features.TextFloat.Components
{
    public interface ITextFloat : IGameService
    {
        void Spawn(Vector3 position, string text);
    }

    [Service(typeof(ITextFloat))]
    public class TextFloatPool : MonoBehaviour, ITextFloat
    {
        [SerializeField] private TextFloat prefab;
        private IObjectPool<TextFloat> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<TextFloat>(
                () => Instantiate(prefab),
                ft => ft.gameObject.SetActive(true),
                ft => ft.gameObject.SetActive(false),
                ft => Destroy(ft.gameObject),
                true, 20, 100
            );
        }

        void Start()
        {
            EventBus.Subscribe<TextFloatEvent>(HandleSpawnRequest);
        }

        void OnDestroy()
        {
            EventBus.Unsubscribe<TextFloatEvent>(HandleSpawnRequest);
            ServiceLocator.Unregister<ITextFloat>();
        }

        private void HandleSpawnRequest(TextFloatEvent @event)
        {
            Spawn(@event.WorldPosition, @event.TextContent);
        }

        public void Spawn(Vector3 position, string text)
        {
            var ft = _pool.Get();
            ft.transform.position = position;
            ft.Setup(text, (obj) => _pool.Release(obj));
        }
    }
}
