using UnityEngine;
using UnityEngine.Rendering;

namespace CustomRenderPipeline
{
    public class SpaceRunPipelineRender : RenderPipeline
    {
        private CameraRendering _cameraRenderer = new CameraRendering();
        
        protected override void Render(ScriptableRenderContext context,
            Camera[] cameras)
        {
            CamerasRender(context, cameras);
        }
        private void CamerasRender(ScriptableRenderContext context, Camera[]cameras)
        {
            foreach (var camera in cameras)
            {
                _cameraRenderer.Render(context, camera);
            }
        }

    }
}
