using BookingLibrary.Helpers.Hash.HashTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingLibrary.Helpers.Hash
{
    public class HashHelper
    {

        public static IHash Type(HashType hashType)
        {
            switch (hashType) 
            {
                case HashType.MD5: return new Md5Hash();
                case HashType.SHA256: return new Sha256Hash();
                default: throw new Exception("Hash not found");
            }
            
        }
    }
    public enum HashType
    {
        MD5,
        SHA256,
    }
}
