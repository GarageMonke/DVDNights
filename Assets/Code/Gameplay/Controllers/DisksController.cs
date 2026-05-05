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
    private int _disksRegistered;
    private DiskType[]  _mergeOrder;
    private IDiskFactory _diskFactory;

    public int DisksRegistered => _disksRegistered;
    public List<IBouncerDisk> AllRegisteredDisks => _allRegisteredDisks;

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
            { DiskType.ORANGE , new List<IBouncerDisk>()},
            { DiskType.GREEN, new List<IBouncerDisk>() },
            { DiskType.MAGENTA, new List<IBouncerDisk>() },
            { DiskType.GOLD, new List<IBouncerDisk>() }
        };
        
        _mergeOrder = new DiskType[]{
            DiskType.WHITE,
            DiskType.CYAN,
            DiskType.YELLOW,
            DiskType.ORANGE,
            DiskType.RED,
            DiskType.GREEN,
            DiskType.MAGENTA
        };

        ServiceLocator.RegisterService<IDisksController>(this);
    }

    private void Start()
    {
        _diskLevelController = ServiceLocator.GetService<IDiskLevelController>();
        _diskFactory = ServiceLocator.GetService<IDiskFactory>();
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
        _disksRegistered++;
        
        CheckDisksToMerge();
    }
    
    public void RemoveDisksByQuantity(DiskType diskTypeToRemove, int quantity)
    {
        List<IBouncerDisk> existingDisks = _registeredDisks[diskTypeToRemove];

        if (existingDisks.Count < quantity)
        {
            return;
        }
        
        List<IBouncerDisk> toRemove = existingDisks.GetRange(0, quantity);

        foreach (IBouncerDisk disk in toRemove)
        {
            _allRegisteredDisks.Remove(disk);
            _pointsController.UnregisterBouncingDisk(disk);
            disk.DestroyDisk();
        }

        existingDisks.RemoveRange(0, quantity);
    }

    public void UpdateAllDisks()
    {
        int updatedSpeed = (int)(GameProgression.DiscBaseSpeed * GameProgression.GetSpeedBonusMult(_diskLevelController.DiskSpeedBonusLevel));
        
        foreach (IBouncerDisk existingDisk in _allRegisteredDisks)
        {
            existingDisk.BaseSpeed = updatedSpeed;
        }
    }

    private void CheckDisksToMerge()
    {
        foreach (DiskType diskType in _mergeOrder)
        {
            List<IBouncerDisk> disks = _registeredDisks[diskType];

            if (disks.Count < 10)
            {
                continue;
            }
            
            DiskType nextTier = GetNextTier(diskType);
            RemoveDisksByQuantity(diskType, 10);
            _diskFactory.CreateDisk(nextTier);
            return;
        }
    }
    
    private DiskType GetNextTier(DiskType current)
    {
        return (DiskType)((int)current + 1);
    }

}

public interface IDisksController
{
    public int DisksRegistered { get; }
    public List<IBouncerDisk> AllRegisteredDisks { get; }
    public void AddDisk(IBouncerDisk diskToAdd);
    public void RemoveDisksByQuantity(DiskType diskTypeToRemove, int quantity);
    public void UpdateAllDisks();
}
