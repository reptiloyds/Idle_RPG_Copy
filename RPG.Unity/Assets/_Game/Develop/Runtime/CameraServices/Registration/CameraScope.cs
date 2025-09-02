using PleasantlyGames.RPG.Runtime.CameraServices.Contract;
using PleasantlyGames.RPG.Runtime.CameraServices.Model;
using PleasantlyGames.RPG.Runtime.DI.Attributes;
using PleasantlyGames.RPG.Runtime.DI.Base;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace PleasantlyGames.RPG.Runtime.CameraServices.Registration
{
    [DisallowMultipleComponent]
    public class CameraScope : AutoFeatureScope
    {
        [SerializeField, Required, AutoFill] private CameraProvider _cameraProvider;
        [SerializeField, Required, AutoFill] private CameraAnimator _cameraAnimator;
        [SerializeField, Required, AutoFill] private CameraDemonstrator _cameraDemonstrator;
        [SerializeField, Required, AutoFill] private VirtualCameraService _virtualCameraService;

        public override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            builder.RegisterComponent(_cameraProvider)
                .As<ICameraProvider>();
            builder.RegisterComponent(_cameraAnimator)
                .As<ICameraAnimator>();
            builder.RegisterComponent(_cameraDemonstrator)
                .As<ICameraDemonstrator>();
            builder.RegisterComponent(_virtualCameraService)
                .As<IVirtualCameraService>();
            
            builder.Register<CameraChaser>(Lifetime.Singleton)
                .AsImplementedInterfaces();
        }

        public override void AutoFill()
        {
            base.AutoFill();
            AddAutoInjectedObject<CameraProvider>();
        }
    }
}