using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class PasswordHashUnitTest
    {
        [Theory]
        [InlineData("15158114099Aa$$")]
        [InlineData("15158114099Aa")]
        [InlineData("15158114099")]
        [InlineData("1")]
        [InlineData(",$clW.Fleo4!04fG..E5432!<.>E´-")]
        public void CompareHashes(string password)
        {
            PasswordHash passwordHash = new PasswordHash();

            string salt;

            string hash = passwordHash.Encrypt(password, out salt);

            bool isValid = passwordHash.Decrypt(password, hash, salt);

            Assert.True(isValid, "A senhas não são iguais");
        }
    }
}
