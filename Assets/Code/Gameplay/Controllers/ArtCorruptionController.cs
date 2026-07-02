using CorePatterns.Providers.Implementations;
using CorePatterns.ServiceLocator;
using TossBoss.Providers.Engine.Implementations.Materials;
using UnityEngine;

namespace DVDNights
{
    public class ArtCorruptionController : MonoBehaviour, IArtCorruptionController
    {
         [Header("References")]
         [SerializeField] private MaterialProviderHandler artMaterialProviderHandler;
         [SerializeField] private AudioClipProvider cleanCorruptionAudioProvider;

         private void Awake()
         {
             InstallService();
         }

         private void InstallService()
         {
             artMaterialProviderHandler.InitializeProviderHandler();
             cleanCorruptionAudioProvider.InitializeProvider();
             ServiceLocator.RegisterService<IArtCorruptionController>(this);
         }

         public Material GetArtMaterialByType(ArtPictureType artType)
         {
             return artMaterialProviderHandler.GetRandomElementById(artType);
         }

         public AudioClip GetAudioClip()
         {
             return cleanCorruptionAudioProvider.GetRandomElement();
         }
    }

    public interface IArtCorruptionController
    {
        public Material GetArtMaterialByType(ArtPictureType artType);
        public AudioClip GetAudioClip();
    }
}