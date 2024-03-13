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

        public Dataset3D crop(string newName, int dimX, int dimY, int dimZ, Vector3 center)
        {
            // check for dim range
            if (dimX <= 0 || dimY <= 0 || dimZ <= 0 || dimX > this.dimX || dimY > this.dimY || dimZ > this.dimZ)
            {
                Debug.LogError("Invalid crop dimension");
                return null;
            }
            
            // check for center pos
            if (center.x < 0 || center.x >= this.dimX || center.y < 0 || center.y >= this.dimY || center.z < 0 || center.z >= this.dimZ)
            {
                Debug.LogError("Invalid crop center");
                return null;
            }
            
            Dataset3D newDataset = new Dataset3D();

            if (newName != "")
            {
                newDataset.datasetName = newName;
            }
            
            newDataset.dimX = dimX;
            newDataset.dimY = dimY;
            newDataset.dimZ = dimZ;
            newDataset.data = new float[dimX * dimY * dimZ];

            int halfDimX = dimX / 2;
            int halfDimY = dimY / 2;
            int halfDimZ = dimZ / 2;
            
            float minVal = float.MaxValue;
            float maxVal = float.MinValue;

            for (int z = 0; z < dimZ; z++)
            {
                for (int y = 0; y < dimY; y++)
                {
                    for (int x = 0; x < dimX; x++)
                    {
                        int newX = (int)center.x - halfDimX + x;
                        int newY = (int)center.y - halfDimY + y;
                        int newZ = (int)center.z - halfDimZ + z;

                        if (newX < 0 || newX >= this.dimX || newY < 0 || newY >= this.dimY || newZ < 0 || newZ >= this.dimZ)
                        {
                            Debug.Log("set out of bounds value to 0");
                            newDataset.data[z * dimX * dimY + y * dimX + x] = 0;
                        }
                        else
                        {
                            float oriVal = this.data[newZ * this.dimX * this.dimY + newY * this.dimX + newX];
                            if (oriVal < minVal)
                            {
                                minVal = oriVal;
                            }

                            if (oriVal > maxVal)
                            {
                                maxVal = oriVal;
                            }
                            newDataset.data[z * dimX * dimY + y * dimX + x] = oriVal;
                        }
                    }
                }
            }

            newDataset.minDataValue = minVal;
            newDataset.maxDataValue = maxVal;
            // log out
            // Debug.Log("crop min value: " + minVal + " max value: " + maxVal);

            return newDataset;
        }
    }
}