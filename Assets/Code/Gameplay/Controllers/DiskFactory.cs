using System;
using System.Collections.Generic;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class DiskFactory : MonoBehaviour, IDiskFactory
    {
        [Header("References")] 
        [SerializeField] private Transform diskOrigin;
        [Header("Disk-Prefabs")]
        [SerializeField] private BouncerDisk whiteBouncerDiskPrefab;

        private Dictionary<DiskType, BouncerDisk> _disks;
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
            _disks = new Dictionary<DiskType, BouncerDisk>
            {
                { DiskType.WHITE, whiteBouncerDiskPrefab }
            };

            ServiceLocator.RegisterService<IDiskFactory>(this);
        }

        public void CreateDisk(DiskType type)
        {
            IBouncerDisk instantiatedDisk = null;
            switch (type)
            {
                case DiskType.WHITE:
                    instantiatedDisk = Instantiate(whiteBouncerDiskPrefab, diskOrigin);
                    break;
                case DiskType.CYAN:
                    break;
                case DiskType.YELLOW:
                    break;
                case DiskType.ORANGE:
                    break;
                case DiskType.RED:
                    break;
                case DiskType.GREEN:
                    break;
                case DiskType.MAGENTA:
                    break;
                case DiskType.GOLD:
                    break;
            }

            instantiatedDisk?.InitializeDisk(diskOrigin);
            _disksController.AddDisk(instantiatedDisk);
        }
    }

    public interface IDiskFactory
    {
        public void CreateDisk(DiskType type);
    }
}