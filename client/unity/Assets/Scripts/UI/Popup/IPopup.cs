using Cysharp.Threading.Tasks;

namespace UI
{
    // 일단 구현 제한 용으로만 사용함
    public interface IPopup<T>
    {
        void SetActive(bool active);
        UniTask<T> Response();
    }
}