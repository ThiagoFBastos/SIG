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
    }
}