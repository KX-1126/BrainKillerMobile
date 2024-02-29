using UnityEngine;

namespace DataLoader
{
    public class Dataset3D
    {
        public string datasetName = "";
        public float[] data;

        public int dimX, dimY, dimZ;

        private float minDataValue = float.MaxValue;
        private float maxDataValue = float.MinValue;

        private Texture3D dataTexture = null;

        public void setMaxMin(float max,float min)
        {
            maxDataValue = max;
            minDataValue = min;
        }

        public Texture3D getTextureData()
        {
            // convert data to texture3d
            if (dataTexture == null)
            {
                CreateTextureData();
            }

            return dataTexture;
        }

        private void CreateTextureData()
        {
            // using RHalf as default format,for using less memory
            Texture3D dataTexture = new Texture3D(dimX, dimY, dimZ, TextureFormat.RHalf, false);

            float maxRange = maxDataValue - minDataValue;
            
            for (int x = 0; x < dimX; x++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    for (int z = 0; z < dimZ; z++)
                    {
                        dataTexture.SetPixel(x,y,z,new Color((float)(data[x + y * dimX + z * (dimX * dimY)] - minDataValue) / maxRange, 0.0f, 0.0f, 0.0f));
                    }
                }
            }
            
            dataTexture.Apply();
        }
    }
}