using System.Collections.Generic;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace DVDNights
{
    public class OutlineController :  MonoBehaviour, IOutlineController
    {
        private List<Outline> _outlines;

        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            _outlines = new List<Outline>();
            ServiceLocator.RegisterService<IOutlineController>(this);
        }

        public void RegisterOutline(Outline outline)
        {
            if (!_outlines.Contains(outline))
            {
                _outlines.Add(outline);
            }
        }

        public void EnableAllOutlines()
        {
            foreach (Outline outline in _outlines)
            {
                outline.enabled = true;
            }
        }

        public void DisableAllOutlines()
        {
            foreach (Outline outline in _outlines)
            {
                outline.enabled = false;
            }
        }
    }

    public interface IOutlineController
    {
        public void RegisterOutline(Outline outline);
        public void EnableAllOutlines();
        public void DisableAllOutlines();
    }
}