using CorePatterns.Providers.Implementations;
using CorePatterns.ServiceLocator;
using TossBoss.Providers.Engine.Implementations.Materials;
using UnityEngine;

namespace Rulebound
{
    public class ArtCorruptionController : MonoBehaviour, IArtCorruptionController
    {
         [Header("References")]
         [SerializeField] private MaterialProviderHandler artMaterialProviderHandler;
         [SerializeField] private AudioClipProvider cleanCorruptionAudioProvider;
         [SerializeField] private AudioClipProvider drawingCorruptionAudioProvider;

         private void Awake()
         {
             InstallService();
         }

         private void InstallService()
         {
             artMaterialProviderHandler.InitializeProviderHandler();
             cleanCorruptionAudioProvider.InitializeProvider();
             drawingCorruptionAudioProvider.InitializeProvider();
             ServiceLocator.RegisterService<IArtCorruptionController>(this);
         }

         public Material GetArtMaterialByType(ArtPictureType artType)
         {
             return artMaterialProviderHandler.GetRandomElementById(artType);
         }

         public AudioClip GetCleanCorruptionAudioClip()
         {
             return cleanCorruptionAudioProvider.GetRandomElement();
         }

         public AudioClip GetDrawingAudioClip()
         {
             return drawingCorruptionAudioProvider.GetRandomElement();
         }
    }

    public interface IArtCorruptionController
    {
        public Material GetArtMaterialByType(ArtPictureType artType);
        public AudioClip GetCleanCorruptionAudioClip();
        public AudioClip GetDrawingAudioClip();
    }
}