using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Game
{
    public class UnitFactory : MonoBehaviour
    {
        private uint _sequence = 0;
        
        public Unit CreateNewUnit(int unitOriginID, UnitTable unitTable, KG.Map map, Player owner, Unit.Listener listener, Transform parent)
        {
            var unitOrigin = unitTable.GetOriginUnit(unitOriginID);
            var unit = Object.Instantiate(unitOrigin, parent);
            unit.Init(_sequence++, map, owner, listener);
            
            var frameUpdateDisposable = GameController.updateFrameStream.Subscribe(unit.OnUpdateFrame);
            unit.OnDestroyAsObservable().Subscribe(_ => frameUpdateDisposable.Dispose());
            
            return unit;
        }
    }
}