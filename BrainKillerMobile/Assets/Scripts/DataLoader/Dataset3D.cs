using UnityEngine;
using System;

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
                dataTexture = CreateTextureData();
            }

            return dataTexture;
        }

        private Texture3D CreateTextureData()
        {
            // using RHalf as default format,for using less memory
            Texture3D texture = new Texture3D(dimX, dimY, dimZ, TextureFormat.RHalf, false);

            float maxRange = maxDataValue - minDataValue;
            
            int sampleSize = 2;
            byte[] bytes = new byte[data.Length * sampleSize]; // This can cause OutOfMemoryException
            for (int iData = 0; iData < data.Length; iData++)
            {
                float pixelValue = (float)(data[iData] - minDataValue) / maxRange;
                byte[] pixelBytes = BitConverter.GetBytes(Mathf.FloatToHalf(pixelValue));

                Array.Copy(pixelBytes, 0, bytes, iData * sampleSize, sampleSize);
            }

            texture.SetPixelData(bytes, 0);
            
            texture.Apply();
            return texture;
        }
    }
}