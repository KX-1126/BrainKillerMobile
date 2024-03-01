using UnityEngine;

namespace DataLoader
{
    public class VolumeObjectFactory : MonoBehaviour
    {
        public static GameObject createVolumeObject(Dataset3D dataset)
        {
            Debug.Log("start creating volume object");
            GameObject outerObject = new GameObject("VolumeRenderedObject_" + dataset.datasetName);

            GameObject meshContainer = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/VolumeContainer"), outerObject.transform);
            
            meshContainer.transform.localScale = Vector3.one;
            meshContainer.transform.localPosition = Vector3.zero;
            outerObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    
            MeshRenderer meshRenderer = meshContainer.GetComponent<MeshRenderer>();
            // meshRenderer.sharedMaterial = new Material(meshRenderer.sharedMaterial);

            // const int noiseDimX = 512;
            // const int noiseDimY = 512;
            // Texture2D noiseTexture = NoiseTextureGenerator.GenerateNoiseTexture(noiseDimX, noiseDimY);

            Texture3D texture3D = dataset.getTextureData();
            if (texture3D == null)
            {
                Debug.LogError("Get texture3D failed");
                return null;
            }
            
            print("texture3D created" + texture3D.dimension);
            meshRenderer.material.SetTexture("_dataTex", texture3D);
            // meshRenderer.material.SetTexture("_NoiseTex", null);
            meshRenderer.material.SetInt("_StepCount", 128);

            print("finished creating volume object");

            return outerObject;
        }
    }
}