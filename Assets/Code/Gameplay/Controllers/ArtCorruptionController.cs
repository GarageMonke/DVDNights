using CorePatterns.ServiceLocator;
using TossBoss.Providers.Engine.Implementations.Materials;
using UnityEngine;

namespace DVDNights
{
    public class ArtCorruptionController : MonoBehaviour, IArtCorruptionController
    {
         [Header("References")]
         [SerializeField] private MaterialProviderHandler artMaterialProviderHandler;

         private void Awake()
         {
             InstallService();
         }

         private void InstallService()
         {
             artMaterialProviderHandler.InitializeProviderHandler();
             ServiceLocator.RegisterService<IArtCorruptionController>(this);
         }

         public Material GetArtMaterialByType(ArtPictureType artType)
         {
             return artMaterialProviderHandler.GetRandomElementById(artType);
         }
    }

    public interface IArtCorruptionController
    {
        public Material GetArtMaterialByType(ArtPictureType artType);
    }
}