using Assets.Scripts.GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Analytics
{
    public struct WaterAbilityData : ITimedData<WaterAbilityEvent>
    {
        public int GetSnapshotIndex()
        {
            throw new NotImplementedException();
        }

        public void Init(int snapshotIndex)
        {
            throw new NotImplementedException();
        }

        public void ProcessEvent(WaterAbilityEvent e)
        {
            throw new NotImplementedException();
        }
    }
}
