using System.Collections.Generic;
using CorePatterns.ServiceLocator;
using DVDNights;
using UnityEngine;

public class DisksController : MonoBehaviour, IDisksController
{
    private Dictionary<DiskType, List<IBouncerDisk>> _registeredDisks;
    private List<IBouncerDisk> _allRegisteredDisks;
    private IPointsController _pointsController;
    private IDiskLevelController _diskLevelController;

    private void Awake()
    {
        InstallService();
    }

    private void InstallService()
    {
        _allRegisteredDisks = new List<IBouncerDisk>();
        _registeredDisks = new Dictionary<DiskType, List<IBouncerDisk>>
        {
            { DiskType.WHITE, new List<IBouncerDisk>() },
            { DiskType.CYAN, new List<IBouncerDisk>() },
            { DiskType.YELLOW, new List<IBouncerDisk>() },
            { DiskType.RED, new List<IBouncerDisk>() },
            { DiskType.GREEN, new List<IBouncerDisk>() },
            { DiskType.MAGENTA, new List<IBouncerDisk>() },
            { DiskType.GOLD, new List<IBouncerDisk>() }
        };

        ServiceLocator.RegisterService<IDisksController>(this);
    }

    private void Start()
    {
        _diskLevelController = ServiceLocator.GetService<IDiskLevelController>();
    }


    public void AddDisk(IBouncerDisk diskToAdd)
    {
        DiskDataSO diskData = diskToAdd.DiskDataSO;
        DiskType diskType = diskData.DiskType;
        List<IBouncerDisk> existingDisks = _registeredDisks[diskType];

        if (existingDisks.Contains(diskToAdd))
        {
            return;
        }
        
        _registeredDisks[diskType].Add(diskToAdd);
        _registeredDisks[diskType] = existingDisks;
        _allRegisteredDisks.Add(diskToAdd);

        _pointsController ??= ServiceLocator.GetService<IPointsController>();
        
        _pointsController.RegisterBouncingDisk(diskToAdd);
    }

    public void RemoveDisksByQuantity(DiskType diskTypeToRemove, int quantity)
    {
        List<IBouncerDisk> existingDisks = _registeredDisks[diskTypeToRemove];

        if (existingDisks.Count < quantity)
        {
            return;
        }
        
        foreach (IBouncerDisk existingDisk in existingDisks)
        {
            _allRegisteredDisks.Remove(existingDisk);
            _registeredDisks[diskTypeToRemove].Remove(existingDisk);
            _pointsController.UnregisterBouncingDisk(existingDisk);
            existingDisk.DestroyDisk();
        }
    }

    public void UpdateAllDisks()
    {
        int updatedSpeed = GameProgression.DiscBaseSpeed * (int)GameProgression.GetSpeedBonusMult(_diskLevelController.DiskSpeedBonusLevel);
        
        foreach (IBouncerDisk existingDisk in _allRegisteredDisks)
        {
            existingDisk.BaseSpeed = updatedSpeed;
        }
    }
}

public interface IDisksController
{
    public void AddDisk(IBouncerDisk diskToAdd);
    public void RemoveDisksByQuantity(DiskType diskTypeToRemove, int quantity);
    public void UpdateAllDisks();
}
