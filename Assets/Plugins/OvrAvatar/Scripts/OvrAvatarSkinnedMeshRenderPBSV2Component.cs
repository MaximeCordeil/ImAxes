using UnityEngine;
using System.Collections;
using System;
using Oculus.Avatar;

public class OvrAvatarSkinnedMeshPBSV2RenderComponent : OvrAvatarRenderComponent
{
    Shader surface;
    bool previouslyActive = false;
        
    internal void Initialize(ovrAvatarRenderPart_SkinnedMeshRenderPBS_V2 skinnedMeshRender, Shader surface, int thirdPersonLayer, int firstPersonLayer, int sortOrder)
    {
        this.surface = surface != null ? surface : Shader.Find("OvrAvatar/AvatarSurfaceShaderPBSV2");
        this.mesh = CreateSkinnedMesh(skinnedMeshRender.meshAssetID, skinnedMeshRender.visibilityMask, thirdPersonLayer, firstPersonLayer, sortOrder);
        bones = mesh.bones;
        UpdateMeshMaterial(skinnedMeshRender.visibilityMask, mesh);
    }

    public void UpdateSkinnedMeshRender(OvrAvatarComponent component, OvrAvatar avatar, IntPtr renderPart)
    {
        ovrAvatarVisibilityFlags visibilityMask = CAPI.ovrAvatarSkinnedMeshRenderPBSV2_GetVisibilityMask(renderPart);
        ovrAvatarTransform localTransform = CAPI.ovrAvatarSkinnedMeshRenderPBSV2_GetTransform(renderPart);
        UpdateSkinnedMesh(avatar, bones, localTransform, visibilityMask, renderPart);

        UpdateMeshMaterial(visibilityMask, mesh == null ? component.RootMeshComponent : mesh);
        bool isActive = this.gameObject.activeSelf;

        if( mesh != null )
        {
            bool changedMaterial = CAPI.ovrAvatarSkinnedMeshRenderPBSV2_MaterialStateChanged(renderPart);
            if (changedMaterial || (!previouslyActive && isActive))
            {
                ovrAvatarPBSMaterialState materialState = CAPI.ovrAvatarSkinnedMeshRenderPBSV2_GetPBSMaterialState(renderPart);

                Material mat = mesh.sharedMaterial;
                mat.SetVector("_AlbedoMultiplier", materialState.albedoMultiplier);
                mat.SetTexture("_Albedo", OvrAvatarComponent.GetLoadedTexture(materialState.albedoTextureID));
                mat.SetTexture("_Metallicness", OvrAvatarComponent.GetLoadedTexture(materialState.metallicnessTextureID));
                mat.SetFloat("_GlossinessScale", materialState.glossinessScale);
            }
        }
        previouslyActive = isActive;
    }

    private void UpdateMeshMaterial(ovrAvatarVisibilityFlags visibilityMask, SkinnedMeshRenderer rootMesh)
    {
        if (rootMesh.sharedMaterial == null || rootMesh.sharedMaterial.shader != surface)
        {
            rootMesh.sharedMaterial = CreateAvatarMaterial(gameObject.name + "_material", surface);
        }
    }
}
