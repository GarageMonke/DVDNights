using System;
using System.Collections.Generic;
using System.Linq;
using CorePatterns.Managers;
using CorePatterns.ServiceLocator;
using DG.Tweening;
using DVDNights;
using UnityEngine;

public class DisksController : MonoBehaviour, IDisksController
{
    [Header("References")] 
    [SerializeField] private Transform bounceArea;
    [SerializeField] private CanvasGroup whiteFlashCanvasGroup;

    [Header("Feedback")] 
    [SerializeField] private AudioClip flashAudioClip;
    [SerializeField] private AudioClip mergeAudioClip;
    
    
    private Dictionary<DiskType, List<IBouncerDisk>> _registeredDisks;
    private List<IBouncerDisk> _allRegisteredDisks;
    private IPointsController _pointsController;
    private IDiskLevelController _diskLevelController;
    private int _disksRegistered;
    private DiskType[]  _mergeOrder;
    private IDiskFactory _diskFactory;
    private IShopController _shopController;
    private Sequence _mergeSequence;

    public int DisksRegistered => _disksRegistered;
    public Action OnGoldDiskCreated { get; set; }
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
        
        _shopController = ServiceLocator.GetService<IShopController>();
        _shopController.OnShopOpened += StopAllDisksMoving;
        _shopController.OnShopClosed += CheckDisksToMerge;
        
        //GameStart here should be loaded the current disks
        CreateDisk(DiskType.WHITE);
        CreateDisk(DiskType.WHITE);
        
        CreateDisk(DiskType.MAGENTA);
        CreateDisk(DiskType.MAGENTA);
        CreateDisk(DiskType.GREEN);
        CreateDisk(DiskType.GREEN);
        CreateDisk(DiskType.GREEN);
        CreateDisk(DiskType.YELLOW);
        ResumeAllDisksMoving();
        CheckDisksToMerge();
    }

    public void CreateDisk(DiskType diskType)
    {
        IBouncerDisk createdDisk = _diskFactory.CreateDisk(diskType, bounceArea.position);
        AddDisk(createdDisk);
    }

    private void AddDisk(IBouncerDisk diskToAdd)
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
    }
    
    private void RemoveDisksByQuantity(DiskType diskTypeToRemove, int quantity)
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
    
    private void RemoveAllDisks()
    {
        foreach (IBouncerDisk disk in _allRegisteredDisks)
        {
            _pointsController.UnregisterBouncingDisk(disk);
            disk.DestroyDisk();
        }
        
        _allRegisteredDisks.Clear();
    }

    public void BoostAllDisksSpeed()
    {
        float updatedSpeed = GameProgression.DiscBaseSpeed * GameProgression.GetFFLevelMult(_diskLevelController.DiskFFMultLevel);
        
        foreach (IBouncerDisk existingDisk in _allRegisteredDisks)
        {
            existingDisk.BaseSpeed = updatedSpeed;
        }
    }

    public void ResetAllDisksSpeed()
    {
        foreach (IBouncerDisk existingDisk in _allRegisteredDisks)
        {
            existingDisk.BaseSpeed = GameProgression.DiscBaseSpeed;
        }
    }

    private void CheckDisksToMerge()
    {
        ResumeAllDisksMoving();
        
        foreach (DiskType diskType in _mergeOrder)
        {
            List<IBouncerDisk> disks = _registeredDisks[diskType];

            if (disks.Count < GameProgression.DiscMergeAmount)
            {
                continue;
            }
            
            List<IBouncerDisk> disksToMerge = disks
                .Take(GameProgression.DiscMergeAmount)
                .ToList();

            DiskType nextTier = GetNextTier(diskType);
            
            if (nextTier == DiskType.GOLD)
            {
                PlayGoldenMergeAnimation();
            }
            else
            {
                PlayMergeAnimation(disksToMerge, diskType, nextTier);
            }
           
            return;
        }
    }
    
    private void PlayGoldenMergeAnimation()
    {
        _mergeSequence?.Kill();
        Vector3 centerPos = bounceArea.position;
        float duration = 2f;
        int completed = 0;
        
        StopDisksMoving(_allRegisteredDisks);

        foreach (IBouncerDisk disk in _allRegisteredDisks)
        {
            Transform diskTransform = disk.Transform;

            diskTransform
                .DOMove(centerPos, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    completed++;

                    if (completed < _allRegisteredDisks.Count)
                    {
                        return;
                    }
                    
                    AudioManager.Instance.PlaySFX(flashAudioClip, 0.25f);
                    float flashLength = flashAudioClip.length / 2f;
                    
                    _mergeSequence = DOTween.Sequence()
                        .Append(whiteFlashCanvasGroup.DOFade(1f, flashLength))
                        .Append(whiteFlashCanvasGroup.DOFade(0f, flashLength))
                        .AppendCallback(() => 
                        {
                            AudioManager.Instance.PlaySFX(flashAudioClip, 0.25f);
                        })
                        .Append(whiteFlashCanvasGroup.DOFade(1f, flashLength))
                        .Append(whiteFlashCanvasGroup.DOFade(0f, flashLength))
                        .AppendCallback(() => 
                        {
                            AudioManager.Instance.PlaySFX(flashAudioClip, 0.25f);
                        })
                        .Append(whiteFlashCanvasGroup.DOFade(1f, flashLength))
                        .AppendCallback(() => 
                        {
                            RemoveAllDisks();
                            CreateDisk(DiskType.GOLD);
                        })
                        .Append(whiteFlashCanvasGroup.DOFade(0f, flashLength))
                        .OnComplete(() =>
                        {
                            AudioManager.Instance.PlaySFX(mergeAudioClip, 0.5f);
                            OnGoldDiskCreated?.Invoke();
                        });
                });
        }
    }

    private void PlayMergeAnimation(List<IBouncerDisk> disksToMerge, DiskType fromTier, DiskType nextTier)
    {
        _mergeSequence?.Kill();
        Vector3 centerPos = bounceArea.position;
        float duration = 2f;
        int completed = 0;
        
        StopDisksMoving(disksToMerge);

        foreach (IBouncerDisk disk in disksToMerge)
        {
            Transform diskTransform = disk.Transform;

            diskTransform
                .DOMove(centerPos, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    completed++;

                    if (completed < GameProgression.DiscMergeAmount)
                    {
                        return;
                    }
                    
                    AudioManager.Instance.PlaySFX(flashAudioClip, 0.25f);
                    float flashLength = flashAudioClip.length / 2f;
                    
                    _mergeSequence = DOTween.Sequence()
                        .Append(whiteFlashCanvasGroup.DOFade(1f, flashLength))
                        .Append(whiteFlashCanvasGroup.DOFade(0f, flashLength))
                        .AppendCallback(() => 
                        {
                            AudioManager.Instance.PlaySFX(flashAudioClip, 0.25f);
                        })
                        .Append(whiteFlashCanvasGroup.DOFade(1f, flashLength))
                        .Append(whiteFlashCanvasGroup.DOFade(0f, flashLength))
                        .AppendCallback(() => 
                        {
                            AudioManager.Instance.PlaySFX(flashAudioClip, 0.25f);
                        })
                        .Append(whiteFlashCanvasGroup.DOFade(1f, flashLength))
                        .AppendCallback(() => 
                        {
                            RemoveDisksByQuantity(fromTier, GameProgression.DiscMergeAmount);
                            CreateDisk(nextTier);
                            ResumeAllDisksMoving();
                        })
                        .Append(whiteFlashCanvasGroup.DOFade(0f, flashLength))
                        .OnComplete(() =>
                        {
                            AudioManager.Instance.PlaySFX(mergeAudioClip, 0.5f);
                            CheckDisksToMerge();
                        });
                });
        }
    }
    
    private void StopAllDisksMoving()
    {
        foreach (IBouncerDisk disk in _allRegisteredDisks)
        {
            disk.SetMoving(false);
        }
    }
    
    private void StopDisksMoving(List<IBouncerDisk> disksList)
    {
        foreach (IBouncerDisk disk in disksList)
        {
            disk.SetMoving(false);
        }
    }
    
    private void ResumeAllDisksMoving()
    {
        foreach (IBouncerDisk disk in _allRegisteredDisks)
        {
            disk.SetMoving(true);
        }
    }
    
    private DiskType GetNextTier(DiskType current)
    {
        return (DiskType)((int)current + 1);
    }

    private void OnDestroy()
    {
        _shopController.OnShopOpened -= StopAllDisksMoving;
        _shopController.OnShopClosed -= CheckDisksToMerge;
    }
}

public interface IDisksController
{
    public int DisksRegistered { get; }
    public Action OnGoldDiskCreated { get; set; }
    public List<IBouncerDisk> AllRegisteredDisks { get; }
    public void CreateDisk(DiskType diskType);
    public void BoostAllDisksSpeed();
    public void ResetAllDisksSpeed();
}
