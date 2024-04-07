using System;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using Network;

namespace DataLoader
{
    public class V3dRawDataLoader: MonoBehaviour
    {
        public static Dataset3D loadV3dRawImage(byte[] imageData) //only support uint8 data type
        {
            Debug.Log("start loading v3draw file");
            int bytesToSkip = 43;
            string formatKey = "raw_image_stack_by_hpeng";
            
            byte[] stringByteArray = imageData.Take(24).ToArray();
            string readFormatKey = System.Text.Encoding.UTF8.GetString(stringByteArray);
            if (readFormatKey != formatKey)
            {
                // Debug.Log(readFormatKey);
                Debug.LogError("This format is not supported");
                return null;
            }
            
            Int32[] size = new Int32[4];
            for (int i = 0; i < 4; i++)
            {
                int offset = 27+i*4;
                int length = 4;
                byte[] subArray = imageData.SubArray(offset,length);
                Int32 number = BitConverter.ToInt32(subArray,0);
                size[i] = number;
            }
            
            int dimX = size[0];
            int dimY = size[1];
            int dimZ = size[2];

            int totalCount = dimX * dimY * dimZ;
            
            float[] rawImageData = new float[totalCount];
            float maxValue = float.MinValue;
            float minValue = float.MaxValue;

            print("total count" + totalCount);
            print("total bytes" + imageData.Length);
            
            for (int i = bytesToSkip; i < totalCount+bytesToSkip; i++)
            {
                int rawImageDataIndex = i - bytesToSkip;
                rawImageData[rawImageDataIndex] = (float)(imageData[i]);
                if (rawImageData[rawImageDataIndex] > maxValue)
                {
                    maxValue = rawImageData[rawImageDataIndex];
                }

                if (rawImageData[rawImageDataIndex] < minValue)
                {
                    minValue = rawImageData[rawImageDataIndex];
                }
            }
            
            print("max value " + maxValue + " min value " + minValue);

            Dataset3D newDateset = new Dataset3D();
            newDateset.datasetName = "testImage";
            newDateset.data = rawImageData;
            newDateset.dimX = dimX;
            newDateset.dimY = dimY;
            newDateset.dimZ = dimZ;
            newDateset.setMaxMin(maxValue, minValue);
            
            Debug.Log("finish loading v3draw file");
            
            return newDateset;
        }
        
        public static Dataset3D readV3dRawFromLocalFile(string filePath)
        {
            byte[] byteArray = File.ReadAllBytes(filePath);
            return loadV3dRawImage(byteArray);
        }

        public async static Task<Dataset3D> readV3dRawFromURL(string imageName)
        {
            byte[] imgeBytes = await NetworkRequest.DownloadImage3d(imageName);
            return loadV3dRawImage(imgeBytes);
        }
    }
    
    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            return new ArraySegment<T>(array, offset, length)
                .ToArray();
        }
    
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length==j) ? Arr[0] : Arr[j];            
        }
    }
}