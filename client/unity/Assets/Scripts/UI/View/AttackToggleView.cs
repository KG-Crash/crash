using System.Linq;
using KG;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    [System.Serializable]
    public class AttackTargetChangeEvent : UnityEvent<int?> { }

    public class AttackToggleView : KG.UIComponent, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private AttackTargetChangeEvent _attackTargetChangeEvent;

        private AttackTargetView[] _indicators;
        private AttackTargetView _target;
        
        private int _targetCount;
        private int? _attackTarget;
        private float _paddedRadius;
        private float _touchRadius;

        public AttackTargetChangeEvent attackTargetChange => _attackTargetChangeEvent;

        public void Initialize(int targetCount)
        {
            _targetCount = targetCount;
            
            _target = InstantiateAttackTargetView(transform);
            _target.attackTarget = null;
            _target.indicator = false;
            _target.rectTransform.localPosition = CalcTargetPosition(null);

            var maxCount = 3;
            var targetViewWidth = _target.rectTransform.rect.width * maxCount;
            var remainMargins = (rectTransform.rect.width - targetViewWidth) / (maxCount + 1); 
            
            _paddedRadius = rectTransform.rect.width / 4 + remainMargins;
            _touchRadius = rectTransform.rect.width / 4;
            
            _indicators =
                Enumerable.Range(0, targetCount).Select(index =>
                {
                    var attackTargetView = InstantiateAttackTargetView(transform);
                    attackTargetView.attackTarget = index + 1;
                    attackTargetView.indicator = true;
                    attackTargetView.rectTransform.localPosition = CalcTargetIndexPosition(index);
                    return attackTargetView;
                }).ToArray();

            _target.transform.SetAsLastSibling();
        }

        // TODO[:shkim] = 팩토리 패턴으로 변경
        public AttackTargetView InstantiateAttackTargetView(Transform parent)
        {
            var path = $"UI/{EntryPoint.uiBundleName}/{nameof(AttackTargetView)}";
            var prefab = Resources.Load<AttackTargetView>(path);
            return Instantiate<AttackTargetView>(prefab, parent);
        }
        
        public void OnBeginDrag(PointerEventData eventData) { }

        public void OnDrag(PointerEventData eventData)
        {
            _target.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var rect = rectTransform.rect;
            
            _attackTarget = CalcAttackTarget(rect.center, eventData.position - (Vector2)transform.position, _touchRadius, _targetCount);
            if (_attackTarget != null)
                _attackTarget = Mathf.Clamp(_attackTarget.Value, 1, _targetCount);
            
            _target.attackTarget = _attackTarget;
            _target.transform.localPosition = CalcTargetPosition(_attackTarget);
            _attackTargetChangeEvent.Invoke(_attackTarget);
        }

        private Vector2 CalcTargetPosition(int? targetCount)
        {
            return targetCount == null ? rectTransform.rect.center : CalcTargetIndexPosition(targetCount.Value - 1);
        }

        private Vector2 CalcTargetIndexPosition(int targetIndex)
        {
            var rect = rectTransform.rect;
            return CalcTargetIndicatorPosition(rect.center, _paddedRadius, targetIndex, _targetCount);
        }

        private static Vector2 CalcTargetIndicatorPosition(Vector2 center, float radius, int targetIndex, int targetCount)
        {
            var stepRadian = (Mathf.PI * 2 / targetCount);
            var radian = (float) targetIndex * stepRadian;
            var value = center + new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * radius;
            return value;
        }

        private static int? CalcAttackTarget(Vector2 center, Vector2 endDrag, float limitRadius, int targetCount)
        {
            var positionDelta = center - endDrag;

            if (positionDelta.sqrMagnitude < limitRadius * limitRadius)
            {
                return null;
            }
            else
            {
                var dragRadian = Mathf.Atan2(positionDelta.y, positionDelta.x) + Mathf.PI;
                var stepRadian = (Mathf.PI * 2) / targetCount;
                var index = Mathf.RoundToInt(dragRadian / stepRadian);

                return index + 1;
            }
        }
    }
}