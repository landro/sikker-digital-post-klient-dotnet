﻿/** 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *         http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace SikkerDigitalPost.Domene.Exceptions
{
    public class XmlValidationException : KonfigurasjonsException
    {
        private const string Ekstrainfo = " Validering av Xml feilet. Se inner exception for mer info.";

        public XmlValidationException()
        {
            
        }

        public XmlValidationException(string message) : base(message + Ekstrainfo)
        {
            
        }

        public XmlValidationException(string message, Exception inner) 
            : base (message + Ekstrainfo, inner)
        {
            
        }
    }
}
