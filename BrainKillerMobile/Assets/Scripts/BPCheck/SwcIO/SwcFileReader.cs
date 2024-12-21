using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Text;
using UnityEngine;

namespace BPCheck.SwcIO
{
    public class SwcFileReader
    {
        private const int MAX_RETRIES = 5;
        private const int RETRY_DELAY_MS = 100;
        private MemoryMappedFile _mappedFile;

        public string readSwcFile(string path)
        {
            string mmapSwcPath = "F:\\Repos\\Tree_CRDT\\tree_255.swc";
            int fileSize = 10 * 1024 * 1024;
            for (int i = 0; i < MAX_RETRIES; i++)
            {
                 try
                {
                  using (FileStream fs = new FileStream(mmapSwcPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                   {
                     _mappedFile = MemoryMappedFile.CreateFromFile(fs, null, fileSize, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, false);
                         using (MemoryMappedViewStream stream = _mappedFile.CreateViewStream())
                        {
                             using (StreamReader reader = new StreamReader(stream))
                            {
                                string content = reader.ReadToEnd();
                                return parseSwcFile(content);
                             }
                        }
                    }
                }
                catch (IOException ex)
                {
                    if (i == MAX_RETRIES - 1)
                    {
                        Console.WriteLine($"读取文件失败，已达到最大重试次数:{ex.Message}");
                         throw;
                    }
                      Console.WriteLine($"读取文件失败，重试中...:{ex.Message}，重试次数:{i+1}");
                        // 等待一段时间后重试
                        Thread.Sleep(RETRY_DELAY_MS);
                   
                }
                finally
                {
                    if (_mappedFile != null)
                    {
                        _mappedFile.Dispose();
                        _mappedFile = null;
                    }
                }
            }
            return "";
        }

        public string parseSwcFile(string content)
        {
            // remove useless nulls
            string validContent = removeNullLines(content);
            return validContent;
        }
        
        public string removeNullLines(string content)
        {
            // remove all '\0' characters
            StringBuilder sb = new StringBuilder();
            foreach (char c in content)
            {
                if (c != '\0')
                {
                    sb.Append(c);
                } else {
                    break;
                }
            }

            return sb.ToString();
        }
    
    }
}