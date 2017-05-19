using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace VPT
{
    public class BIFFFile
    {
        public byte[] Buffer = null;

        //Return the bytes that make up the BIFFFile
        public byte[] GetBuffer()
        {
            return Buffer;
        }

        //Create a new BIFF file based on a byte array
        public BIFFFile(byte[] B)
        {
            Buffer = B;
        }

        //Is this a little off-standard?
        bool ischunkcode(String S)
        {
            if (S == "CODE") return true;
            return false;
        }

        //Delegate used to call IterateChunks
        public delegate void BlowChunks(string code, byte[] content);

        //Create a hash of all of the DATA (but not the sizes).
        public void Hash(ref HashAlgorithm A)
        {
            IterateChunks(buildhash,A);            
        }

        Object m_IterateObject = null;

        void buildhash(String S, byte[] arr)
        {
            HashAlgorithm HA = (HashAlgorithm)m_IterateObject;
            byte[] b = Encoding.ASCII.GetBytes(S);
            HA.TransformBlock(b,0,4,null,0);
            HA.TransformBlock(arr, 0, arr.Length, null, 0);
        }

        //Iterate through the data, calling the callback function on each chunk.
        public void IterateChunks(BlowChunks Callback, Object cb = null)
        {
            int indx = 0;
            int mx = Buffer.Length;            

            m_IterateObject = cb;            

            int chunklen = BitConverter.ToInt32(Buffer, indx);
            while (indx < mx)
            {
                string ChunkName = Encoding.ASCII.GetString(Buffer, indx + 4, 4);
                
                byte[] Buf = new byte[chunklen];
                int offset = 0;
                if (chunklen == 4) 
                {
                    if (ChunkName == "ENDB")
                    {
                        Callback(ChunkName, Buf);
                        indx += 8;
                        continue;                 
                    }
                    int RealCount = 0;                    
                    for(int q= indx;q< mx;q++)
                    {
                        if ((Buffer[q-3] == 'E') && (Buffer[q - 2] == 'N') && (Buffer[q - 1] == 'D') && (Buffer[q] == 'B'))
                        {
                            RealCount = (q - 4) - (indx + 8);
                        }
                    }
                    chunklen = RealCount+1;
                    Buf = new byte[chunklen];                    
                }
                for (int x = 0; x < chunklen; x++)
                {
                    Buf[x] = Buffer[x + indx + 8];
                }

                Callback(ChunkName, Buf);

                indx += chunklen + 4;
               
                chunklen = BitConverter.ToInt32(Buffer, indx);
            }
        }

        public byte[] GetChunk(string Code)
        {
            int indx = 0;
            int mx = Buffer.Length;

            int chunklen = BitConverter.ToInt32(Buffer, indx);
            while (indx < mx)
            {
                string ChunkName = Encoding.ASCII.GetString(Buffer, indx + 4, 4);
                if (ChunkName == Code)
                {
                    byte[] Buf = new byte[chunklen];
                    if ((chunklen == 4) && (ChunkName == "CODE"))
                    {
                        int RealCount = BitConverter.ToInt32(Buffer, indx + 8);
                        chunklen = RealCount;
                        Buf = new byte[chunklen];
                    }
                    for (int x = 0; x < chunklen; x++)
                    {
                        Buf[x] = Buffer[x + indx + 12];
                    }
                    return Buf;
                }
                else
                {
                    if ((chunklen == 4) && (ChunkName == "CODE"))
                    {                        
                        int RealCount = BitConverter.ToInt32(Buffer, indx + 8);
                        chunklen = RealCount;
                    }
                    indx += chunklen + 4;
                }
                chunklen = BitConverter.ToInt32(Buffer, indx);
            }
            return null;
        }

        public void SetChunk(string chunk, byte[] value)
        {
            int indx = 0;
            int mx = Buffer.Length;
            List<byte> NewBuffer = new List<byte>();

            int chunklen = BitConverter.ToInt32(Buffer, indx);
            while (indx < mx)
            {
                string ChunkName = Encoding.ASCII.GetString(Buffer, indx + 4, 4);
                if (ChunkName == chunk)
                {
                    byte[] Buf = new byte[chunklen - 4];
                    if ((ischunkcode(ChunkName)) && (chunklen == 4))
                    {

                        for (int n = 0; n < 4; n++)
                        {
                            NewBuffer.Add(Buffer[indx + n]);
                        }
                        for (int n = 0; n < 4; n++)
                        {
                            NewBuffer.Add(Buffer[indx + n + 4]);
                        }

                        int RealCount = BitConverter.ToInt32(Buffer, indx + 8);
                        chunklen = RealCount;

                        int NewChunkLen = value.Length;
                        byte[] bits = BitConverter.GetBytes(NewChunkLen);
                        for (int n = 0; n < 4; n++)
                        {
                            NewBuffer.Add(bits[n]);
                        }
                        for (int n = 0; n < value.Length; n++)
                        {
                            NewBuffer.Add(value[n]);
                        }
                        indx += chunklen + 12;
                    }
                    else
                    {
                        byte[] bits = BitConverter.GetBytes(value.Length);
                        for (int n = 0; n < 4; n++)
                        {
                            NewBuffer.Add(bits[n]);
                        }

                        for (int n = 0; n < 4; n++)
                        {
                            NewBuffer.Add(Buffer[indx + n + 4]);
                        }

                        for (int n = 0; n < value.Length; n++)
                        {
                            NewBuffer.Add(value[n]);
                        }
                        indx += chunklen + 4;
                    }
                }
                else
                {
                    if ((ischunkcode(ChunkName)) && (chunklen == 4))
                    {
                        int RealCount = BitConverter.ToInt32(Buffer, indx + 8);
                        chunklen = RealCount;

                        for (int n = 0; n < 4; n++)
                        {
                            NewBuffer.Add(Buffer[indx + n]);
                        }
                        for (int n = 0; n < 4; n++)
                        {
                            NewBuffer.Add(Buffer[indx + n + 4]);
                        }

                        for (int n = 0; n < 4; n++)
                        {
                            NewBuffer.Add(Buffer[indx + n + 8]);
                        }

                        for (int n = 0; n < chunklen; n++)
                        {
                            NewBuffer.Add(Buffer[indx + n + 12]);
                        }
                    }
                    else
                    {
                        for (int n = 0; n < 4; n++)
                        {
                            NewBuffer.Add(Buffer[indx + n]);
                        }
                        for (int n = 0; n < 4; n++)
                        {
                            NewBuffer.Add(Buffer[indx + n + 4]);
                        }

                        for (int n = 0; n < chunklen - 4; n++)
                        {
                            NewBuffer.Add(Buffer[indx + n + 8]);
                        }
                    }
                    indx += chunklen + 4;
                }

                if (indx >= mx) break;
                try
                {
                    chunklen = BitConverter.ToInt32(Buffer, indx);
                }
                catch
                {
                    break;
                }
            }

            Buffer = NewBuffer.ToArray();
        }
    }
}
