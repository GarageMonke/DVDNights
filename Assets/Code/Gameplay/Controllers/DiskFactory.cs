using System.Collections.Generic;
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
        
        private IDisksController _disksController;

        private void Awake()
        {
            InstallService();
        }

        private void Start()
        {
            _disksController = ServiceLocator.GetService<IDisksController>();
            CreateDisk(DiskType.WHITE);
        }

        private void InstallService()
        {
            diskDataProvider.InitializeProvider();
            
            ServiceLocator.RegisterService<IDiskFactory>(this);
        }

        public void CreateDisk(DiskType type)
        {
            IBouncerDisk instantiatedDisk = Instantiate(diskPrefab, diskOrigin);
            DiskDataSO diskDataSO = diskDataProvider.GetElementById(type.ToString());
            instantiatedDisk?.InitializeDisk(diskDataSO, diskOrigin);
            _disksController.AddDisk(instantiatedDisk);
        }
    }

    public interface IDiskFactory
    {
        public void CreateDisk(DiskType type);
    }
}