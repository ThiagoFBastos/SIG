﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Options
{
    public class TokensServiceOptions
    {
        public required string SecretKey {  get; set; }

        public required string Audience { get; set; }

        public required string Issuer { get; set; }
    }
}
