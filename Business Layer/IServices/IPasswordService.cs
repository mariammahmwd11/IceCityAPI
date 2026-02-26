using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.IServices
{
    public interface IPasswordService
    {
        string HashPassword(string PassWord);
        bool VarifyHashedPassword(string Password, String HashedPassword);
    }
}
