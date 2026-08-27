using System.Collections.Generic;
using CorePatterns.ServiceLocator;
using UnityEngine;

namespace Rulebound
{
    public class PlatformController : MonoBehaviour, IPlatformController
    {
        private List<IPlatform> _platforms;
        
        private void Awake()
        {
            InstallService();
        }

        private void InstallService()
        {
            _platforms = new List<IPlatform>();
            ServiceLocator.RegisterService<IPlatformController>(this);
        }
        
        public void RegisterPlatform(IPlatform platformToRegister)
        {
            if (_platforms.Contains(platformToRegister))
            {
                return;
            }
            
            _platforms.Add(platformToRegister);
        }

        public void EnableAllPlatforms()
        {
            foreach (IPlatform platform in _platforms)
            {
                platform.EnablePlatform();
            }
        }

        public void DisableAllPlatforms()
        {
            foreach (IPlatform platform in _platforms)
            {
                platform.DisablePlatform();
            }
        }
    }

    public interface IPlatformController
    {
        public void RegisterPlatform(IPlatform platformToRegister);
        public void EnableAllPlatforms();
        public void DisableAllPlatforms();
    }
}