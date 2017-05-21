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
        
        //Delegate used to call IterateChunks
        public delegate void BlowChunks(string code, byte[] content);

        Object m_IterateObject = null;        

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
                
                byte[] Buf = new byte[chunklen-4];
                int offset = 8;
                if (chunklen == 4)
                {
                    
                    //Console.WriteLine(ChunkName);
                    if (ChunkName == "CODE")
                    {
                        //This is a bit of an oddity - read the length AGAIN.
                        int newchunklen = BitConverter.ToInt32(Buffer, indx+8);

                        chunklen = newchunklen;
                        offset = 12;
                        Buf = new byte[newchunklen];
                    }
                    else
                    {
                        if (ChunkName == "FONT")
                        {
                            //Fonts are really odd.                            
                            int newchunklen = (Buffer[indx+17] << 8) | Buffer[indx+18];                            

                            chunklen = newchunklen;
                            offset = 19;
                            Buf = new byte[newchunklen];
                        }
                        else
                        {
                            chunklen -= 4;
                        }
                    }                    
                }
                else
                {
                    chunklen -= 4;
                    //offset = 8;
                }

                for (int x = 0; x < chunklen; x++)
                {
                    Buf[x] = Buffer[x + indx + offset];
                }

                Callback(ChunkName, Buf);

                indx += chunklen + offset;
                if (indx == mx) break;
               
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
                    if ((ChunkName == "CODE") && (chunklen == 4))
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
                    if ((ChunkName == "CODE") && (chunklen == 4))
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
