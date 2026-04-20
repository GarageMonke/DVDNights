using System.Collections.Generic;
using UnityEngine;

namespace CorePatterns.ObjectPooling
{
   public class PoolHandlerMonoInstaller<T> : MonoBehaviour where T : Behaviour
   {
      [SerializeField] protected List<PoolData<T>> pools;
      [SerializeField] protected Transform poolContainer;

      private IPoolHandler<T> _poolHandler;
      
      private void Awake()
      {
         _poolHandler = PopulatePools();
        ServiceLocator.ServiceLocator.RegisterService(_poolHandler);;
      }

      private IPoolHandler<T> PopulatePools()
      {
         IPoolHandler<T> poolHandler = new PoolHandler<T>(poolContainer);

         foreach (PoolData<T> poolData in pools)
         {
            poolHandler.CreatePool(poolData.PoolKey, poolData.PoolObject, poolData.PoolSize);
         }

         return poolHandler;
      }
   }
}