using System.Collections.Generic;
using CorePatterns.ServiceLocator;
using DVDNights;
using UnityEngine;

public class DisksController : MonoBehaviour, IDisksController
{
    private Dictionary<DiskType, List<IBouncerDisk>> _registeredDisks;
    private IScoreController _scoreController;

    private void Awake()
    {
        InstallService();
    }

    private void InstallService()
    {
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

        _scoreController ??= ServiceLocator.GetService<IScoreController>();
        
        _scoreController.RegisterBouncingDisk(diskToAdd);
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
            _registeredDisks[diskTypeToRemove].Remove(existingDisk);
            _scoreController.UnregisterBouncingDisk(existingDisk);
            existingDisk.DestroyDisk();
        }
    }
}

public interface IDisksController
{
    public void AddDisk(IBouncerDisk diskToAdd);
    public void RemoveDisksByQuantity(DiskType diskTypeToRemove, int quantity);
}
