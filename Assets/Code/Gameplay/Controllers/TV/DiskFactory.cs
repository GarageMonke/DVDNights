using CorePatterns.Providers.Implementations;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class DiskFactory : MonoBehaviour, IDiskFactory
    {
        [Header("References")] 
        [SerializeField] private DiskDataProvider diskDataProvider;
        [SerializeField] private Transform diskOrigin;
        [Header("Disk-Prefabs")]
        [SerializeField] private BouncerDisk diskPrefab;

        private int _diskIndex;
        
        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            diskDataProvider.InitializeProvider();
            
            ServiceLocator.RegisterService<IDiskFactory>(this);
        }

        public IBouncerDisk CreateDisk(DiskType type, Vector3 position)
        {
            IBouncerDisk instantiatedDisk = Instantiate(diskPrefab, diskOrigin);
            _diskIndex++;
            instantiatedDisk.Transform.SetPositionAndRotation(position, Quaternion.identity);
            DiskDataSO diskDataSO = diskDataProvider.GetElementById(type.ToString());
            instantiatedDisk.InitializeDisk(diskDataSO, diskOrigin, _diskIndex);
            return instantiatedDisk;
        }
    }

    public interface IDiskFactory
    {
        public IBouncerDisk CreateDisk(DiskType type, Vector3 position);
    }
}