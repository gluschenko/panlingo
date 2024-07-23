﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LangaugeCode.Core.Models
{
    public enum ISO639Set
    {
        /// <summary>
        /// Set 1: two-letter language identifiers (originally as in ISO 639-1:2002) for major, 
        /// mostly national individual languages.
        /// </summary>
        Set1 = 1,
        /// <summary>
        /// Set 2: three-letter language identifiers (originally as in ISO 639-2:1998) for 
        /// a larger number of widely known individual languages (including all individual 
        /// languages covered by Set 1) and a number of language groups.
        /// </summary>
        Set2 = 2,
        /// <summary>
        /// Set 3: three-letter language identifiers (originally as in ISO 639-3:2007) 
        /// covering all individual languages (including all individual languages covered by Set 2), 
        /// including living, extinct and ancient languages.
        /// </summary>
        Set3 = 3,
        /// <summary>
        /// Set 5: three-letter language identifiers (originally as in ISO 639-5:2008) covering 
        /// a larger set of language groups, living and extinct (including all language 
        /// groups covered by Set 2).
        /// </summary>
        Set5 = 5,
    }
}