using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API.Validators
{
    public static class CustomValidations
    {
        public static bool CPFValido(string cpf)
        {
            if(!Regex.IsMatch(cpf, @"^\d{11}$"))
                return false;

            int DV1 = 0, DV2 = 0;

            for(int i = 0; i < 10; ++i)
            {
                if(i < 9)
                    DV1 += (10 - i) * (cpf[i] - '0');

                DV2 += (11 - i) * (cpf[i] - '0');
            }

            Console.WriteLine(cpf + " " + DV1 + " " + DV2);

            DV1 %= 11;
            DV1 = DV1 < 2 ? 0 : 11 - DV1;
            DV2 %= 11;
            DV2 = DV2 < 2 ? 0 : 11 - DV2;
            
            return cpf.EndsWith("" + DV1 + DV2);
        }

        public static bool CelularValido(string celular) => Regex.IsMatch(celular, @"^\d{2}9\d{8}$");

        public static bool RGValido(string rg) => Regex.IsMatch(rg, @"^\d{8,9}$");

        public static bool PasswordValido(string password)
        {
            List<char> specialCharacters = new List<char>
            {
                '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', ':',
                ';', '<', '=', '>', '?', '@', '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~'
            };

            List<char> digits = new List<char>();

            for(char c = '0'; c <= '9'; ++c)
                digits.Add(c);
            
            List<char> lowercaseLetters = new List<char>();

            for (char c = 'a'; c <= 'z'; c++)
                lowercaseLetters.Add(c);

            List<char> uppercaseLetters = new List<char>();

            for (char c = 'A'; c <= 'Z'; c++)
                uppercaseLetters.Add(c);

            bool containsSpecialCharacter = false;
            bool containsDigit = false;
            bool containsLowerCaseLetter = false;
            bool containsUpperCaseLetter = false;

            foreach (char ch in password)
            {
                containsSpecialCharacter = containsSpecialCharacter || specialCharacters.Contains(ch);
                containsDigit = containsDigit || digits.Contains(ch);
                containsLowerCaseLetter = containsLowerCaseLetter || lowercaseLetters.Contains(ch);
                containsUpperCaseLetter = containsUpperCaseLetter || uppercaseLetters.Contains(ch);
            }

            return containsSpecialCharacter && containsDigit && containsLowerCaseLetter && containsUpperCaseLetter;
        }
    }
}